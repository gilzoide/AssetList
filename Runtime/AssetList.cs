using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Gilzoide.AllAssetsList
{
    [CreateAssetMenu(menuName = "AssetList", fileName = "AssetList")]
    public class AssetList : ScriptableObject
    {
        public string SearchFilter;
        public List<Object> Assets;

#if UNITY_EDITOR
        [ContextMenu("Update List")]
        void UpdateList()
        {
            if (string.IsNullOrEmpty(SearchFilter))
            {
                return;
            }

            string[] guids = AssetDatabase.FindAssets(SearchFilter);
            Array.Sort(guids);
            Assets = guids
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(Object)))
                .ToList();
            EditorUtility.SetDirty(this);
        }

        void OnEnable()
        {
            UpdateList();
        }

        [CustomEditor(typeof(AssetList)), CanEditMultipleObjects]
        public class AssetListEditor : Editor 
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                SerializedProperty property = serializedObject.GetIterator();
                property.NextVisible(true);
                do
                {
                    using (property.name == "m_Script" ? new EditorGUI.DisabledScope(true) : default)
                    {
                        EditorGUILayout.PropertyField(property);
                    }
                    if (property.name == nameof(AssetList.SearchFilter) && GUILayout.Button("Update List"))
                    {
                        foreach (Object obj in targets)
                        {
                            (obj as AssetList).UpdateList();
                        }
                    }
                }
                while (property.NextVisible(false));
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
