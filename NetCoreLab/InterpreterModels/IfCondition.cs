using System;

namespace NetCoreLab.InterpreterModels
{
    internal class IfCondition
    {
        private string parameterPath;
        private string value;
        private ConditionalComparer comparer;

        private string GetComparer(string raw)
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

        /// <summary>
        /// Creates a new instance of the IfCondition class.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        public IfCondition(string raw)
        {
            var parts = raw.Split(new[] { GetComparer(raw) }, StringSplitOptions.RemoveEmptyEntries);
            parameterPath = parts[0];
            value = parts[1].Replace(@"""", string.Empty).Replace(@"'", string.Empty);
        }       
        
        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        /// <param name="target">Data context.</param>
        /// <returns>Evaluated conditional at the data context.</returns>
        public bool Evaluate(object target)
        {
            switch(comparer)
            {
                case ConditionalComparer.Equals:
                    return JObjectHelper.GetValueFromAnonym(target, parameterPath) == value;
                case ConditionalComparer.Different:
                    return JObjectHelper.GetValueFromAnonym(target, parameterPath) != value;
            }

            return true;
        }
    }
}
