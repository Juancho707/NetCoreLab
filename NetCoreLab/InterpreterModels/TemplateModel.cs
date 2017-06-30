using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetCoreLab.InterpreterModels
{
    /// <summary>
    /// Represents a full template entity.
    /// </summary>
    public class TemplateModel
    {
        private List<IResolvable> TemplateSteps;
        private string concreteForm;
        
        private void ExtractFromStream(Stream raw)
        {
            var rawTemplate = raw;
            TemplateSteps = new List<IResolvable>();
            rawTemplate.Position = 0;

            var sr = new StreamReader(rawTemplate);
            var currentLine = sr.ReadLine();
            while (currentLine != null)
            {
                if (currentLine.Contains(@"${<#if"))
                {
                    var rawIfStatement = new MemoryStream();
                    var writer = new StreamWriter(rawIfStatement);

                    while (!currentLine.Contains(@"</#if>"))
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
                                TemplateSteps.Add(Placeholder.FromRaw(par));
                        }
                    }
                }
                currentLine = sr.ReadLine();
            }

        }

        /// <summary>
        /// Creates a new instance of a template entity.
        /// </summary>
        /// <param name="raw">Raw source string.</param>
        public TemplateModel(string raw)
        {
            concreteForm = RegExHelper.ClearStringFormatting(raw);
            raw = RegExHelper.FormatLines(raw);            

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    sw.Write(raw);
                    sw.Flush();
                    ExtractFromStream(ms);
                }
            }
        }

        /// <summary>
        /// Resolves the template.
        /// </summary>
        /// <param name="dataItem">Data context.</param>
        /// <returns>Formatted template string.</returns>
        public string ResolveTemplate(object dataItem)
        {
            var format = concreteForm;

            for(int i = 0; i < TemplateSteps.Count(); i++)
            {
                var paramId = "{" + i.ToString() + "}";
                format = format.Replace(TemplateSteps[i].ConcreteForm, paramId);
            }

            var processedPlaceholders = TemplateSteps.Select(x => x.ResolveTemplate(dataItem)).ToArray();
            return string.Format(format, processedPlaceholders);
        }
    }
}
