using UnityEngine;

namespace MalbersAnimations.IK
{
    /// <summary>  Process the weight by checking the Look At Angle of the Animator / </summary>
    [System.Serializable]
    [AddTypeMenu("Weight Animation Paramameter")]
    public class WeightAnimParam : WeightProcessor
    {
        public Animator animator;

        [Tooltip("Name of the Animator Parameter to check")]
        [AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        public string Parameter;
        [Tooltip("Normalize the weight by this value")]
        public float normalizedBy = 1;

        [HideInInspector] public int AnimParamHash;


        [Tooltip("  ")]
        public bool invert = false;

        [Tooltip("Evaluate the Animation Parameter on an Animation Curve")]
        public bool evaluate = false;

        [Hide(nameof(evaluate))]
        [Tooltip("Evaluate the Animation Parameter value on this curve")]
        public AnimationCurve curve = new(new Keyframe(1, 1), new Keyframe(1, 1));

        public override float Process(IKSet set, float weight)
        {
            if (AnimParamHash == 0)
                AnimParamHash = Animator.StringToHash(Parameter);

            var animWeight = 1f;

            if (AnimParamHash != 0)
            {
                animWeight = set.Animator.GetFloat(AnimParamHash) / normalizedBy;

                if (evaluate) animWeight = curve.Evaluate(animWeight);


                if (invert) animWeight = 1 - animWeight;
            }
            return weight * animWeight;
        }
    }
}
