using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Gilzoide.AssetList
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

            Assets = AssetDatabase.FindAssets(SearchFilter)
                .OrderBy(Identity)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Object>)
                .ToList();
            EditorUtility.SetDirty(this);
        }

        void OnEnable()
        {
            UpdateList();
        }

        T Identity<T>(T x)
        {
            return x;
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
                    using (new EditorGUI.DisabledScope(property.name == "m_Script"))
                    {
                        EditorGUILayout.PropertyField(property);
                    }
                    if (property.name == nameof(SearchFilter) && GUILayout.Button("Update List"))
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
