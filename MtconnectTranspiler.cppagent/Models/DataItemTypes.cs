
using MtconnectTranspiler.CodeGenerators.ScribanTemplates;

namespace MtconnectTranspiler.cppagent.Models
{
    [ScribanTemplate("observation_validations.scriban")]
    public class DataItemTypes : IFileSource
    {
        public DataItemType[] Items { get; set; }


        /// <summary>
        /// Internal reference to the class filename.
        /// </summary>
        protected string _filename { get; set; }
        /// <inheritdoc />
        public virtual string Filename
        {
            get
            {
                if (string.IsNullOrEmpty(_filename))
                    _filename = $"observation_validations.hpp";
                return _filename;
            }
            set { _filename = value; }
        }
    }
}
