using System;
using System.Collections.Generic;
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
            object node = dataItem;

                for (int i = 0; i < dataHierarchy.Length; i++)
                {
                    node = GetObjectFieldValue(node, dataHierarchy[i]);
                }

                string result = node.ToString();      

            return result;
        }

        static object GetObjectFieldValue(object target, string fieldName)
        {
            var fields = target.GetType().GetProperties();
            var fInfo = fields.First(x => x.Name.Equals(fieldName));

            return fInfo.GetValue(target);
        }
    }
}
