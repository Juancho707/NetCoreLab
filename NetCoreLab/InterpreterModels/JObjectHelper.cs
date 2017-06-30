using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace NetCoreLab.InterpreterModels
{
    /// <summary>
    /// Helper that exposes tools to work with deserialized JSON objects.
    /// </summary>
    internal class JObjectHelper
    {
        private static JObject NavigateNode(JObject target, string fieldName)
        {
            return target[fieldName] as JObject;
        }

        private static JValue GetValue(JObject target, string fieldName)
        {
            return target[fieldName] as JValue;
        }

        /// <summary>
        /// Gets a value from a generic JSON object.
        /// </summary>
        /// <param name="dataItem">JSON object.</param>
        /// <param name="path">The hierarchy path to the target field.</param>
        /// <returns>Target field value.</returns>
        public static string GetValueFromAnonym(object dataItem, string path)
        {
            var dataHierarchy = path.Split('.');
            var node = dataItem as JObject;
            JValue result = null;

            for (int i = 0; i < dataHierarchy.Length; i++)
            {
                if (i < dataHierarchy.Length - 1)
                {
                    node = NavigateNode(node, dataHierarchy[i]);
                }
                else
                {
                    result = GetValue(node, dataHierarchy[i]);
                }
            }

            return result != null ? result.Value.ToString() : null;
        }
             
        /// <summary>
        /// Deserializes an object from a data stream.
        /// </summary>
        /// <param name="raw">Raw data stream.</param>
        /// <returns>Serialized object.</returns>
        public static object CreateFromRaw(Stream raw)
        {
            var reader = new StreamReader(raw);
            raw.Position = 0;
            return JsonConvert.DeserializeObject(reader.ReadToEnd());
        }
    }
}
