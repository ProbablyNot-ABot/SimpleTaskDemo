using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tipspanel : BasePanel
{
    public Image itemIcon;
    public Text itemName;
    public Text itemDescription;
    public Text itemValue;
    public Text remainMoney;
    public InputField inputField;
    public Slider slider;
    public Button buyButton;
    public Button cancelButton;
    public GameObject buyPanel;
    private ItemData currentItemData;
    
    private void Start()
    {
        // 设置 Slider 的范围
        if (slider != null)
        {
            slider.minValue = 1;
            slider.maxValue = 999;
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        
        // 设置 InputField 监听
        if (inputField != null)
        {
            inputField.text = "1";
            inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        }
        
        // 购买按钮监听
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClick);
        }
        
        // 取消按钮监听
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelButtonClick);
        }
    }
    
    /// <summary>
    /// Slider 值变化时调用
    /// </summary>
    private void OnSliderValueChanged(float value)
    {
        if (inputField != null)
        {
            // 更新 InputField 显示（取整数）
            inputField.text = Mathf.RoundToInt(value).ToString();
        }
    }
    
    /// <summary>
    /// InputField 值变化时调用
    /// </summary>
    private void OnInputFieldValueChanged(string value)
    {
        if (slider == null || string.IsNullOrEmpty(value))
            return;
        
        // 验证输入是否为有效的正整数
        if (!int.TryParse(value, out int intValue))
        {
            // 输入无效（包含 - 或 . 等非数字字符），恢复为之前的有效值
            inputField.text = Mathf.RoundToInt(slider.value).ToString();
            return;
        }
        
        // 限制在 1-999 范围内
        intValue = Mathf.Clamp(intValue, 1, 999);
        
        // 更新 Slider 的值
        slider.value = intValue;
        
        // 如果输入的值超出范围，修正 InputField 显示
        if (inputField.text != intValue.ToString())
        {
            inputField.text = intValue.ToString();
        }
    }
     
    /// <summary>
    /// 购买按钮点击
    /// </summary>
    private void OnBuyButtonClick()
    {
        if (inputField != null && int.TryParse(inputField.text, out int num))
        {
            if (num <= 0)   return;
            if (currentItemData == null)   return;
            if (ShopManager.Instance.BuyItem(currentItemData, num))
                remainMoney.text = "余额：$" + GameManager.Instance.playerData.money.ToString();
            else    Debug.Log("余额不足");
        }
    }
    
    /// <summary>
    /// 取消按钮点击
    /// </summary>
    public void OnCancelButtonClick() => UIManager.Instance.ClosePanel(panelName);
    
    /// <summary>
    /// 关闭面板时调用
    /// </summary>
    protected override void OnClose()
    {
        base.OnClose();
        // 重置 InputField 和 Slider 的值
        if (inputField != null)
        {
            inputField.text = "1";
        }
        if (slider != null)
        {
            slider.value = 1;
        }
        // 清空当前物品数据
        currentItemData = null;
    }
    
    private void OnDestroy()
    {
        // 清理事件监听，防止内存泄漏
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
        }
        
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyButtonClick);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(OnCancelButtonClick);
        }
    }

    /// <summary>
    /// 打开面板时调用
    /// </summary>
    protected override void OnOpen(object data = null)
    {
        base.OnOpen(data);
        // 接收传递过来的物品数据
        if (data is ItemData itemData)
        {
            currentItemData = itemData;
            InitializePanel(itemData);
            Debug.Log($"Tipspanel 已初始化：{itemData.itemName}");
        }
        else
        {
            Debug.LogWarning("Tipspanel.OnOpen: 传递的数据不是 ItemData 类型");
        }
    }
    
    /// <summary>
    /// 初始化面板，显示物品信息
    /// </summary>
    private void InitializePanel(ItemData itemData)
    {
        if(itemData != null)
        {
            itemIcon.sprite = itemData.icon;
            itemName.text = itemData.itemName;
            itemDescription.text = itemData.description;
            itemValue.text = "$" + itemData.value.ToString();
            remainMoney.text = "余额：$" + GameManager.Instance.playerData.money.ToString();
        }
    }
}
