using System;
using UnityEditor;
using UnityEngine;


namespace MalbersAnimations.Reactions
{
    [Serializable]
    public abstract class Reaction
    {
        /// <summary>Instant Reaction ... without considering Active or Delay parameters</summary>
        protected abstract bool _TryReact(Component reactor);

        /// <summary>Get the Type of the reaction</summary>
        public abstract Type ReactionType { get; }

        public void React(Component component) => TryReact(localComponent.useLocal ? localComponent.component : component);

        public void React(GameObject go) => TryReact(go.transform);

        [Tooltip("Enable or Disable the Reaction")]
        [HideInInspector] public bool Active = true;


        public LocalComponet localComponent;

        //[Tooltip("If local is true, the component used for the reaction will not change when you send a Dynamic value")]
        //[FormerlySerializedAs("useLocalTarget")]
        ////[HideInInspector] 
        //public bool useLocal;

        //[Hide("useLocal")]
        //[Tooltip("Local component to apply the reaction\n Make sure the Component is the correct Type!!")]
        //[SerializeField, RequiredField]
        //[FormerlySerializedAs("LocalTarget")]
        ////[HideInInspector] 
        //protected Component LocalComponent;

        //[Tooltip("Delay the Reaction this ammount of seconds")]
        [HideInInspector]
        [Min(0)] public float delay = 0;

        /// <summary>  Checks and find the correct component to apply a reaction  </summary>  
        public Component VerifyComponent(Component component)
        {
            Component TrueComponent;

            //Find if the component is the same 
            if (ReactionType.IsAssignableFrom(component.GetType()))
            {
                TrueComponent = component;
            }
            else
            {
                //Debug.Log($"Component {component.name} REACTION TYPE: {ReactionType.Name}");

                TrueComponent = component.GetComponent(ReactionType);

                if (TrueComponent == null)
                    TrueComponent = component.GetComponentInParent(ReactionType);
                if (TrueComponent == null)
                    TrueComponent = component.GetComponentInChildren(ReactionType);
            }

            return TrueComponent;
        }

        public bool TryReact(Component dynamicComponent)
        {
            if (Application.isPlaying) //Reactions cannot be called in Editor!!
            {
                if (dynamicComponent == null)
                {
                    Debug.Log($"Component is null. Ignoring the Reaction. <b>[{ReactionType.Name}] </b>");
                    return false; //NO Component to React
                }


                if (Active)
                {
                    if (localComponent.useLocal && localComponent.component != null) //Use Local Target and ignore the dynamic Component
                    {
                        dynamicComponent = VerifyComponent(localComponent.component);
                    }
                    else
                    {
                        //Check if the component is the correct component.. a first time
                        if (dynamicComponent != null)
                        {
                            dynamicComponent = VerifyComponent(dynamicComponent);
                        }
                    }


                    //Find the First MonoBehaviour to use Coroutines (Check First if the component is already a Mono)
                    var Delay = (dynamicComponent is MonoBehaviour MB ? MB : null) ?? dynamicComponent.GetComponent<MonoBehaviour>();

                    //If the Reaction has a Delay
                    if (delay > 0 && Delay != null)
                    {
                        Delay.Delay_Action(delay, () => _TryReact(dynamicComponent));
                        return true;
                    }
                    else
                    {
                        return _TryReact(dynamicComponent);
                    }
                }
            }
            return false;
        }

        //React to multiple components
        public bool TryReact(params Component[] components)
        {
            if (Active && components != null && components.Length > 0)
            {
                foreach (var component in components)
                {
                    var comp = VerifyComponent(component);
                    _TryReact(comp);
                }
            }
            return true;
        }
    }


    [System.Serializable]
    public struct LocalComponet
    {
        [RequiredField] public Component component;
        public bool useLocal;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LocalComponet))]
    public class SubclassSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int indent = EditorGUI.indentLevel;

            EditorGUI.indentLevel = 0;

            label = EditorGUI.BeginProperty(position, new GUIContent("Use Local", "If local is true, the component used for the reaction will not change when you send a Dynamic value"), property);
            position = EditorGUI.PrefixLabel(position, label);
            var component = property.FindPropertyRelative("component");
            var useLocal = property.FindPropertyRelative("useLocal");


            var useLocalRect = new Rect(position)
            {
                width = 20f,
                height = EditorGUIUtility.singleLineHeight,
                x = position.x,
            };

            EditorGUI.PropertyField(useLocalRect, useLocal, GUIContent.none, false);

            if (useLocal.boolValue)
            {
                position.x += 17;
                position.width -= 17;

                EditorGUI.PropertyField(position, component, GUIContent.none, false);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
#endif
}