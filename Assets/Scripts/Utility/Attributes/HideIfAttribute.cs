using UnityEngine;

namespace Scripts.Utility
{
    public class HideIfAttribute : PropertyAttribute
    {
        public string ConditionFieldName { get; }

        public HideIfAttribute(string conditionFieldName)
        {
            ConditionFieldName = conditionFieldName;
        }
    }
}
