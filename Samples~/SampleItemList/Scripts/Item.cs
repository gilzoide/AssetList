using UnityEngine;

namespace Gilzoide.AssetList.Samples.SampleItemList
{
    [CreateAssetMenu(menuName = "AssetListSamples/SampleItemList/Item")]
    public class Item : ScriptableObject
    {
        public string Name;
        public int Level;
    }
}
