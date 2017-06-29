using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NetCoreLab.InterpreterModels
{
    class IfBlock : IResolvable
    {
        public IfCondition[] conditions;
        public string optr;
        public IResolvable action;
        public IResolvable elseAction;

        public IfBlock(string raw)
        {
            var subBlocks = Regex.Matches(raw, @"(?s)(?<=>).*?(?=<)");
            var innerBlock = Regex.Matches(raw, @"(?s)(?<=<#if).*?(?=>)")[0].Value;


            var rawAction = subBlocks[0].Value;
            if(subBlocks.Count > 1)
            {
                elseAction = TemplateParameter.FromRaw(subBlocks[1].Value);
            }

            action = TemplateParameter.FromRaw(rawAction);

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
