using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.IK
{
    [System.Serializable]
    [AddTypeMenu("Humanoid/IK Goal Offset")]
    public class HumanIKGoalOffset : IKProcessor
    {
        public override bool RequireTargets => false;
        [Tooltip("Target to to lock any of the limbs ")]
        public AvatarIKGoal goal = AvatarIKGoal.RightHand;


        public bool RelativeToRoot = true;

        [Hide("RelativeToRoot", true), SearcheableEnum]
        public HumanBodyBones RelativeTo = HumanBodyBones.UpperChest;

        public Vector3 GoalOffset;
        public Vector3 GoalRotation;


        public bool FixHint = true;

        [Hide("FixHint", false)]
        public Vector3 HintOffset;


        public bool position = true;
        public bool rotation = true;

        public bool gizmos = true;
        // public bool handles = false;

        [Tooltip("States that the IK Set will be active")]
        public List<StateID> states;
        private List<int> statesID;

        [Tooltip("Stances that the IK Set will be active")]
        public List<StanceID> stances;
        private List<int> stancesID;

        [Tooltip("Lerp the Goal Offset")]
        public float lerp = 10f;

        public override void Start(IKSet set, Animator animator, int index)
        {
            //Cache the RootBone
            set.Var[index].RootBone = RelativeToRoot ? animator.transform : animator.GetBoneTransform(RelativeTo);


            if (states != null)
            {
                statesID = new List<int>();
                foreach (var state in states)
                {
                    statesID.Add(state.ID);
                }
            }

            if (stances != null)
            {
                stancesID = new List<int>();
                foreach (var stance in stances)
                {
                    stancesID.Add(stance.ID);
                }
            }

            set.sharedVars.TryAdd($"GoalOffset{index}", GoalOffset); //Store the Goal Offset in the Shared Variables
            set.sharedVars.TryAdd($"HintOffset{index}", HintOffset); //Store the Hint Offset in the Shared Variables
            set.sharedVars.TryAdd($"GoalRotation{index}", GoalRotation); //Store the Goal Rotation in the Shared Variables
        }


        private AvatarIKHint GetHint()
        {
            return goal switch
            {
                AvatarIKGoal.LeftFoot => AvatarIKHint.LeftKnee,
                AvatarIKGoal.RightFoot => AvatarIKHint.RightKnee,
                AvatarIKGoal.LeftHand => AvatarIKHint.LeftElbow,
                AvatarIKGoal.RightHand => AvatarIKHint.RightElbow,
                _ => AvatarIKHint.LeftKnee,
            };
        }

        private Transform GetGoal(Animator ani)
        {
            return goal switch
            {
                AvatarIKGoal.LeftFoot => ani.GetBoneTransform(HumanBodyBones.LeftFoot),
                AvatarIKGoal.RightFoot => ani.GetBoneTransform(HumanBodyBones.RightFoot),
                AvatarIKGoal.LeftHand => ani.GetBoneTransform(HumanBodyBones.LeftHand),
                AvatarIKGoal.RightHand => ani.GetBoneTransform(HumanBodyBones.RightHand),
                _ => ani.GetBoneTransform(HumanBodyBones.LeftFoot),
            };
        }


        public override void OnAnimatorIK(IKSet set, Animator animator, int index, float weight)
        {

            var CheckStates = true;
            var CheckStances = true;


            if (statesID != null && statesID.Count > 0)
            {
                CheckStates = statesID.Contains(set.CurrentState);
                weight = CheckStates ? weight : 0;
            }

            if (stancesID != null && stancesID.Count > 0)
            {
                CheckStances = stancesID.Contains(set.CurrentStance);
                weight = CheckStances ? weight : 0;
            }

            if (weight == 0) return;

            var root = set.Var[index].RootBone;

            var SharedGoalOffset = set.sharedVars.Get<Vector3>($"GoalOffset{index}");
            var SharedHintOffset = set.sharedVars.Get<Vector3>($"HintOffset{index}");
            var SharedGoalRotation = set.sharedVars.Get<Vector3>($"GoalRotation{index}");

            set.sharedVars[$"GoalOffset{index}"] = Vector3.Lerp(SharedGoalOffset, GoalOffset, lerp * Time.deltaTime);
            set.sharedVars[$"HintOffset{index}"] = Vector3.Lerp(SharedHintOffset, HintOffset, lerp * Time.deltaTime);
            set.sharedVars[$"GoalRotation{index}"] = Vector3.Lerp(SharedGoalRotation, GoalRotation, lerp * Time.deltaTime);


            var GoalPosition = root.TransformPoint(SharedGoalOffset);
            var HintPosition = root.TransformPoint(SharedHintOffset);

            if (position)
            {
                animator.SetIKPositionWeight(goal, weight);
                animator.SetIKPosition(goal, GoalPosition);
                MDebug.DrawWireSphere(GoalPosition, Color.green, 0.05f);

                if (FixHint)
                {
                    animator.SetIKHintPositionWeight(GetHint(), weight);
                    animator.SetIKHintPosition(GetHint(), HintPosition);

                    MDebug.DrawWireSphere(HintPosition, Color.green, 0.05f);
                }
            }
            if (rotation)
            {
                animator.SetIKRotationWeight(goal, weight);
                animator.SetIKRotation(goal, root.rotation * Quaternion.Euler(SharedGoalRotation));
            }
        }


        public override void OnDrawGizmos(IKSet IKSet, Animator anim, float weight)
        {
            if (gizmos)
            {
                var RootBone = RelativeToRoot ? anim.transform : anim.GetBoneTransform(RelativeTo);

                var goalTransform = GetGoal(anim);
                if (RootBone == null) return;

                var oldMatrix = Gizmos.matrix;
                Gizmos.matrix = RootBone.localToWorldMatrix;

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Vector3.zero + GoalOffset, 0.05f);
                Gizmos.DrawSphere(Vector3.zero + GoalOffset, 0.05f);

                Gizmos.matrix = oldMatrix;

                var off = RootBone.TransformPoint(GoalOffset);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(goalTransform.position, off);
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(RootBone.position, off);

                Gizmos.color = Color.green;
                Gizmos.DrawRay(off, RootBone.rotation * Quaternion.Euler(GoalRotation) * Vector3.up * 0.2f);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(off, RootBone.rotation * Quaternion.Euler(GoalRotation) * Vector3.right * 0.2f);
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(off, RootBone.rotation * Quaternion.Euler(GoalRotation) * Vector3.forward * 0.2f);

                if (FixHint)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(RootBone.TransformPoint(HintOffset), 0.05f);
                }
            }
        }

