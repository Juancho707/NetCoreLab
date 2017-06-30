using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreLab.InterpreterModels
{
    /// <summary>
    /// Conditional block object from a template.
    /// </summary>
    internal class IfBlock : IResolvable
    {
        private IfCondition[] conditions;
        private string optr;
        private TemplateModel action;
        private TemplateModel elseAction;

        /// <summary>
        /// The concrete form of the placeholder representation.
        /// </summary>
        public string ConcreteForm { get; set; }

        /// <summary>
        /// Creates a new instance of the IfBlock class.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        public IfBlock(string raw)
        {
            ConcreteForm = RegExHelper.TrimUntilChar(RegExHelper.ClearStringFormatting(raw), '$') + "}";
            var subBlocks = RegExHelper.GetContainedString(raw, ">", "<");
            var innerBlock = RegExHelper.GetContainedString(raw, "<#if", ">")[0].Value;


            var rawAction = subBlocks[0].Value;
            if(subBlocks.Count > 1)
            {
                elseAction = new TemplateModel(subBlocks[1].Value);
            }

            action = new TemplateModel(rawAction);

            innerBlock = innerBlock.Replace(" ", string.Empty);

            if(innerBlock.Contains("&&"))
            {
                optr = "&&";
            }
            else if(innerBlock.Contains("||"))
            {
                optr = "||";
            }

            if (string.IsNullOrEmpty(optr))
            {
                conditions = new[] { new IfCondition(innerBlock) };
            }
            else
            {
                var rawConditions = innerBlock.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                var condList = new List<IfCondition>();

                foreach(var c in rawConditions)
                {
                    condList.Add(new IfCondition(c));
                }

                conditions = condList.ToArray();
            }
        }


        /// <summary>
        /// Resolve placeholder value and format.
        /// </summary>
        /// <param name="target">Target data context.</param>
        /// <returns>Formatted string.</returns>
        public string ResolveTemplate(object target)
        {
            bool result = false;

            if(optr == null)
            {
                result = conditions.First().Evaluate(target);
            }
            else if (optr.Equals("||"))
            {
                result = conditions.Select(x => x.Evaluate(target)).Any(x => x == true);
                
            }
            else
            {
                result = conditions.Select(x => x.Evaluate(target)).All(x => x == true);
            }

            return result ? action.ResolveTemplate(target) : elseAction.ResolveTemplate(target);
        }
    }
}
