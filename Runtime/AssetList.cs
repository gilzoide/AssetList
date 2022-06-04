using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Threading.Tasks;
#endif

namespace Gilzoide.AssetList
{
    [CreateAssetMenu(menuName = "AssetList", fileName = "AssetList")]
    public partial class AssetList : ScriptableObject
    {
        /// <summary>
        /// An <see cref="AssetDatabase"/> search filter.
        /// If empty, the <see cref="Assets"/> list will be empty.
        /// </summary>
        /// <seealso cref="AssetDatabase.FindAssets" href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html"/>
        [Tooltip("An AssetDatabase search filter. If empty, the Assets list will be empty.")]
        public string SearchFilter;

        /// <summary>The folders where the search will start.</summary>
        /// <seealso cref="AssetDatabase.FindAssets" href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html"/>
        [Tooltip("The folders where the search will start.")]
        public string[] SearchInFolders;

        /// <summary>
        /// List of assets found by <see cref="AssetDatabase.FindAssets"/> using the
        /// <see cref="SearchFilter"/> and <see cref="SearchInFolders"/>.
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

            Assets = AssetDatabase.FindAssets(SearchFilter, SearchInFolders)
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

    /// <summary>
    /// Updates all AssetLists in project when creating/deleting assets or building.
    /// </summary>
    static class AllAssetListsUpdater
    {
        static bool _updateQueued = false;

        static void UpdateAllLists()
        {
            IEnumerable<AssetList> lists = AssetDatabase.FindAssets("t:" + nameof(AssetList))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AssetList>);
            foreach (AssetList list in lists)
            {
                list.UpdateList();
            }
        }

        static async void QueueUpdateAllLists()
        {
            if (_updateQueued)
            {
                return;
            }

            _updateQueued = true;
            await Task.Yield();
            UpdateAllLists();
            _updateQueued = false;
        }

#if UNITY_2018_1_OR_NEWER
        class AssetListPreprocessBuild : IPreprocessBuildWithReport
        {
            public int callbackOrder => 0;
            public void OnPreprocessBuild(BuildReport report)
            {
                UpdateAllLists();
            }
        }
#else
        class AssetListPreprocessBuild : IPreprocessBuild
        {
            public int callbackOrder => 0;
            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                UpdateAllLists();
            }
        }
#endif

        class AssetListModificationProcessor : UnityEditor.AssetModificationProcessor
        {
            static void OnWillCreateAsset(string path)
            {
                QueueUpdateAllLists();
            }

            static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
            {
                QueueUpdateAllLists();
                return AssetDeleteResult.DidNotDelete;
            }
        }
    }
#endif
}
