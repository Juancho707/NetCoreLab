using NetCoreLab.InterpreterModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetCoreLab
{
    /// <summary>
    /// Template interpreter for file names.
    /// </summary>
    public class FileNameTemplateInterpreter
    {
        private string rawInput;
        private TemplateModel template;        
        private JObject dataItem;

        private void ReadRawInput()
        {
            var extracted = JsonConvert.DeserializeObject<FileNameRequest>(rawInput);
            template = new TemplateModel(extracted.template);
            dataItem = extracted.dataStructure;
        }

        /// <summary>
        /// Creates a new instance of the File Name Template Interpreter class.
        /// </summary>
        /// <param name="raw">Raw data.</param>
        public FileNameTemplateInterpreter(string raw)
        {
            rawInput = raw;
            ReadRawInput();
        }        

        /// <summary>
        /// Resolves the template.
        /// </summary>
        /// <returns>Formatted template string.</returns>
        public string ResolveTemplate()
        {            
            return template.ResolveTemplate(dataItem);            
        }        
    }
}
