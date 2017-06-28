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
        private Stream rawInput;
        private Stream rawTemplate;
        private Stream rawData;

        private TemplateModel template;

        private StringBuilder builder;
        private object dataItem;

        public TemplateInterpreter(Stream raw)
        {
            rawInput = PreTransform(raw);
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

        Stream PreTransform(Stream raw)
        {
            return raw;
        }

        public void ReadRawInput()
        {
            using (var sr = new StreamReader(rawInput))
            {
                var currentLine = sr.ReadLine();
                while (currentLine != null)
                {
                    if (currentLine.StartsWith("#Template"))
                    {
                        rawTemplate = new MemoryStream();
                        var writer = new StreamWriter(rawTemplate);
                        
                            while (!currentLine.StartsWith("#end"))
                            {
                                currentLine = sr.ReadLine();
                                writer.WriteLine(currentLine);
                            }

                        writer.WriteLine(currentLine);
                        writer.Flush();
                    }
                    else if (currentLine.StartsWith("#Data Structure"))
                    {
                        rawData = new MemoryStream();
                        var writer = new StreamWriter(rawData);
                        
                        while (!currentLine.StartsWith("#end"))
                        {
                            currentLine = sr.ReadLine();
                            writer.WriteLine(currentLine);
                        }

                        writer.WriteLine(currentLine);
                        writer.Flush();
                        
                    }

                    currentLine = sr.ReadLine();
                }
            }
        }

        public void ReadRawTemplate()
        {
            template = new TemplateModel(rawTemplate);
        }

        public string ApplyTemplate()
        {
            var sr = new StreamReader(rawInput);
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
            Console.WriteLine(ifBlock.ResolveTemplate(dataItem));
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
