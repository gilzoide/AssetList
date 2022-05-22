using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Gilzoide.AllAssetsList
{
    [CreateAssetMenu(menuName = "ScriptableObject/AllAssetsList")]
    public class AllAssetsList : ScriptableObject
    {
        [TypeNameAttribute(TypeFilterMethod = nameof(TypeFilter))]
        public string AssetTypeName;
        public List<Object> Assets;

        static bool TypeFilter(Type type) => !type.IsSubclassOf(typeof(Component))
            && !type.IsSubclassOf(typeof(GameObject));

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private string _assetTypeName;

        [ContextMenu("Update List")]
        void UpdateList()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + AssetTypeName);
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

        void OnValidate()
        {
            if (_assetTypeName != AssetTypeName)
            {
                _assetTypeName = AssetTypeName;
                UpdateList();
            }
        }
#endif
    }
}
