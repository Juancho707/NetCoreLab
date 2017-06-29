using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using NetCoreLab.InterpreterModels;
using System.Dynamic;

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
        }

        Stream PreTransform(Stream raw)
        {
            return raw;
        }

        private void ReadRawInput()
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
                        currentLine = sr.ReadLine();

                        while (!currentLine.StartsWith("#end"))
                        {
                            writer.WriteLine(currentLine);
                            currentLine = sr.ReadLine();                            
                        }
                        
                        writer.Flush();
                        
                    }

                    currentLine = sr.ReadLine();
                }
            }

            ReadRawContext();
            ReadRawTemplate();
        }

        private void ReadRawContext()
        {
            dataItem = AnonymExplorer.CreateFromRaw(rawData);
        }

        private void ReadRawTemplate()
        {
            template = new TemplateModel(rawTemplate);
        }

        public string ResolveTemplate()
        {
            ReadRawInput();
            return template.ResolveTemplate(dataItem);            
        }        
    }
}
