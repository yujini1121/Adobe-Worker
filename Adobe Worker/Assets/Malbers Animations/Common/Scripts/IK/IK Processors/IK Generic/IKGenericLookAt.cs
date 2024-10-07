using System;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
#endif

namespace MalbersAnimations.IK
{
    [Serializable]
    [AddTypeMenu("Generic/LookAt")]
    public class IKGenericLookAt : IKProcessor
    {
        public override bool RequireTargets => true;

        public enum UpVectorType { VectorUp, Local, Global }

        public List<GenericIKOffset> Bones = new();

        private Quaternion[] ChildRotations;

        public override void LateUpdate(IKSet IKSet, Animator anim, int index, float weight)
        {
            Quaternion TargetRotation = Quaternion.identity;

            foreach (var bn in Bones)
            {
                var FinalWeight = weight * bn.Weight;
                if (FinalWeight <= 0) continue; //Do nothing if the weight is zero

                var Bone = IKSet.Targets[bn.BoneIndex];

                //Store the  bone's Child Rotation 
                if (bn.KeepChildRot)
                {
                    ChildRotations = new Quaternion[Bone.Value.childCount];
                    for (int i = 0; i < ChildRotations.Length; i++)
                        ChildRotations[i] = Bone.Value.GetChild(i).rotation;
                }


                switch (bn.IK)
                {
                    case IKGenerigType.LookAt:

                        var direction = IKSet.aimer.AimDirection;
                        var angle = Vector3.Angle(anim.transform.forward, direction);

                        if (bn.LookAtLimit.maxValue != 0 && bn.LookAtLimit.minValue != 0) //Check the Limit in case there is a limit
                            FinalWeight *= angle.CalculateRangeWeight(bn.LookAtLimit.minValue, bn.LookAtLimit.maxValue);

                        if (bn.Gizmos) MDebug.DrawRay(Bone.Value.transform.position, direction.normalized, Color.Lerp(Color.black, Color.green, FinalWeight));

                        if (FinalWeight == 0) continue; //Do nothing if the weight is zero


                        TargetRotation = Quaternion.LookRotation(IKSet.aimer.RawAimDirection, bn.UpVector) * Quaternion.Euler(bn.Offset);
                        break;
                    case IKGenerigType.AdditiveOffset:
                        TargetRotation = Bone.rotation * Quaternion.Euler(bn.Offset);
                        break;
                    case IKGenerigType.RotationOverride:
                        TargetRotation = Quaternion.Euler(bn.Offset);
                        break;
                    default:
                        break;
                }


                Bone.Value.rotation = Quaternion.Lerp(Bone.rotation, TargetRotation, FinalWeight);

                //Store the bone's Child Rotation
                if (bn.KeepChildRot)
                {
                    for (int i = 0; i < ChildRotations.Length; i++)
                    {
                        Bone.Value.GetChild(i).rotation = ChildRotations[i];
                    }
                }
            }
        }

        public override void Validate(IKSet set, Animator animator, int index)
        {
            if (set.Targets.Length == 0)
            {
                Debug.LogWarning($"There's no Targets on the IK Set. Generic IK needs a Target on on Index [{TargetIndex}]");
            }
            if (set.Targets.Length <= TargetIndex)
            {
                Debug.LogWarning($"The Target Index [{TargetIndex}] is out of range on the IK Set. The IK Set has only {set.Targets.Length} targets");
            }
            else
            {
                Debug.Log($"<B>[IK Processor: {name}][IK Generic Look At]</B>  <color=yellow>[OK]</color>");
            }
        }

        //        public override void OnDrawGizmos(IKSet IKSet, Animator anim, float weight)
        //        {
        //#if UNITY_EDITOR && MALBERS_DEBUG
        //            // bool AppIsPlaying = Application.isPlaying;
        //            if (anim == null) return;

        //            foreach (var bn in Bones)
        //            {
        //                if (IKSet.Targets != null && IKSet.Targets.Length > 0 && IKSet.Targets.Length > bn.BoneIndex)
        //                {
        //                    var Bone = IKSet.Targets[bn.BoneIndex];

        //                    if (Bone == null || !bn.Gizmos) continue;

        //                    var FinalWeight = weight * bn.Weight * GetProcessorAnimWeight(anim);

        //                    Handles.color = new Color(0, 1, 0, 0.1f);
        //                    Handles.DrawSolidArc(Bone.position, bn.UpVector,
        //                        Quaternion.Euler(0, -bn.LookAtLimit.minValue, 0) * anim.transform.forward, bn.LookAtLimit.minValue * 2, 1);



        //                    Handles.color = Color.green;
        //                    Handles.DrawWireArc(Bone.position,
        //                        bn.UpVector, Quaternion.Euler(0, -bn.LookAtLimit.minValue, 0) * anim.transform.forward, bn.LookAtLimit.minValue * 2, 1);


        //                    Handles.color = new Color(0, 0.3f, 0, 0.2f);
        //                    var Maxlimit = (bn.LookAtLimit.minValue - bn.LookAtLimit.maxValue);

        //                    Handles.DrawSolidArc(Bone.position,
        //                        bn.UpVector, Quaternion.Euler(0, -(bn.LookAtLimit.minValue), 0) * anim.transform.forward, (Maxlimit), 1);

        //                    Handles.DrawSolidArc(Bone.position,
        //                        bn.UpVector, Quaternion.Euler(0, (bn.LookAtLimit.minValue), 0) * anim.transform.forward, -(Maxlimit), 1);


        //                    Handles.color = Color.black;

        //                    Handles.DrawWireArc(Bone.position,
        //                        bn.UpVector, Quaternion.Euler(0, -(bn.LookAtLimit.minValue), 0) * anim.transform.forward, (Maxlimit), 1);

        //                    Handles.DrawWireArc(Bone.position,
        //                        bn.UpVector, Quaternion.Euler(0, (bn.LookAtLimit.minValue), 0) * anim.transform.forward, -(Maxlimit), 1);

        //                }
        //            }
        //#endif
        //        }
    }



    public enum IKGenerigLookAt
    {
        LookAt,
        [InspectorName("Local Rotation Additive")]
        AdditiveOffset,
        [InspectorName("Local Rotation Override")]
        RotationOverride
    }
}
