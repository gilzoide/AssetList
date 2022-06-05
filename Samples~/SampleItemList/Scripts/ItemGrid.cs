using System.Linq;
using UnityEngine;

namespace Gilzoide.AssetList.Samples.SampleItemList
{
    public class ItemGrid : MonoBehaviour
    {
        public ItemCell CellPrefab;
        public AssetList ItemList;

        void Start()
        {
            foreach (Item item in ItemList.Assets.Cast<Item>())
            {
                ItemCell cell = Instantiate(CellPrefab, transform);
                cell.Setup(item);
            }
        }
    }
}
