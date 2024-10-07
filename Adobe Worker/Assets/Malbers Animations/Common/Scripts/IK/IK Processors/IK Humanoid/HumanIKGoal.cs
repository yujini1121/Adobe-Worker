using UnityEngine;

namespace MalbersAnimations.IK
{
    [System.Serializable]
    [AddTypeMenu("Humanoid/IK Goal")]
    public class HumanIKGoal : IKProcessor
    {
        public override bool RequireTargets => true;
        [Tooltip("Target to to lock any of the limbs ")]
        public AvatarIKGoal goal;
        public bool position = true;
        public bool rotation = true;

        [Tooltip("Min and Max Distance to the Goal to modify the weight. Id the distance is lower than the Min the weight is 1. If is greater than the max then the weight is zero")]
        public RangedFloat Distance = new();
        public bool gizmos = true;

        public override void Start(IKSet set, Animator animator, int index)
        {
            //Cache the RootBone
            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    set.Var[index].RootBone = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                    break;
                case AvatarIKGoal.RightFoot:
                    set.Var[index].RootBone = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                    break;
                case AvatarIKGoal.LeftHand:
                    set.Var[index].RootBone = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                    break;
                case AvatarIKGoal.RightHand:
                    set.Var[index].RootBone = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                    break;
                default:
                    break;
            }
        }

        public override void OnAnimatorIK(IKSet set, Animator animator, int index, float weight)
        {
            var Target = set.Targets[TargetIndex];

            //Check Max and Min Distance if is greater than Zero
            if (Distance.Min != 0 && Distance.Max != 0)
            {
                var RootBone = set.Var[index].RootBone; //Get the Local RootBone

                var DistanceFromRoot = Vector3.Distance(RootBone.position, Target.position);
                weight *= DistanceFromRoot.CalculateRangeWeight(Distance.Min, Distance.Max);

                if (gizmos)
                {
                    var dir = (Target.position - RootBone.position).normalized;
                    MDebug.DrawRay(RootBone.position, dir * Distance.Max, Color.gray);
                    MDebug.DrawRay(RootBone.position, dir * Distance.Min, Color.green);
                }
            }

            if (position)
            {
                animator.SetIKPositionWeight(goal, weight);
                animator.SetIKPosition(goal, Target.position);
            }
            if (rotation)
            {
                animator.SetIKRotationWeight(goal, weight);
                animator.SetIKRotation(goal, Target.rotation);
            }
        }

        //public override void CheckForNullReferences(IKSet IKSet, Animator anim)
        //{
        //    if (IKSet.Targets.Length < TargetIndex)
        //    {
        //        Debug.LogError($"The IK Set <B>[{IKSet.name}]</B> has no Transform set on the [Targets] array - Index {TargetIndex}." +
        //            $" <B>[IK Processor: {name}]</B> Needs an a value in Index {TargetIndex}." +
        //            $" Please add a reference for that index in the [Targets] array", anim);
        //        // IKSet.active = false;
        //    }
        //}

        public override void Validate(IKSet set, Animator animator, int index)
        {
            if (set.Targets.Length < TargetIndex)
            {
                Debug.LogError($"The IK Set <B>[{set.name}]</B> has no Transform set on the [Targets] array - Index {TargetIndex}." +
                    $" <B>[IK Processor: {name}]</B> Needs an a value in Index [{TargetIndex}]." +
                    $" Please add a reference for that index in the [Targets] array", animator);
            }
            else
            {
                Debug.Log($"<B>[IK Processor: {name}][HumanIK Goal]</B>  <color=yellow>[OK]</color>");
            }
        }
    }
}
