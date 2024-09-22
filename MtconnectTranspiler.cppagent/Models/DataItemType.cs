using MtconnectTranspiler.Sinks.CSharp;
using MtconnectTranspiler.Sinks.CSharp.Contracts.Interfaces;

namespace MtconnectTranspiler.cppagent.Models
{
    public class DataItemType
    {
        /// <summary>
        /// The name of the Data Item Type/Sub-Type.
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

        /// <summary>
        /// Collection of enumerable values applicable to this type.
        /// </summary>
        public ObservationValue[] Values {  get; set; }

        /// <summary>
        /// Collection of available sub-types.
        /// </summary>
        public DataItemType[]? SubTypes { get; set; }

        public DataItemType(ObservationType source)
        {
            Name = source.Name;
            Introduced = source.Introduced;
            Deprecated = source.Deprecated;

            var resultProperty = source.Properties.FirstOrDefault(o => o.Name == "result");
            IEnum resultInstance = null;
            MtconnectTranspiler.Sinks.CSharp.Contracts.Interfaces.IEnumInstance[] values = null;
            if (resultProperty?.Type?.GetInterfaces()?.Any(o => o == (typeof(IEnum))) == true)
            {
                resultInstance = Activator.CreateInstance(resultProperty.Type) as IEnum;
                if (resultInstance != null)
                {
                    values = resultInstance.Values.OrderBy(o => o.NormativeVersion).ThenBy(o => o.Name).ToArray();
                }
            }
            Values = values?.Select(o => new ObservationValue(o))?.ToArray();
        }
    }
}
