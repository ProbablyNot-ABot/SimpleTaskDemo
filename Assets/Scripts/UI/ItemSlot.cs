using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 物品来源类型
/// </summary>
public enum ItemSourceType
{
    Shop,      // 从商店打开
    Inventory  // 从背包打开
}

public class ItemSlot : MonoBehaviour
{
    public Image itemIcon;
    public Text itemName;
    public Text itemValue;
    private ItemData itemData;
    public Button button;
    private ItemSourceType sourceType;  // 物品来源
    
    private void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }
    
    /// <summary>
    /// 设置物品数据（带来源类型）
    /// </summary>
    public void SetupItem(ItemData item, ItemSourceType source)
    {
        itemData = item;
        sourceType = source;
        
        if (itemIcon != null && item.icon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
        }
        else if (itemIcon != null)
        {
            itemIcon.enabled = false;
        }
        
        if (itemName != null)
        {
            itemName.text = item.itemName;
        }
        
        if (itemValue != null)
        {
            itemValue.text = item.value.ToString();
        }
    }
    
    /// <summary>
    /// 按钮点击处理
    /// </summary>
    private void OnButtonClicked()
    {
        Tipspanel tipPanel = UIManager.Instance.OpenPanel<Tipspanel>("Tipspanel", UILayerType.Midle, itemData);
        // 根据来源区分打开方式
        if (sourceType == ItemSourceType.Shop)  tipPanel.buyPanel.SetActive(true);
        else if (sourceType == ItemSourceType.Inventory) tipPanel.buyPanel.SetActive(false);
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
