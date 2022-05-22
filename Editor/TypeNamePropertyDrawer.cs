using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Gilzoide.AllAssetsList.Editor
{
    [CustomPropertyDrawer(typeof(TypeNameAttribute))]
    public class TypeNamePropertyDrawer : PropertyDrawer
    {
        private string _currentValue;
        private List<string> _availableTypes;

        private TypeNameAttribute Attribute => (TypeNameAttribute) attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use TypeNameProperty with String properties only");
                return;
            }

            if (_currentValue == null)
            {
                _currentValue = property.stringValue;
            }
            else if (property.stringValue != _currentValue)
            {
                property.stringValue = _currentValue;
            }

            EditorGUI.LabelField(position, label);
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;
            if (!EditorGUI.DropdownButton(position, new GUIContent(_currentValue), FocusType.Keyboard))
            {
                return;
            }
            
            GetGenericMenu().ShowAsContext();
        }

        GenericMenu GetGenericMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (string typeName in GetAvailableTypes())
            {
                menu.AddItem(new GUIContent(typeName.Replace(".", "/")), typeName == _currentValue, OnNameSelected, typeName);
            }

            return menu;
        }

        IEnumerable<string> GetAvailableTypes()
        {
            if (_availableTypes == null)
            {
                IEnumerable<Type> types = TypeCache.GetTypesDerivedFrom(Attribute.BaseClass);
                if (!string.IsNullOrEmpty(Attribute.TypeFilterMethod))
                {
                    var method = fieldInfo.DeclaringType.GetMethod(Attribute.TypeFilterMethod,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    var filterDelegate = (Func<Type, bool>) method.CreateDelegate(typeof(Func<Type, bool>));
                    types = types.Where(filterDelegate);
                }
                _availableTypes = types.Select(type => type.FullName).OrderBy(x => x).ToList();
            }
            return _availableTypes;
        }

        void OnNameSelected(object name)
        {
            _currentValue = (string) name;
        }
    }
}
