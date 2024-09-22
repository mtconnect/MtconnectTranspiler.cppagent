using Microsoft.Extensions.Logging;
using MtconnectTranspiler.CodeGenerators.ScribanTemplates;
using MtconnectTranspiler.cppagent.Models;

namespace MtconnectTranspiler.cppagent
{
    internal class Transpiler
    {
        /// <inheritdoc cref="ILogger"/>
        private readonly ILogger<Transpiler> _logger;
        private readonly IScribanTemplateGenerator _generator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectPath">Expected to be <c>MtconnectCore/Standard/Contracts</c></param>
        public Transpiler(IScribanTemplateGenerator generator, ILogger<Transpiler> logger = default)
        {
            _generator = generator;
            _logger = logger;
        }

        public async Task TranspileAsync(CancellationToken token = default)
        {
            _logger?.LogInformation("Received MTConnectModel, beginning transpilation");

            var dataItems = new DataItemTypes()
            {
                Items = MtconnectTranspiler.Sinks.CSharp.NavigationExtensions
                    .GetObservationTypes()
                    .Select(o => new DataItemType(o))
                    .ToArray()
            };

            _logger?.LogInformation($"Processing data types");
            _generator.ProcessTemplate(dataItems, Path.Combine(_generator.OutputPath), true);

            // TODO: Handle Enum and Class types for properties. Replace *EnumMetaClass with reference to newly created Enum. Replace *Class with reference to other interfaces.
            //var classes = new List<MtconnectCoreInterface>();
            //foreach (var item in MtconnectModel.Packages)
            //    getClasses(item, classes);
            //_generator.ProcessTemplate(classes, Path.Combine(_generator.OutputPath, "Interfaces"), true);
        }
    }
}
