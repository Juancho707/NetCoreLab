using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using NetCoreLab.InterpreterModels;

namespace NetCoreLab
{
    class TemplateInterpreter
    {
        private string rawTemplate;
        private StringBuilder builder;
        private object dataItem;

        public TemplateInterpreter(string raw)
        {
            rawTemplate = raw;
            builder = new StringBuilder();

            dataItem = new
            {
                data = new
                {
                    customer = "amazon",
                    provider = "starz",
                    deliverytype = "subtitle",
                    tag = "01_Trailer",
                    DL3 = new
                    {
                        assetdetails = new
                        {
                            nf = "dsg",
                            nfdf = "sgd"
                        }
                    },
                    language ="eng"
                }
            };
        }

        public string ApplyTemplate()
        {
            var sr = new StringReader(rawTemplate);
            var line = sr.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            var lineParameters = line.Split('$');
            foreach (var lp in lineParameters)
            {
                var processed = ProcessParameter(lp);
                if (!string.IsNullOrEmpty(processed))
                {
                    builder.Append(processed);
                    builder.Append("_");
                }
            }

            return builder.ToString();
        }

        public void GetIfStatement(string raw)
        {
            var capture = Regex.Match(raw, @"(?s)(?<=<#if).*?(?=#if>)");
            var ifBlock = new IfBlock(capture.Captures[0].ToString());
            Console.WriteLine(ifBlock.Evaluate(dataItem).Evaluate(dataItem));
        }

        string ProcessParameter(string raw)
        {
            var enclosed = Regex.Match(raw, @"(?<=\{)(.*?)(?=\})");
            if (enclosed.Captures.Count > 0)
            {
                var elements = enclosed.Captures[0].Value.Split('?');

                var field = elements[0];
                var modifier = elements.Length > 1 ? elements[1] : string.Empty;
                var dataHierarchy = field.Split('.');
                object node = dataItem;

                for (int i = 0; i < dataHierarchy.Length; i++)
                {
                    node = GetObjectFieldValue(node, dataHierarchy[i]);
                }

                string result = node.ToString();
                return ApplyModifier(result, modifier);
            }

            return null;
        }

        object GetObjectFieldValue(object target, string fieldName)
        {
            var fields = target.GetType().GetProperties();
            var fInfo = fields.First(x => x.Name.Equals(fieldName)); 

            return fInfo.GetValue(target);
        }

        string ApplyModifier(string input, string modifier)
        {
            switch(modifier)
            {
                case "cap_first":
                    var capFirst = input.ToLower().ToCharArray();
                    capFirst[0] = char.ToUpper(capFirst[0]);
                    return new string(capFirst);
                    break;
                case "upper_case":
                    return input.ToUpper();
                    break;
            }

            return input;
        }

        
    }
}
