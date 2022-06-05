using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gilzoide.AssetList
{
    [CreateAssetMenu(menuName = "AssetList", fileName = "AssetList")]
    public class AssetList : ScriptableObject
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
        public DefaultAsset[] SearchInFolders;

        /// <summary>
        /// List of assets found by <see cref="AssetDatabase.FindAssets"/> using
        /// <see cref="SearchFilter"/> and <see cref="SearchInFolders"/> as parameters.
        /// </summary>
        public List<Object> Assets;

#if UNITY_EDITOR
        /// <summary>Update list when this asset gets enabled in Play mode</summary>
        void OnEnable()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UpdateList();
            }
        }

        /// <summary>
        /// Update the <see cref="Assets"/> list based on the configured <see cref="SearchFilter"/>
        /// and <see cref="SearchInFolders"/> fields.
        /// </summary>
        [ContextMenu("Update Assets List")]
        public void UpdateList()
        {
            if (string.IsNullOrEmpty(SearchFilter))
            {
                Assets.Clear();
                return;
            }

            string[] searchFolders = SearchInFolders.Select(AssetDatabase.GetAssetPath).ToArray();
            Assets = AssetDatabase.FindAssets(SearchFilter, searchFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Object>)
                .ToList();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
