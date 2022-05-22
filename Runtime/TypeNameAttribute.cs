using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Gilzoide.AllAssetsList
{
    [Conditional("UNITY_EDITOR")]
    public class TypeNameAttribute : PropertyAttribute
    {
        public Type BaseClass = typeof(UnityEngine.Object);
        public string TypeFilterMethod;
    }
}
