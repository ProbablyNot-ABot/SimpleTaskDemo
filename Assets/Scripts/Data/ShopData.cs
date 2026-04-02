using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopData", menuName = "Data/ShopData")]
public class ShopData : ScriptableObject
{
    public string shopName;
    public List<ItemData> shopItems = new List<ItemData>();
}
