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


        public enum Operations
        {
            Account_Creation = 1,
            Transaction_Autorization = 2,
            Leave = 3
        }

        public static AccountBusinessImplementation svcAccount;

        //public AccountsVO accounts;
        //public static TransactionsVO transactions;
        //public List<AccountsVO> logAccounts = new List<AccountsVO>();

        static void Main(string[] args)
        {


            //Start config settings 
            BeginConfig();

            //Start Applications with rules
            startViewJson(new List<string>());
            //Run();

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


        //private static void Run()
        //{
        //    var operation = startView();

        //    while (operation != Operations.Leave)
        //    {
        //        var creditValue = string.Empty;
        //        switch (operation)
        //        {
        //            case Operations.Account_Creation:

        //                Console.Clear();
        //                //Verify with alredy exist the account
        //                if (accounts != null)
        //                {
        //                    Log.Logger.Information(string.Format("You can have only one account! {0}", JsonSerializer.Serialize(accounts)));
        //                    operation = startView();
        //                    break;
        //                }

        //                //if the amout typed is valid create account
        //                Log.Logger.Information("\n Amount to be credited:");
        //                creditValue = Console.ReadLine();
        //                if (!ValidateCreditInput(creditValue)) break;

        //                //call business to create accout
        //                accounts = svcAccount.CreateAccount(creditValue);

        //                Console.Clear();
        //                Log.Logger.Information(JsonSerializer.Serialize(accounts));

        //                operation = startView();
        //                break;
        //            case Operations.Transaction_Autorization:
        //                Console.Clear();

        //                //Verify with alredy exist the account
        //                if (accounts == null)
        //                {
        //                    Log.Logger.Information("Please create account!");
        //                    operation = startView();
        //                    break;
        //                }

        //                Log.Logger.Information(JsonSerializer.Serialize(accounts));

        //                var transaction = new TransactionVO();

        //                Log.Logger.Information("\n Input de transaction:");
        //                Log.Logger.Information("\n Merchant:");
        //                var merchant = Console.ReadLine();

        //                Log.Logger.Information("\n Amount:");
        //                creditValue = Console.ReadLine();
        //                if (!ValidateCreditInput(creditValue)) break;



        //                operation = startView();
        //                break;
        //            default:
        //                Console.Clear();
        //                Log.Logger.Information("Chose a valid option!");
        //                startView();
        //                break;
        //        }
        //    }
        //}

        //private static bool ValidateCreditInput(string input)
        //{
        //    int value = 0;
        //    if (int.TryParse(input, out value) && value > 0) return true;

        //    Console.Clear();
        //    Log.Logger.Information("\n Invalid amount! Try again!");
        //    return false;
        //}

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

                    if (createAccount.account != null)
                    {
                        ExecuteOperations(accounts, logAccounts, createAccount);

                    }
                    else if (transactionAuthorization.transaction != null)
                    {
                        ExecuteOperations(accounts, logAccounts, transactionAuthorization);
                    }
                    else
                    {
                        isValid = false;
                    }

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
                
                listJsonTest.Remove(input);//.RemoveAt(listJsonTest.Count - 1);
                isValid = unitTest? listJsonTest.Any() : true;
            }
            return accounts;
        }

        /// <summary>
        /// Moviment Account
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="logAccounts"></param>
        /// <param name="internalTransaction"></param>
        private static void ExecuteOperations(AccountsVO accounts, List<AccountsVO> logAccounts, TransactionsVO internalTransaction)
        {
            if (accounts.account.availableLimit >= internalTransaction.transaction.amount)
            {
                accounts.account.availableLimit = accounts.account.availableLimit - internalTransaction.transaction.amount;
                logAccounts.Add(DuplicateObject(accounts));
            }
            else
            {
                var newAccount = DuplicateObject(accounts);
                newAccount.violations.Add("insufficient-limit");
                logAccounts.Add(DuplicateObject(newAccount));
            }
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="logAccounts"></param>
        /// <param name="internalAccount"></param>
        private static void ExecuteOperations(AccountsVO accounts, List<AccountsVO> logAccounts, AccountsVO internalAccount)
        {
            if (accounts.account == null && internalAccount.account.availableLimit > 0)
            {
                accounts.account = internalAccount.account;
                logAccounts.Add(DuplicateObject(accounts));
            }
            else if (accounts.account == null && internalAccount.account.availableLimit < 0)
            {
                var newAccount = DuplicateObject(accounts);
                newAccount.violations.Add("insufficient-limit-to-create-account");
                logAccounts.Add(newAccount);
            }
            else
            {
                var newAccount = DuplicateObject(accounts);
                newAccount.violations.Add("account-limit-reached");
                logAccounts.Add(newAccount);
            }
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

        private static AccountsVO DuplicateObject(AccountsVO input)
        {
            var json = JsonSerializer.Serialize(input);
            var accounts = JsonSerializer.Deserialize<AccountsVO>(json);
            return accounts;
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
        //public static string ValidateOperation(string param)
        //{
        //    //Only valid operations
        //    int value = 0;
        //    if (int.TryParse(param, out value) && Enum.IsDefined(typeof(Operations), value))
        //    {
        //        //Validate multiple accounts
        //        if ((int)Operations.Account_Creation == value && accounts != null)
        //        {
        //            return string.Format("You can have only one account! {0}", JsonSerializer.Serialize(accounts));
        //        }
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        return "Chose a valid option!";
        //    }
        //}

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
