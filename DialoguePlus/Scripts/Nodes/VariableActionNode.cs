using UnityEngine;

namespace DialoguePlus
{
    public class VariableActionNode : DialogueNode
    {
        public string Variable { get; set; }
        public string Operation { get; set; }
        public string Modifier { get; set; }

        private object oldValue;

        public override bool IsDisplayable => false;
        public override string Info => Variable + " " + Operation + " " + Modifier;


        public override void Execute(DialogueEngine engine)
        {
            oldValue = VariableDatabase.GetVariableValue(Variable);
            object newValue = CalculateNewValue();

            VariableDatabase.SetVariableValue(Variable, newValue);
        }
        public override void Undo(DialogueEngine engine)
        {
            VariableDatabase.SetVariableValue(Variable, oldValue);
        }

        private object CalculateNewValue()
        {
            object currentValue = VariableDatabase.GetVariableValue(Variable);
            object newValue = currentValue;


            if (currentValue is int i)
            {
                int modifier = int.Parse(Modifier);

                newValue = Operation switch
                {
                    "=" => modifier,
                    "+=" => i += modifier,
                    "-=" => i -= modifier,
                    "*=" => i *= modifier,
                    "/=" => i /= modifier,
                    _ => i
                };
            }
            else if (currentValue is bool)
            {
                bool modifier = bool.Parse(Modifier);

                if (Operation == "=") newValue = modifier;
                else Debug.LogWarning($"This operation '{Operation}' is not supported for booleans!");
            }
            else if (currentValue is string)
            {
                if (Operation == "=") newValue = Modifier;
                else Debug.Log($"This operation '{Operation}' is not supported for strings!");
            }

            return newValue;
        }



    }
}
