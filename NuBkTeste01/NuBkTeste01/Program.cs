﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NuBkTeste01.Business;
using NuBkTeste01.Business.Implementation;
using NuBkTeste01.Data.VO;
using Serilog;
using System;
using System.IO;
using System.Text.Json;

namespace NuBkTeste01
{

    public class Program
    {


        public enum Operations
        {
            Account_Creation = 1,
            Transaction_Autorization = 2,
            Leave = 3
        }

        public static AccountBusinessImplementation svcAccount;
        public static AccountsVO accounts;

        static void Main(string[] args)
        {
            //Start config settings 
            BeginConfig();

            //Start Applications with rules
            Run();

            var violations = new ViolationsVO();

        }

        /// <summary>
        /// Configurations to start the application
        /// </summary>
        private static void BeginConfig()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            //Config Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            //Injection Dependency
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IAccountBusiness, AccountBusinessImplementation>();
                })
                .UseSerilog()
                .Build();

            svcAccount = ActivatorUtilities.CreateInstance<AccountBusinessImplementation>(host.Services);
        }


        private static void Run()
        {
            var operation = startView();

            while (operation != Operations.Leave)
            {
                switch (operation)
                {
                    case Operations.Account_Creation:

                        Console.Clear();
                        //Verify with alredy exist the account
                        if (accounts != null)
                        {
                            Log.Logger.Information(string.Format("You can have only one account! {0}", JsonSerializer.Serialize(accounts)));
                            operation = startView();
                            break;
                        }

                        //if the amout typed is valid create account
                        Log.Logger.Information("\n Amount to be credited:");
                        var creditValue = Console.ReadLine();
                        if(!ValidateCreditInput(creditValue)) break;

                        //call business to create accout
                        accounts  = svcAccount.CreateAccount(creditValue);

                        Console.Clear();
                        Log.Logger.Information(JsonSerializer.Serialize(accounts));

                        operation = startView();
                        break;
                    case Operations.Transaction_Autorization:
                        Console.Clear();
                        Log.Logger.Information(JsonSerializer.Serialize(accounts));
                        operation = startView();
                        break;
                    default:
                        Console.Clear();
                        Log.Logger.Information("Chose a valid option!");
                        startView();
                        break;
                }
            }
        }

        private static bool ValidateCreditInput(string input)
        {
            int value = 0;
            if (int.TryParse(input, out value) && value > 0) return true;
            
            Console.Clear();
            Log.Logger.Information("\n Invalid amount! Try again!");
            return false;
        }

        /// <summary>
        /// Create and control start layout with validations
        /// </summary>
        /// <returns>Selected Operation</returns>
        private static Operations startView()
        {
            Log.Logger.Information("\n Operations:\n    {0}:         Option: {1}" +
                " \n    {2}: Option: {3}" +
                " \n    {4}:                    Option: {5}",

                                        Operations.Account_Creation.ToString().Replace("_", " "),
                                        (int)Operations.Account_Creation,

                                        Operations.Transaction_Autorization.ToString().Replace("_", " "),
                                        (int)Operations.Transaction_Autorization,

                                        Operations.Leave.ToString(),
                                        (int)Operations.Leave);
            string input = Console.ReadLine();

            int value = 0;
            if (!int.TryParse(input, out value) && !Enum.IsDefined(typeof(Operations), value))
            {
                Console.Clear();
                Log.Logger.Information("Chose a valid option!");
                value = (int)startView();
            }
                return (Operations)value;
        }

        /// <summary>
        /// Validate the operations
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Message Error</returns>
        public static string ValidateOperation(string param)
        {
            //Only valid operations
            int value = 0;
            if (int.TryParse(param, out value) && Enum.IsDefined(typeof(Operations), value))
            {
                //Validate multiple accounts
                if ((int)Operations.Account_Creation == value && accounts != null)
                {
                    return string.Format("You can have only one account! {0}", JsonSerializer.Serialize(accounts));
                }
                return string.Empty;
            }
            else
            {
                return "Chose a valid option!";
            }
        }

        /// <summary>
        /// Prepare config settings
        /// </summary>
        /// <param name="builder"></param>
        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
