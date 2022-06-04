using UnityEditor;
using UnityEngine;

namespace Gilzoide.AssetList.Editor
{
    [CustomEditor(typeof(AssetList)), CanEditMultipleObjects]
    class AssetListEditor : UnityEditor.Editor 
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
                if (property.name == nameof(AssetList.SearchInFolders) && GUILayout.Button("Update Assets List"))
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
}
