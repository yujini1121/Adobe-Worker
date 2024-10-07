using System;
using UnityEngine;

namespace MalbersAnimations
{
    [System.Serializable]
    [AddTypeMenu("Malbers/Set Local Variable")]
    public class MLocalVarsReaction : Reactions.Reaction
    {
        [Header("Variable Name")]
        public LocalVar var;
        public override Type ReactionType => typeof(MLocalVars);

        protected override bool _TryReact(Component reactor)
        {
            var m = reactor as MLocalVars;
            m.SetVar(var);
            return true;
        }
    }
}