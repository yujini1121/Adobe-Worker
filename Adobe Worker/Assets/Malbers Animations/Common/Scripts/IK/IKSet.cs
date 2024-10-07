using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace MalbersAnimations.IK
{
    [System.Serializable]
    public class IKSet
    {
        /// <summary> Local IKProcessor Variables for the IK Sets </summary>
        public struct IKVars
        {
            /// <summary> Store a Transform for a rootBone</summary>
            public Transform RootBone;
            public Transform Bone;

            public Dictionary<int, int> ints;
            public Dictionary<int, float> floats;
            public Dictionary<int, Vector3> vectors;
            public Dictionary<int, Transform> transforms;
            public Dictionary<int, Quaternion> rotations;

            public IKVars(string name)
            {
                ints = new();
                floats = new();
                vectors = new();
                transforms = new();
                rotations = new();
                RootBone = Bone = null;
            }
        }

        public string name;

        public bool active = true;

        [Tooltip("Smoothly Enable the IK Set Weight")]
        [Min(0)] public float EnableTime = 0.0f;

        [Tooltip("Smoothly Disable the IK Set Weight")]
        [Min(0)] public float DisableTime = 0.25f;

        [Range(0f, 1f)]
        [Tooltip("Weight of the IK Set")]
        public float weight = 1f;

        [Tooltip("Use this Targets array to assign IK goals, Targets or Transform References to the IK Processors")]
        public TransformReference[] Targets;

        [Tooltip("Reference for the Aimer Component to get Directions")]
        public Aim aimer;

        [SerializeReference]
        public List<IKProcessor> IKProcesors;


        [SerializeReference, SubclassSelector]
        public List<WeightProcessor> weightProcessors;

        [HideInInspector] public int SelectedIKProcessor; // Inspector Only!!

        /// <summary> Local Variables used on the IK Processors</summary>
        public IKVars[] Var;

        [Tooltip("States that the IK Set will be active")]
        public List<StateID> states;
        private List<int> statesID;

        [Tooltip("Stances that the IK Set will be active")]
        public List<StanceID> stances;
        private List<int> stancesID;

        [Tooltip("Lerp the Weight of the IK Set. Set the value to zero to ignore lerping")]
        [Min(0)] public float LerpWeight = 5f;

        public FloatEvent OnWeightChanged = new();
        public UnityEvent OnSetEnable = new();
        public UnityEvent OnSetDisable = new();

        public float FinalWeight { get; private set; }

        public int CurrentState { get; private set; }

        public int CurrentStance { get; private set; }

        /// <summary> Reference for the Animator component </summary>
        public Animator Animator { get; set; }

        /// <summary>  Reference for Setting the Owner to do Coroutine stuffs</summary>
        public MonoBehaviour Owner { get; set; }

        public AnimationCurve EnterLerp = new(new Keyframe(0, 0), new Keyframe(1, 1));
        public AnimationCurve ExitLerp = new(new Keyframe(0, 0), new Keyframe(1, 1));


        /// <summary> Shader Variables between IK Processors</summary>
        public Dictionary<string, object> sharedVars = new();

        //[Tooltip("Use a Local IK Processor when true, or use a global (Scriptable  Variable)")]
        //public bool local = true;

        //public IKProccesorProfile GlobalVar;


        public List<IKProcessor> Processors => IKProcesors;
        //public List<IKProcessor> Processors
        //{
        //    get
        //    {
        //        if (local || GlobalVar == null) return IKProcesors;
        //        else return GlobalVar.IKProcesors;
        //    }
        //}


        public void Initialize(Animator anim, HashSet<int> animParams)
        {
            Targets ??= new TransformReference[0]; //Make sure the Target list is not Null
            Animator = anim; //Cache the Animator
            Var = new IKVars[Processors.Count]; //Initialize the IKVars

            FinalWeight = weight; //Initialize the Cache Weight from the current weight

            for (int i = 0; i < Processors.Count; i++)
            {
                var link = Processors[i];

                Var[i] = new IKVars(link.name); //Initialize the IKVars for each IKProcessor

                if (link != null)
                {
                    link.AnimParameterHash = TryAnimParameter(link.AnimParameter, animParams); //Store if the anim Param can be found.
                    link.Start(this, anim, i);
                }
            }


            //Store the ID of the States and Stances in a List of Integers, so the Methods can read it instead the scriptable IDs
            if (states != null && states.Count > 0)
            {
                statesID = new List<int>(states.Count);
                foreach (var state in states)
                {
                    statesID.Add(state.ID);
                }
            }

            if (stances != null && stances.Count > 0)
            {
                stancesID = new List<int>(stances.Count);
                foreach (var stance in stances)
                {
                    stancesID.Add(stance.ID);
                }
            }
        }


        //Send 0 if the Animator does not contain
        public int TryAnimParameter(string param, HashSet<int> animParams)
        {
            var AnimHash = Animator.StringToHash(param);
            return animParams.Contains(AnimHash) ? AnimHash : 0;
        }

        public void OnAnimatorIK(Animator anim, float GlobalWeight)
        {
            float preWeight = weight * GlobalWeight;

            if (weightProcessors != null)
            {
                for (int i = 0; i < weightProcessors.Count; i++)
                    if (weightProcessors[i].Active)
                        preWeight = weightProcessors[i].Process(this, preWeight);
            }

            if (active)
            {
                for (int i = 0; i < Processors.Count; i++)
                {
                    var processor = Processors[i];

                    if (processor.Active)
                    {
                        var IKProcessorWeight = FinalWeight * processor.Weight;

                        if (IKProcessorWeight > 0)
                            processor.OnAnimatorIK(this, anim, i, IKProcessorWeight); // Process first the IK Logic after the weight

                        GetFinalWeight(anim, preWeight, processor);
                    }
                }
            }
        }

        public void LateUpdate(Animator anim, float GlobalWeight)
        {
            float preWeight = weight * GlobalWeight;

            if (weightProcessors != null)
            {
                for (int i = 0; i < weightProcessors.Count; i++)
                    if (weightProcessors[i].Active)
                        preWeight = weightProcessors[i].Process(this, preWeight);
            }

            if (active)
            {
                for (int i = 0; i < Processors.Count; i++)
                {
                    var processor = Processors[i];
                    if (processor.Active)
                    {
                        var IKProcessorWeight = FinalWeight * processor.Weight;

                        if (IKProcessorWeight > 0)
                            processor.LateUpdate(this, anim, i, IKProcessorWeight); // Process first the IK Logic after the weight

                        GetFinalWeight(anim, preWeight, processor);
                    }
                }
            }
        }

        private void GetFinalWeight(Animator anim, float finalWeight, IKProcessor processor)
        {
            if (FinalWeight == finalWeight) return; //If the final weight is the same as the current weight then ignore the rest

            finalWeight *= processor.GetProcessorAnimWeight(anim);

            FinalWeight = LerpWeight > 0 ? Mathf.Lerp(FinalWeight, finalWeight, Time.deltaTime * LerpWeight) : finalWeight; //Lerp when the LerpWeight is higher than 0


            if (FinalWeight > 0.9999f) FinalWeight = 1;
            else if (FinalWeight < 0.0001f) FinalWeight = 0;


            OnWeightChanged.Invoke(FinalWeight);

            //if (FinalWeight == 0)
            //{
            //    OnSetDisable.Invoke();
            //    active = false;
            //}
        }


        public virtual void Enable(bool value)
        {
            //Clear the Coroutine it one was enabled
            if (C_EnableSmooth != null) Owner.StopCoroutine(C_EnableSmooth);

            C_EnableSmooth = value ? EnableSmooth() : DisableSmooth(); //Set Enable or Disable Smooth Coroutine


            Owner.StartCoroutine(C_EnableSmooth);
        }

        public virtual void SetWeight(bool value)
        {
            if (!active) return; //Ignore if the set is not active

            weight = value ? 1 : 0;
        }

        private IEnumerator C_EnableSmooth;

        private IEnumerator EnableSmooth()
        {
            var elapsedTime = 0f;
            var startWeight = weight;
            active = true;

            OnSetEnable.Invoke();

            while (weight != 1 && (EnableTime > 0) && (elapsedTime <= EnableTime))
            {
                weight = Mathf.Lerp(startWeight, 1, EnterLerp.Evaluate(elapsedTime / EnableTime));
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            weight = 1;

            yield return null;
        }


        private IEnumerator DisableSmooth()
        {
            var elapsedTime = 0f;
            var startWeight = weight;

            while (weight != 0 && (DisableTime > 0) && (elapsedTime <= DisableTime))
            {
                weight = Mathf.Lerp(startWeight, 0, ExitLerp.Evaluate(elapsedTime / DisableTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            weight = 0;
            yield return null;

            OnSetDisable.Invoke();
            active = false;
        }

        public virtual void SetTarget(Transform target, int index)
        {
            Targets[index] = target;
        }

        public virtual void ClearTarget(int index)
        {
            Targets[index] = null;
        }

        public virtual void ClearAllTargets()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                Targets[i] = null;
            }
        }

        public virtual void SetTargets(Transform[] newTargets)
        {
            Targets = new TransformReference[newTargets.Length];

            for (int i = 0; i < Targets.Length; i++)
            {
                if (newTargets[i] != null)
                    Targets[i] = new(newTargets[i]);
            }
        }


        internal void Processor_SetEnable(string processor, bool value)
        {
            for (int i = 0; i < Processors.Count; i++)
            {
                if (Processors[i].name.Contains(processor))
                {
                    Processors[i].Active = value;
                    break;
                }
            }
        }



        public void OnModeStart(int Mode, int Ability)
        {
            //Need to be implemented yet
        }

        public void OnModeEnd(int Mode, int Ability)
        {
            //Need to be implemented yet
        }

        public void OnStanceChange(int stance)
        {
            CurrentStance = stance;
            bool ValidState;
            bool ValidStance;

            //Check if the State is on the list of states to enable the IK Set
            if (stancesID != null && stancesID.Count > 0)
            {
                ValidStance = (stancesID.Contains(stance));
            }
            else
            {
                return; //Meaning that you can ignore the Valid Stance since there is none
            }

            //Check if the State is on the list of states to enable the IK Set
            if (statesID != null && statesID.Count > 0)
            {
                ValidState = (statesID.Contains(CurrentState));
            }
            else
            {
                ValidState = true; //Meaning that you can ignore the Valid State since there is none
            }

            SetWeight(ValidState && ValidStance);
        }

        public void OnStateChange(int state)
        {
            CurrentState = state;

            bool ValidStance;
            bool ValidState;
            //Check if the State is on the list of states to enable the IK Set
            if (statesID != null && statesID.Count > 0)
            {
                ValidState = statesID.Contains(state);
            }
            else
            {
                return; //Meaning that you can ignore the Valid State since there is none
            }

            //Check if the State is on the list of states to enable the IK Set
            if (stancesID != null && stancesID.Count > 0)
            {
                ValidStance = (stancesID.Contains(CurrentStance));
            }
            else
            {
                ValidStance = true; //Meaning that you can ignore the Valid Stance since there is none
            }
            SetWeight(ValidState && ValidStance);
        }

        internal void Verify(Animator animator)
        {
            for (int i = 0; i < Processors.Count; i++)
            {
                Processors[i].Validate(this, animator, i);
            }
        }


        ///// <summary> Process the Animation Curve in case there's one </summary>
        //protected virtual float GetSetAnimWeight(Animator animator)
        //{
        //    var result = AnimParameterHash != 0 ? animator.GetFloat(AnimParameterHash) : 1;

        //    if (InvertAnimParameter) result = 1 - result;

        //    return result;
        //}
    }
}