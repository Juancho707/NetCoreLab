using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetCoreLab.InterpreterModels
{
    class AnonymExplorer
    {
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

        static JObject NavigateNode(JObject target, string fieldName)
        {
            return target[fieldName] as JObject;
        }

        static JValue GetValue(JObject target, string fieldName)
        {
            return target[fieldName] as JValue;
        }

        public static object CreateFromRaw(Stream raw)
        {
            var reader = new StreamReader(raw);
            raw.Position = 0;
            //var extracted = reader.ReadToEnd().Replace("\t", string.Empty);
            return JsonConvert.DeserializeObject(reader.ReadToEnd());
        }
    }
}
