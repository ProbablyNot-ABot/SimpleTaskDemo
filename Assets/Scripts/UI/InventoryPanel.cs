using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : BasePanel
{
    public InventoryData inventoryData;
    
    [Header("物品容器")]
    [Tooltip("用于存放物品格子的父节点")]
    public Transform itemContainer;
    
    [Header("物品格子预制体")]
    [Tooltip("物品格子的预制体")]
    public ItemSlot itemSlotPrefab;
    
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    
    protected override void OnOpen(object data = null)
    {
        base.OnOpen(data);
        // 初始化背包显示
        InitializeInventory();
    }
    
    /// <summary>
    /// 初始化背包，显示所有物品
    /// </summary>
    private void InitializeInventory()
    {
        // 清空现有的物品格子
        ClearItemSlots();
        // 从 InventoryManager 获取所有物品
        List<InventorySlot> slots = InventoryManager.Instance.GetAllSlots();
        if (slots == null || slots.Count == 0)
        {
            Debug.Log("背包为空");
            return;
        }
        // 显示所有物品
        foreach (var slot in slots)
        {
            if (slot.item != null)
            {
                CreateItemSlot(slot.item, slot.num);
            }
        }
    }
    
    /// <summary>
    /// 清空所有物品格子
    /// </summary>
    private void ClearItemSlots()
    {
        if (itemContainer == null)
        {
            Debug.LogWarning("Item Container 未设置！");
            return;
        }
        // 销毁所有子对象
        for (int i = itemContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }
        itemSlots.Clear();
    }
    
    /// <summary>
    /// 创建物品格子
    /// </summary>
    private void CreateItemSlot(ItemData item, int num)
    {
        if (itemContainer == null || itemSlotPrefab == null)
        {
            Debug.LogWarning("Item Container 或 ItemSlot Prefab 未设置！");
            return;
        }
        // 实例化物品格子
        ItemSlot newSlot = Instantiate(itemSlotPrefab, itemContainer);
        // 设置物品数据
        newSlot.SetupItem(item, ItemSourceType.Inventory);
        Text itemValueText = newSlot.GetComponentInChildren<Text>();
        if (itemValueText != null)
            itemValueText.text = item.itemName + " " + $"x{num}";
        itemSlots.Add(newSlot);
    }
}
