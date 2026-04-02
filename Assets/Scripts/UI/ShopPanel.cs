using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店面板 - 只负责 UI 显示
/// 职责：显示商店物品列表（纯 UI 层，不处理业务逻辑）
/// </summary>
public class ShopPanel : BasePanel
{
    [Header("UI References")]
    public Transform itemGrid;
    public GameObject itemSlotPrefab;
    public Text shopTitle;
    
    [Header("Data")]
    public ShopData shopData;
    
    protected override void OnInit()
    {
        base.OnInit();
        
        // 初始化商店标题
        if (shopData != null && shopTitle != null)
        {
            shopTitle.text = shopData.shopName;
        }
    }
    
    protected override void OnOpen(object data = null)
    {
        base.OnOpen(data);
        DisplayAllItems();
    }
    
    /// <summary>
    /// 显示所有物品
    /// </summary>
    private void DisplayAllItems()
    {
        if (itemGrid == null || shopData == null)
        {
            Debug.LogError("ItemGrid or ShopData is null!");
            return;
        }
        
        ClearAllItems();
        
        foreach (ItemData item in shopData.shopItems)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemGrid);
            ItemSlot slotScript = slot.GetComponent<ItemSlot>();
            if (slotScript != null)
            {
                slotScript.SetupItem(item, ItemSourceType.Shop);
            }
        }
    }
    
    /// <summary>
    /// 清空所有物品
    /// </summary>
    private void ClearAllItems()
    {
        foreach (Transform child in itemGrid)
        {
            Destroy(child.gameObject);
        }
    }
}
