using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetCoreLab.InterpreterModels
{
    class TemplateModel
    {
        public List<IResolvable> TemplateSteps;

        Stream rawTemplate;

        public TemplateModel(Stream raw)
        {
            rawTemplate = raw;
            TemplateSteps = new List<IResolvable>();
            rawTemplate.Position = 0;

            var sr = new StreamReader(rawTemplate);
            var currentLine = sr.ReadLine();
            while (currentLine != null)
            {
                if (currentLine.StartsWith(@"<#if"))
                {
                    var rawIfStatement = new MemoryStream();
                    var writer = new StreamWriter(rawIfStatement);

                    while (!currentLine.StartsWith(@"</#if"))
                    {
                        writer.WriteLine(currentLine);
                        currentLine = sr.ReadLine();                        
                    }

                    writer.WriteLine(currentLine);
                    writer.Flush();

                    var sr1 = new StreamReader(rawIfStatement);
                    rawIfStatement.Position = 0;

                    var ifBlock = new IfBlock(sr1.ReadToEnd());
                    TemplateSteps.Add(ifBlock);
                }
                else
                {
                    var lineParameters = currentLine.Split('$');
                    if (lineParameters.Length > 0)
                    {
                        foreach (var par in lineParameters)
                        {
                            if (par.Contains("{"))
                                TemplateSteps.Add(TemplateParameter.FromRaw(par));
                        }
                    }
                }
                currentLine = sr.ReadLine();
            }

        }

        public string ResolveTemplate(object dataItem)
        {
            var builder = new StringBuilder();

            foreach (var f in TemplateSteps)
            {
                builder.Append(f.ResolveTemplate(dataItem));

                if (f != TemplateSteps.Last())
                {
                    builder.Append("_");
                }
            }

            return builder.ToString();
        }
    }
}
