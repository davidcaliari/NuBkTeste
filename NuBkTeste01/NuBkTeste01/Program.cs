using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NuBkTeste01.Business;
using NuBkTeste01.Business.Implementation;
using NuBkTeste01.Data.VO;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NuBkTeste01
{
    
    public class Program
    {
        

        private enum Operations
        {
            Account_Creation = 1,
            Transaction_Autorization = 2,
            Leave = 3
        }

        static AccountsVO accounts;

        public Program()
        {
            
        }

        static void Main(string[] args)
        {
            //Start config settings 
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

            var svcAccount = ActivatorUtilities.CreateInstance<AccountBusinessImplementation>(host.Services);
            
            
            var account = new AccountVO();
            var violations = new ViolationsVO();
            var operation = StartGreeting();

            while(operation != Operations.Leave)
            {
                switch (operation)
                {
                    case Operations.Account_Creation:
                        Log.Logger.Information("\n Amount to be credited:");
                        var creditValue = Console.ReadLine();

                        account.activeCard = true;
                        account.availableLimit = Convert.ToInt32(creditValue);

                        accounts = new AccountsVO() { accounts = account };
                        //accounts.accounts = account;

                        Console.Clear();

                        Log.Logger.Information(JsonSerializer.Serialize(accounts));
                        operation = StartGreeting();
                        break;
                    case Operations.Transaction_Autorization:
                        Console.Clear();
                        Log.Logger.Information(JsonSerializer.Serialize(accounts));
                        operation = StartGreeting();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Creat the start page console
        /// </summary>
        private static Operations StartGreeting()
        {
            string option = startView();

            var errorMessage = ValidateOperation(option);

            while (!string.IsNullOrEmpty(errorMessage))
            {
                Console.Clear();
                Log.Logger.Information(errorMessage);

                option = startView();
                errorMessage = ValidateOperation(option);
            }

            Console.Clear();
            return (Operations)Convert.ToInt32(option);
        }

        /// <summary>
        /// Create and control start layout with validations
        /// </summary>
        /// <returns>Selected Operation</returns>
        private static string startView()
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

            return Console.ReadLine();
        }

        /// <summary>
        /// Validate the operations
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Message Error</returns>
        public static string ValidateOperation(string param)
        {
            int value = 0;
            
            if (int.TryParse(param, out value) && Enum.IsDefined(typeof(Operations), value))
            {
                if((int)Operations.Account_Creation == value && accounts != null) return string.Format("You can have only one account! {0}", JsonSerializer.Serialize(accounts));
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
