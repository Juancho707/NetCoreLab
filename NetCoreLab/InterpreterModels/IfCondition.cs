using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NetCoreLab.InterpreterModels
{
    class IfCondition
    {
        public string parameterPath;
        public string value;
        public ConditionalComparer comparer;        

        public IfCondition(string raw)
        {
            var parts = raw.Split(new[] { GetComparer(raw) }, StringSplitOptions.RemoveEmptyEntries);
            parameterPath = parts[0];
            value = parts[1].Replace(@"""", string.Empty);            
        }
        
        string GetComparer(string raw)
        {
            if (raw.Contains("=="))
            {
                comparer = ConditionalComparer.Equals;
                return "==";
            }
            else if (raw.Contains("!="))
            {
                comparer = ConditionalComparer.Different;
                return "!=";
            }
            else if (raw.Contains(">"))
            {
                comparer = ConditionalComparer.Greater;
                return ">";
            }
            else if (raw.Contains(">="))
            {
                comparer = ConditionalComparer.GreaterEquals;
                return ">=";
            }
            else if (raw.Contains("<"))
            {
                comparer = ConditionalComparer.Less;
                return "<";
            }
            else if (raw.Contains("<="))
            {
                comparer = ConditionalComparer.LessEquals;
                return "<=";
            }

            comparer = ConditionalComparer.Equals;
            return "==";
        }

        public bool Evaluate(object target)
        {
            switch(comparer)
            {
                case ConditionalComparer.Equals:
                    return AnonymExplorer.GetValueFromAnonym(target, parameterPath) == value;
                case ConditionalComparer.Different:
                    return AnonymExplorer.GetValueFromAnonym(target, parameterPath) != value;
            }

            return true;
        }
    }
}
