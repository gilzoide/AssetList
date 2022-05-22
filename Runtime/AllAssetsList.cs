using System;
using System.Diagnostics;
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

        public Object[] Assets;

        [Conditional("UNITY_EDITOR"), ContextMenu("Update List")]
        public void UpdateList()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + AssetTypeName);
            Array.Sort(guids);
            Assets = guids
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(Object)))
                .ToArray();
        }

        [Conditional("UNITY_EDITOR")] public void OnValidate() => UpdateList();
        [Conditional("UNITY_EDITOR")] public void OnEnable() => UpdateList();

        public static bool TypeFilter(Type type) => !type.IsSubclassOf(typeof(Component));
    }
}
