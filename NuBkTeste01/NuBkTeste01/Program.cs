using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace NuBkTeste01
{
    public enum Operations
    {
        Account_Creation = 1,
        Transaction_Autorization = 2
    }
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            StartGreeting();
        }

        /// <summary>
        /// Creat the start page console
        /// </summary>
        private static void StartGreeting()
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

            var operation = (Operations)Convert.ToInt32(option);
            Log.Logger.Information("Operation chose was: {0}", operation.ToString().Replace("_", " "));
        }

        /// <summary>
        /// Create and control start layout with validations
        /// </summary>
        /// <returns>Selected Operation</returns>
        private static string startView()
        {
            Log.Logger.Information("\n Operations:\n {0}:         Option: {1} \n {2}: Option: {3}",

                                        Operations.Account_Creation.ToString().Replace("_", " "),
                                        (int)Operations.Account_Creation,

                                        Operations.Transaction_Autorization.ToString().Replace("_", " "),
                                        (int)Operations.Transaction_Autorization);

            return Console.ReadLine();
        }

        /// <summary>
        /// Validate the operations
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Message Error</returns>
        public static string ValidateOperation(string param)
        {
            int i = 0;
            if (int.TryParse(param, out i))
            {
                return string.Empty;
            }
            else
            {
                return "Chose a valid option!";
            }
        }


        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
