using UnityEngine;
  
namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_HasComponentGameObject : MCondition
    {
        public override string DisplayName => "Unity/Has Component [GameObject]";

        [Tooltip("Target to check for the condition ")]
        [RequiredField] public GameObject Target;
        [Tooltip("Type of Script-Component attached to the GameObject")]
        public string componentName;
        

        public override bool _Evaluate()
        {
            return Target != null && Target.GetComponent(componentName)!= null;
        }

        protected override void _SetTarget(Object target) => VerifyTarget(target, ref Target);

        private void Reset() => Name = "Does the GameObject has this component?";
    }
}
