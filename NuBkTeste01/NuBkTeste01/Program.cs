using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    internal class Program
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

        private static void StartGreeting()
        {
            Log.Logger.Information("\n Operations:\n {0}:         Option: {1} \n {2}: Option: {3}",

                            Operations.Account_Creation.ToString().Replace("_", " "),
                            (int)Operations.Account_Creation,

                            Operations.Transaction_Autorization.ToString().Replace("_", " "),
                            (int)Operations.Transaction_Autorization);

            int option = Convert.ToInt32(Console.ReadLine());
            Log.Logger.Information("Operation chose was: {0}", ((Operations)option).ToString().Replace("_", " "));
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
