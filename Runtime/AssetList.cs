using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace Gilzoide.AssetList
{
    [CreateAssetMenu(menuName = "AssetList", fileName = "AssetList")]
    public partial class AssetList : ScriptableObject
    {
        /// <summary>
        /// An <cref>AssetDatabase</cref> search filter.
        /// If empty, the <cref>Assets</cref> list will be empty.
        /// </summary>
        [Tooltip("An AssetDatabase search filter. If empty, the Assets list will be empty")]
        public string SearchFilter;

        /// <summary>
        /// List of assets found by <cref>AssetDatabase.FindAssets</cref> using the
        /// <cref>SearchFilter</cref>.
        /// </summary>
        public List<Object> Assets;
    }

#if UNITY_EDITOR
    public partial class AssetList
    {
        /// <summary>Update list when the Unity editor first loads this asset</summary>
        void Awake()
        {
            UpdateList();
        }

        /// <summary>Update list when this asset gets enabled in Play mode</summary>
        void OnEnable()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UpdateList();
            }
        }

        /// <summary>
        /// Update the <cref>Assets</cref> list, based on the configured <cref>SearchFilter</cref>.
        /// </summary>
        [ContextMenu("Update Assets List")]
        internal void UpdateList()
        {
            if (string.IsNullOrEmpty(SearchFilter))
            {
                Assets.Clear();
                return;
            }

            Assets = AssetDatabase.FindAssets(SearchFilter)
                .OrderBy(x => x)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Object>)
                .ToList();
            EditorUtility.SetDirty(this);
        }
    }

    [CustomEditor(typeof(AssetList)), CanEditMultipleObjects]
    class AssetListEditor : Editor 
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
                if (property.name == nameof(AssetList.SearchFilter) && GUILayout.Button("Update Assets List"))
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
