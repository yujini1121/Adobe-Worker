using System;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

//AL Serialized Properties and Serialized Reference Extentions and new Methods are located here
namespace MalbersAnimations
{
#if UNITY_EDITOR
    public static class MSerializedTools
    {
        private const string ArrayPropertySubstring = ".Array.data[";
        public static object SetManagedReference(this SerializedProperty property, System.Type type)
        {
            object obj = (type != null) ? System.Activator.CreateInstance(type) : null;
            property.managedReferenceValue = obj;
            return obj;
        }

        public static bool IsArray(this SerializedProperty property)
        {
            return property.propertyPath.Contains(ArrayPropertySubstring);
        }

        public static SerializedProperty GetArrayPropertyFromArrayElement(SerializedProperty property)
        {
            var path = property.propertyPath;
            var startIndexArrayPropertyPath = path.IndexOf(ArrayPropertySubstring);
            var propertyPath = path.Remove(startIndexArrayPropertyPath);
            return property.serializedObject.FindProperty(propertyPath);
        }


        public static IEnumerable<SerializedProperty> Children(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.Next(false);
            }

            if (currentProperty.Next(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.Next(false));
            }
        }


        /// <summary>  Returns the Type of a serialized property </summary>
        public static System.Type GetPropertyType(SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            var fi = parentType.GetFieldViaPath(property.propertyPath);
            return fi.FieldType;
        }

        public static System.Type GetType(string typeName)
        {
            int splitIndex = typeName.IndexOf(' ');
            var assembly = Assembly.Load(typeName[..splitIndex]);
            return assembly.GetType(typeName[(splitIndex + 1)..]);
        }

        public static T GetPropertyAttribute<T>(this SerializedProperty prop, bool inherit) where T : PropertyAttribute
        {
            if (prop == null) return null;
            Type t = prop.serializedObject.targetObject.GetType();


            FieldInfo f = null;
            PropertyInfo p = null;

            Debug.Log($"prop.propertyPath. {prop.propertyPath}");

            foreach (var name in prop.propertyPath.Split('.'))
            {
                f = t.GetField(name, (BindingFlags)(-1));

                if (f == null)
                {
                    p = t.GetProperty(name, (BindingFlags)(-1));
                    if (p == null)
                    {
                        return null;
                    }

                    t = p.PropertyType;
                }
                t = f.FieldType;
            }

            T[] attributes;

            if (f != null)
            {
                attributes = f.GetCustomAttribute(typeof(T), inherit) as T[];
            }
            else if (p != null)
            {
                attributes = p.GetCustomAttribute(typeof(T), inherit) as T[];
            }
            else
            {
                return null;
            }

            return attributes.Length > 0 ? attributes[0] : null;
        }


        /// <summary>  Returns the object value of a serialized property </summary>
        public static object GetValue(this SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
            return fi.GetValue(property.serializedObject.targetObject);
        }

        /// <summary>  Set the object value of a serialized property </summary>
        public static void SetValue(this SerializedProperty property, object value)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);//this FieldInfo contains the type.
            fi.SetValue(property.serializedObject.targetObject, value);
        }

        public static void SetValue(this SerializedProperty property, Type parentType, object value)
        {
            //System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);//this FieldInfo contains the type.
            fi.SetValue(property.serializedObject.targetObject, value);
        }


        /// <summary> Returns attributes of type <typeparamref name="TAttribute"/> on <paramref name="serializedProperty"/>. </summary>
        public static TAttribute[] GetAttributes<TAttribute>(this SerializedProperty serializedProperty, bool inherit)
            where TAttribute : Attribute
        {
            if (serializedProperty == null)
            {
                throw new ArgumentNullException(nameof(serializedProperty));
            }

            var targetObjectType = serializedProperty.serializedObject.targetObject.GetType();

            if (targetObjectType == null)
            {
                throw new ArgumentException($"Could not find the {nameof(targetObjectType)} of {nameof(serializedProperty)}");
            }

            foreach (var pathSegment in serializedProperty.propertyPath.Split('.'))
            {
                var fieldInfo = targetObjectType.GetField(pathSegment, AllBindingFlags);
                if (fieldInfo != null)
                {
                    return (TAttribute[])fieldInfo.GetCustomAttributes<TAttribute>(inherit);
                }

                var propertyInfo = targetObjectType.GetProperty(pathSegment, AllBindingFlags);
                if (propertyInfo != null)
                {
                    return (TAttribute[])propertyInfo.GetCustomAttributes<TAttribute>(inherit);
                }
            }

            throw new ArgumentException($"Could not find the field or property of {nameof(serializedProperty)}");
        }
        private const BindingFlags AllBindingFlags = (BindingFlags)(-1);



        public static T GetBaseProperty<T>(SerializedProperty prop)
        {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            System.Object reflectionTarget = prop.serializedObject.targetObject as object;
            // Walk down the path to get the target object
            foreach (var path in separatedPaths)
            {
                FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }
            return (T)reflectionTarget;
        }

    }
#endif
}