#if UNITY_EDITOR
        internal override void OnSceneGUI(IKSet set, Animator animator, UnityEngine.Object Target, int index)
        {
            if (gizmos)
            {
                var RootBone = RelativeToRoot ? animator.transform : animator.GetBoneTransform(RelativeTo);
                var goalTransform = GetGoal(animator);

                if (Tools.current == Tool.Move)
                {
                    using (var cc = new EditorGUI.ChangeCheckScope())
                    {
                        Vector3 piv = RootBone.TransformPoint(GoalOffset);
                        Vector3 NewPivPosition = Handles.PositionHandle(piv, Quaternion.identity);

                        if (cc.changed)
                        {
                            Undo.RecordObject(Target, "Change Pos Goal");
                            GoalOffset = RootBone.InverseTransformPoint(NewPivPosition);
                            EditorUtility.SetDirty(Target);
                        }
                    }

                    using (var dd = new EditorGUI.ChangeCheckScope())
                    {
                        Vector3 HandlePos = RootBone.TransformPoint(HintOffset);
                        Vector3 NewHintPos = Handles.PositionHandle(HandlePos, Quaternion.identity);

                        if (dd.changed)
                        {
                            Undo.RecordObject(Target, "Change Pos Hint");
                            HintOffset = RootBone.InverseTransformPoint(NewHintPos);
                            EditorUtility.SetDirty(Target);
                        }
                    }
                }
                else if (Tools.current == Tool.Rotate)
                {
                    using (var cc = new EditorGUI.ChangeCheckScope())
                    {
                        Vector3 Pos = RootBone.TransformPoint(GoalOffset);

                        Quaternion oldQ = RootBone.rotation * Quaternion.Euler(GoalRotation);

                        Quaternion NewRotation = Handles.RotationHandle(oldQ, Pos);

                        NewRotation = Quaternion.Inverse(RootBone.rotation) * NewRotation;

                        if (cc.changed)
                        {
                            Undo.RecordObject(Target, "Change Rot");
                            GoalRotation = NewRotation.eulerAngles;
                            EditorUtility.SetDirty(Target);
                        }
                    }
                }
            }
        }

#endif
        public override void Validate(IKSet set, Animator animator, int index)
        {
            Debug.Log($"<B>[IK Processor: {name}][HumanIK Goal]</B>  <color=yellow>[OK]</color>");
        }
    }
}
