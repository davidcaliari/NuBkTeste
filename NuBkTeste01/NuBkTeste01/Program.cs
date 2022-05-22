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
using System.Linq;
using System.Text.Json;

namespace NuBkTeste01
{

    public class Program
    {
        public static AccountBusinessImplementation svcAccount;

        static void Main(string[] args)
        {
            //Start config settings 
            BeginConfig();

            //Start Applications with rules
            startViewJson(new List<string>());
        }

        /// <summary>
        /// Configurations to start the application
        /// </summary>
        public static void BeginConfig()
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

        /// <summary>
        /// Start method from application
        /// </summary>
        /// <param name="listJsonTest">Only for unit test</param>
        /// <returns></returns>
        public static AccountsVO startViewJson(List<string> listJsonTest, bool unitTest = false)
        {
            var accounts = new AccountsVO();
            var logAccounts = new List<AccountsVO>();
            //{ "account": { "activeCard": true, "availableLimit": 100 } }
            //{ "transaction": { "merchant": "Burger King", "amount": 20, "time": "2019-02-13T10:00:00" } }

            bool isValid = true;
            while (isValid)
            {
                Log.Logger.Information("\nInput Operations:");
                var input = (!unitTest) ? Console.ReadLine() : listJsonTest.FirstOrDefault();

                if (IsValidJSON(input))
                {
                    var createAccount = JsonSerializer.Deserialize<AccountsVO>(input);
                    var transactionAuthorization = JsonSerializer.Deserialize<TransactionsVO>(input);


                    //Create Account
                    if (createAccount.account != null)
                    {
                        logAccounts.Add(
                            svcAccount.CreateAccount(accounts, createAccount)
                        );
                    }
                    //Execute Transaction
                    else if (transactionAuthorization.transaction != null)
                    {
                        logAccounts.Add(
                            svcAccount.MovimentAccount(accounts, transactionAuthorization)
                        );
                    }
                    else
                    {
                        isValid = false;
                    }

                    //Always write de log informations
                    if (!unitTest) Console.Clear();
                    foreach (var account in logAccounts)
                    {
                        Log.Logger.Information(JsonSerializer.Serialize(account));
                    }

                }
                else
                {
                    if (!unitTest) Console.Clear();
                    Log.Logger.Information("\n Invalid json! Try again!");
                }

                //only for unit tests
                listJsonTest.Remove(input);//.RemoveAt(listJsonTest.Count - 1);
                isValid = unitTest ? listJsonTest.Any() : true;
            }
            return accounts;
        }

        /// <summary>
        /// Validate if input is a valid json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool IsValidJSON(string json)
        {
            json = json.Trim();
            if (json.StartsWith("{") && json.EndsWith("}")) //For object
            {
                try
                {
                    var token = JsonDocument.Parse(json);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
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
