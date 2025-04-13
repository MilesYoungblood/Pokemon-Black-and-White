using UnityEngine;

namespace Scripts.Utility
{
    public class DisableIfAttribute : PropertyAttribute
    {
        public DisableIfAttribute(string conditionFieldName)
        {
            ConditionFieldName = conditionFieldName;
        }

        public string ConditionFieldName { get; private set; }
    }
}