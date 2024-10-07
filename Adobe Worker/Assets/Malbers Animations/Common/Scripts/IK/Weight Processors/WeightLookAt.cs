using MalbersAnimations.Scriptables;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.IK
{
    /// <summary>  Process the weight by checking the Look At Angle of the Animator / </summary>
    [System.Serializable]
    [AddTypeMenu("Weight Look At")]
    public class WeightLookAt : WeightProcessor
    {
        public enum UpVectorType { VectorUp, Local, Global }

        [Tooltip("Limits the Look At from the Min to Max Value")]
        public RangedFloat LookAtLimit = new(90, 120);

        [Tooltip("Normalize the weight by this value")]
        public float normalizedBy = 1;

        public UpVectorType upVector = UpVectorType.VectorUp;
        [Hide("upVector", (int)UpVectorType.Local)]
        public Vector3 LocalUp = new(0, 1, 0);
        [Hide("upVector", (int)UpVectorType.Global)]
        [CreateScriptableAsset] public Vector3Var WorldUp;


        public float GizmoRadius = 1f;

        // private Vector3 DirVector;

        public Vector3 UpVector
        {
            get
            {
                return upVector switch
                {
                    UpVectorType.Local => LocalUp,
                    UpVectorType.Global => WorldUp != null ? (Vector3)WorldUp : Vector3.up,
                    _ => Vector3.up,
                };
            }
        }


        public override float Process(IKSet set, float weight)
        {
            var anim = set.Animator;
            var direction = set.aimer.AimDirection;//Get the Aim Direction


            var angle = Vector3.Angle(anim.transform.forward, direction);

            if (LookAtLimit.maxValue != 0 && LookAtLimit.minValue != 0) //Check the Limit in case there is a limit
                weight *= angle.CalculateRangeWeight(LookAtLimit.minValue, LookAtLimit.maxValue);

            return weight;
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos(IKSet IKSet, Animator anim)
        {
            if (anim == null || IKSet.aimer == null || IKSet.aimer.AimOrigin == null) return;

            var Bone = IKSet.aimer.AimOrigin;
            var UpVector = this.UpVector;

            Handles.color = new Color(0, 1, 0, 0.1f);
            Handles.DrawSolidArc(Bone.position, UpVector,
                Quaternion.Euler(0, -LookAtLimit.minValue, 0) * anim.transform.forward, LookAtLimit.minValue * 2, GizmoRadius);


            Handles.color = Color.green;
            Handles.DrawWireArc(Bone.position,
                UpVector, Quaternion.Euler(0, -LookAtLimit.minValue, 0) * anim.transform.forward, LookAtLimit.minValue * 2, GizmoRadius);


            Handles.color = new Color(0, 0.3f, 0, 0.2f);
            var Maxlimit = (LookAtLimit.minValue - LookAtLimit.maxValue);

            Handles.DrawSolidArc(Bone.position,
                UpVector, Quaternion.Euler(0, -(LookAtLimit.minValue), 0) * anim.transform.forward, (Maxlimit), GizmoRadius);

            Handles.DrawSolidArc(Bone.position,
                UpVector, Quaternion.Euler(0, (LookAtLimit.minValue), 0) * anim.transform.forward, -(Maxlimit), GizmoRadius);


            Handles.color = Color.black;

            Handles.DrawWireArc(Bone.position,
                UpVector, Quaternion.Euler(0, -(LookAtLimit.minValue), 0) * anim.transform.forward, (Maxlimit), GizmoRadius);

            Handles.DrawWireArc(Bone.position,
                UpVector, Quaternion.Euler(0, (LookAtLimit.minValue), 0) * anim.transform.forward, -(Maxlimit), 1);
        }
#endif
    }
}

