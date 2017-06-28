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
            raw = raw.Replace(GetComparer(raw), "|");
            var parts = raw.Split('|');
            parameterPath = parts[0];
            value = parts[1].Replace(@"""", string.Empty);            
        }
        
        string GetComparer(string raw)
        {
            if (raw.Contains("!="))
            {
                comparer = ConditionalComparer.Diff;
                return "!=";
            }
            else if (raw.Contains(">"))
            {
                comparer = ConditionalComparer.Gr;
                return ">";
            }
            //else if (raw.Contains("=="))
            //{
            //    comparer = ConditionalComparer.Eq;
            //    return "==";
            //}
            //else if (raw.Contains("=="))
            //{
            //    comparer = ConditionalComparer.Eq;
            //    return "==";
            //}
            //else if (raw.Contains("=="))
            //{
            //    comparer = ConditionalComparer.Eq;
            //    return "==";
            //}

            comparer = ConditionalComparer.Eq;
            return "==";
        }

        public bool Evaluate(object target)
        {
            switch(comparer)
            {
                case ConditionalComparer.Eq:
                    return AnonymExplorer.GetValueFromAnonym(target, parameterPath) == value;
                case ConditionalComparer.Diff:
                    return AnonymExplorer.GetValueFromAnonym(target, parameterPath) != value;
            }

            return true;
        }
    }
}
