﻿using ConsoulLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MtconnectTranspiler.CodeGenerators.ScribanTemplates.Formatters;
using MtconnectTranspiler.Extensions;
using MtconnectTranspiler.Interpreters;

namespace MtconnectTranspiler.cppagent
{
    internal class Program
    {
        static void Main(string[] args)
        {

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
#else
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
#endif
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            //setup our DI
            var services = new ServiceCollection()
                .AddLogging((builder) => {
                    builder.AddConsoulLogger();
                });
            var serviceProvider = services
                .AddSingleton(configuration)
                .AddScribanServices(builder => {
                    string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
                    builder
                        .ConfigureTemplateLoader((loader) =>
                            // Use the local "/Templates" directory to store ".scriban" files
                            loader.UseTemplatesPath(templatePath)
                            // If using embedded resources to store ".scriban" files, then provide the assembly
                            //.UseResourceAssembly(typeof(Transpiler).Assembly, "MtconnectTranspiler.Sinks.CSharp.Example")
                            // Configure a Scriban ScriptObject capable of interpreting SysML comment contents into other formats.
                            .AddMarkdownInterpreter("cpp_docs", new VisualStudioSummaryInterpreter())
                            // Configure a Scriban ScriptObject capable of formatting strings into code safe formats.
                            .AddCodeFormatter("cpp_formatter", new CppCodeFormatter())
                        )
                        .ConfigureGenerator((options) => {
                            options.OutputPath = configuration["OutputPath"];
                        });
                })
                .AddScoped<Transpiler>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            using (var tokenSource = new CancellationTokenSource())
            {

                var defaultTranspiler = serviceProvider.GetService<Transpiler>();
                var task = Task.Run(() => defaultTranspiler.TranspileAsync());

#if DEBUG
                task = task.ContinueWith((t) => tokenSource.Cancel());
                Consoul.Wait(cancellationToken: tokenSource.Token);
#else
            task.Wait();
#endif

                if (task.IsCompletedSuccessfully)
                {
                    Consoul.Write("Done!", ConsoleColor.Green);

                    Environment.Exit(0);
                }
                else
                {
                    Consoul.Write("Cancelled", ConsoleColor.Red);
                    Environment.Exit(1);
                }

            }
        }
    }
}
