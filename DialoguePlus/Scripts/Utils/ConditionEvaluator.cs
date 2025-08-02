using System.Text.RegularExpressions;
using UnityEngine;

namespace DialoguePlus
{
    public static class ConditionEvaluator
    {
        public static bool CheckCondition(string conditionSentence)
        {
            // Comparison operators are => '<' '>' '=' '==' '<=' '>=' '!='
            // 'if' and 'elif' is trimmed before coming here
            // definedInteger = 50, definedInteger <= 50

            if (conditionSentence == "else") return true; // No need to check for 'else'

            var match = Regex.Match(conditionSentence.Trim(), @"(\w+)\s*(==|!=|>=|<=|>|<|=)\s*(.+)$");

            if (!match.Success)
            {
                Debug.LogError($"Wrong expression for condition; '{conditionSentence}'");
                return false;
            }

            string variableName = match.Groups[1].Value.Trim();
            string op = match.Groups[2].Success ? match.Groups[2].Value.Trim() : null;
            string expectedValue = match.Groups[3].Value?.Trim();

            if (op == "=") op = "==";

            return Evaluate(variableName, op, expectedValue);
        }

        private static bool Evaluate(string variable, string op, string expectedValue)
        {
            var value = VariableDatabase.GetVariableValue(variable);

            if (value is bool b)
            {
                bool expectedV;

                if (op == null)
                {
                    return b == true;
                }
                else
                {
                    if (!bool.TryParse(expectedValue, out expectedV))
                    {
                        Debug.LogError($"Couldn't parse given value '{expectedValue}', must be 'True' or 'False'");
                        return false;
                    }
                }
                if (op == "==")
                {
                    return b == expectedV;
                }
                else if (op == "!=")
                {
                    return b != expectedV;
                }
                else
                {
                    Debug.LogError($"Operator '{op}' cannot be used for booleans!");
                }
            }
            else if (value is int i)
            {
                int target = int.Parse(expectedValue);

                return op switch
                {
                    "==" => i == target,
                    "!=" => i != target,
                    ">=" => i >= target,
                    "<=" => i <= target,
                    ">" => i > target,
                    "<" => i < target,
                    _ => false
                };
            }
            else if (value is string s)
            {
                if (op == "==")
                {
                    return s == expectedValue;
                }
                else if (op == "!=")
                {
                    return s != expectedValue;
                }
                else
                {
                    Debug.LogError($"Operator '{op}' cannot be used for comparing strings!");
                }
            }

            return false;
        }
    }
}
