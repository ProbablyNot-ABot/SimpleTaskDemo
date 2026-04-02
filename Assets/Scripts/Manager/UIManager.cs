using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 层级枚举
/// </summary>
public enum UILayerType
{
    Bottom,     // 底层
    Midle,      // 中层
    Top         // 顶层
}

/// <summary>
/// UI 管理器，负责加载、显示、隐藏面板
/// </summary>
public class UIManager : BaseMonoManager<UIManager>
{
    [Header("UI 层级根节点")]
    [Tooltip("底层 Canvas")]
    [SerializeField] private Transform bottomLayerRoot;
    [Tooltip("中层 Canvas")]
    [SerializeField] private Transform midleLayerRoot;
    [Tooltip("顶层 Canvas")]
    [SerializeField] private Transform topLayerRoot;
    [Tooltip("世界 Canvas")]
    [SerializeField] private Transform worldLayerRoot;
    private Dictionary<string, BasePanel> panelCache;   // 已打开的面板缓存

    protected override void OnStart()
    {
        base.OnStart();
        panelCache = new Dictionary<string, BasePanel>();
        // 初始化 UI 层级
        InitializeUILayers();
    }
    
    /// <summary>
    /// 初始化 UI 层级，设置各层级的 sortingOrder
    /// </summary>
    private void InitializeUILayers()
    {
        // 设置普通层
        if (bottomLayerRoot != null)
        {
            Canvas normalCanvas = bottomLayerRoot.GetComponent<Canvas>();
            if (normalCanvas != null)
            {
                normalCanvas.sortingOrder = 0;
            }
        }
        // 设置中层
        if (midleLayerRoot != null)
        {
            Canvas midleCanvas = midleLayerRoot.GetComponent<Canvas>();
            if (midleCanvas != null)
            {
                midleCanvas.sortingOrder = 100;
            }
        }
        // 设置顶层
        if (topLayerRoot != null)
        {
            Canvas topCanvas = topLayerRoot.GetComponent<Canvas>();
            if (topCanvas != null)
            {
                topCanvas.sortingOrder = 200;
            }
        }
    }
    
    public T LoadWorldUI<T>(string prefabName, Vector3 worldPosition) where T : MonoBehaviour
    {
        // 加载预制体
        GameObject prefab = ObjectPoolManager.Instance.GetObject(prefabName, worldPosition, Quaternion.identity, worldLayerRoot);
        if (prefab == null)
        {
            Debug.LogError($"[UIManager] 加载世界 UI 失败：未找到预制体 {prefabName}");
            return null;
        }
        // 获取组件
        if (prefab.TryGetComponent<T>(out var component))
            return component;
        return null;
    }
    
    /// <summary>
    /// 打开面板
    /// </summary>
    /// <typeparam name="T">面板类型（必须继承 BasePanel）</typeparam>
    /// <param name="panelName">预制体名称（位于 Resources/UI/ 下，支持子文件夹路径，如 "Folder/PanelName"）</param>
    /// <param name="layer">UI 层级</param>
    /// <param name="data">传递给 OnOpen 的数据</param>
    public T OpenPanel<T>(string panelName, UILayerType layer = UILayerType.Bottom, object data = null) where T : BasePanel
    {
        // 检查缓存
        if (panelCache.TryGetValue(panelName, out BasePanel panel))
        {
            panel.Open(data);
            return panel as T;
        }
        
        // 加载预制体（支持子文件夹路径）
        string resourcePath = $"UI/{panelName}";
        GameObject prefab = Resources.Load<GameObject>(resourcePath);
        if (prefab == null)
        {
            Debug.LogError($"未找到 UI 预制体：{resourcePath}");
            return null;
        }
        
        // 根据层级选择父节点
        Transform parent = GetLayerRoot(layer);
        
        GameObject obj = Instantiate(prefab, parent);
        T newPanel = obj.GetComponent<T>();
        if (newPanel == null)
        {
            Debug.LogError($"预制体 {panelName} 上未找到 {typeof(T)} 组件");
            Destroy(obj);
            return null;
        }
        
        obj.SetActive(false);
        newPanel.Init(panelName);
        panelCache[panelName] = newPanel;

        newPanel.Open(data);
        return newPanel;
    }
    
    /// <summary>
    /// 根据层级类型获取对应的根节点
    /// </summary>
    private Transform GetLayerRoot(UILayerType layer)
    {
        switch (layer)
        {
            case UILayerType.Midle:
                return midleLayerRoot;
            case UILayerType.Top:
                return topLayerRoot;
            default:
                return bottomLayerRoot;
        }
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    public void ClosePanel(string panelName)
    {
        if (panelCache.TryGetValue(panelName, out BasePanel panel))
        {
            panel.Close();
        }
    }

    /// <summary>
    /// 关闭所有打开的面板
    /// </summary>
    public void CloseAllPanels()
    {
        var panels = new List<BasePanel>(panelCache.Values);
        foreach (var panel in panels)
        {
            panel.Close();
        }
    }

    /// <summary>
    /// 彻底销毁面板（从缓存中移除并销毁）
    /// </summary>
    public void DestroyPanel(string panelName)
    {
        if (panelCache.TryGetValue(panelName, out BasePanel panel))
        {
            panelCache.Remove(panelName);
            panel.Dispose();
        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    public BasePanel GetPanel(string panelName)
    {
        if (panelCache.TryGetValue(panelName, out BasePanel panel))
        {
            return panel;
        }
        return null;
    }
}