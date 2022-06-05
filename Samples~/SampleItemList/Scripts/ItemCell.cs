using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gilzoide.AssetList.Samples.SampleItemList
{
    public class ItemCell : MonoBehaviour
    {
        public Text NameText;

        public void Setup(Item item)
        {
            NameText.text = $"{item.Name} - {item.Level}";
        }
    }
}
