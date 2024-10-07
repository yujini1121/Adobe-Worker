using MalbersAnimations.Scriptables;
using UnityEngine;
namespace MalbersAnimations.IK
{
    public class IKBehavior : StateMachineBehaviour
    {
        private IIKSource source;

        public StringReference IKSet = new();
        public bool OnEnter = true;
        [Hide(nameof(OnEnter))]
        public bool enable = true;

        [Space]
        public bool OnExit = false;
        [Hide(nameof(OnExit))]
        public bool m_enable = true;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            source ??= animator.GetComponent<IIKSource>();

            if (OnEnter)
                source?.Set_Weight(IKSet.Value, enable);
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (OnExit)
                source?.Set_Weight(IKSet.Value, m_enable);
        }


    }
}
