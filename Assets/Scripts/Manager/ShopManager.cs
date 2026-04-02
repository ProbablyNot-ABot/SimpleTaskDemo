using System;
using UnityEngine;

/// <summary>
/// 商店管理器 - 处理所有商店相关业务逻辑
/// 职责：管理商店界面、处理购买逻辑、管理商品数据
/// </summary>
public class ShopManager : BaseManager<ShopManager>
{
    /// <summary>
    /// 购买物品
    /// </summary>
    /// <param name="currentItemData">当前物品数据</param>
    /// <param name="num">购买数量</param>
    public bool BuyItem(ItemData currentItemData, int num)
    {
        // 检查余额是否足够
        if (GameManager.Instance.playerData.money < currentItemData.value * num)
        {
            Debug.Log("余额不足");
            return false;
        }
        // 更新余额
        GameManager.Instance.playerData.money -= currentItemData.value * num;
        InventoryManager.Instance.AddItem(currentItemData, num);
        return true;
    }
    
}
