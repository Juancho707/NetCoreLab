using Newtonsoft.Json.Linq;

namespace NetCoreLab.InterpreterModels
{
    /// <summary>
    /// File name template generator request model.
    /// </summary>
    public class FileNameRequest
    {
        /// <summary>
        /// Template string.
        /// </summary>
        public string template;

        /// <summary>
        /// Data context.
        /// </summary>
        public JObject dataStructure;
    }
}
