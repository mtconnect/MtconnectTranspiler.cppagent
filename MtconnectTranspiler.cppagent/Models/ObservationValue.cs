using MtconnectTranspiler.Sinks.CSharp.Contracts.Interfaces;

namespace MtconnectTranspiler.cppagent.Models
{
    public class ObservationValue
    {
        /// <summary>
        /// The name of the enumerable observation value.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Version that the Data Item Type/Sub-Type was introduced.
        /// </summary>
        public string Introduced { get; set; }

        /// <summary>
        /// Version that the Data Item Type/Sub-Type was deprecated.
        /// </summary>
        public string Deprecated { get; set; }

        public ObservationValue(IEnumInstance source)
        {
            Name = source.Name;
            Introduced = source.NormativeVersion;
            Deprecated = source.DeprecatedVersion;
        }
    }
}
