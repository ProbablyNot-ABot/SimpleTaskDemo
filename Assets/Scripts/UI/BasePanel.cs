using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有 UI 面板的基类，提供统一的生命周期接口
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    protected string panelName;
    protected bool isOpened;
    private static List<BasePanel> openPanels = new List<BasePanel>();
    public static IReadOnlyList<BasePanel> OpenPanels => openPanels.AsReadOnly();
    public bool closeOtherPanels = true;
    
    [Header("UI 层级设置")]
    [Tooltip("面板所属的 UI 层级")]
    public UILayerType uiLayer = UILayerType.Bottom;

    /// <summary>
    /// 由 UIManager 在首次实例化时调用，用于初始化组件和事件
    /// </summary>
    public void Init(string name)
    {
        panelName = name;
        OnInit();
    }

    /// <summary>
    /// 由 UIManager 在显示界面时调用
    /// </summary>
    public void Open(object data = null)
    {
        if (isOpened) return;
        
        // 关闭其他已打开的面板
        if (closeOtherPanels)   CloseOtherPanels();
        
        isOpened = true;
        openPanels.Add(this);
        gameObject.SetActive(true);
        OnOpen(data);
    }

    /// <summary>
    /// 关闭其他所有已打开的面板
    /// </summary>
    private void CloseOtherPanels()
    {
        // 复制一份列表，避免在遍历中修改集合
        var panelsToClose = new List<BasePanel>(openPanels);
        
        foreach (var panel in panelsToClose)
        {
            if (panel != this && panel.isOpened)
            {
                panel.Close();
            }
        }
    }

    /// <summary>
    /// 由 UIManager 在关闭界面时调用
    /// </summary>
    public void Close()
    {
        if (!isOpened) return;
        isOpened = false;
        openPanels.Remove(this);
        OnClose();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 彻底销毁时调用（如从池中移除）
    /// </summary>
    public void Dispose()
    {
        OnDispose();
        Destroy(gameObject);
    }

    // ---------- 子类可重写的生命周期方法 ----------
    protected virtual void OnInit() { }
    protected virtual void OnOpen(object data) { }
    protected virtual void OnClose() { }
    protected virtual void OnDispose() { }
}