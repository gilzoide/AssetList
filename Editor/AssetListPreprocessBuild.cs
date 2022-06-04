#if UNITY_2018_1_OR_NEWER
#define HAVE_IPREPROCESS_BUILD_WITH_REPORT
#endif

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
#if HAVE_IPREPROCESS_BUILD_WITH_REPORT
using UnityEditor.Build.Reporting;
#endif

namespace Gilzoide.AssetList.Editor
{
    /// <summary>Updates all AssetLists when building the project.</summary>
    class AssetListPreprocessBuild :
#if HAVE_IPREPROCESS_BUILD_WITH_REPORT
        IPreprocessBuildWithReport
#else
        IPreprocessBuild
#endif
    {
        public int callbackOrder => 0;

#if HAVE_IPREPROCESS_BUILD_WITH_REPORT
        public void OnPreprocessBuild(BuildReport report)
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
#endif
        {
            UpdateAllLists();
        }

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
    }
}
