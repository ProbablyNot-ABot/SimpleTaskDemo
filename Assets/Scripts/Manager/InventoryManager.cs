using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 背包物品格子（运行时使用）
/// </summary>
public class InventorySlot
{
    public ItemData item;
    public int num;
    
    public InventorySlot(ItemData item, int quantity)
    {
        this.item = item;
        this.num = quantity;
    }
}

/// <summary>
/// 背包物品格子（用于 JSON 序列化）
/// </summary>
[System.Serializable]
public class InventorySlotData
{
    public string itemName;
    public int num;
    
    public InventorySlotData(string name, int qty)
    {
        itemName = name;
        num = qty;
    }
}

/// <summary>
/// 背包数据（用于 JSON 序列化和本地存储）
/// </summary>
[System.Serializable]
public class SaveData
{
    public int maxSlots;
    public int maxStackPerSlot;
    public List<InventorySlotData> slots;
    
    public SaveData(int maxSlots, int maxStack, List<InventorySlotData> slots)
    {
        this.maxSlots = maxSlots;
        this.maxStackPerSlot = maxStack;
        this.slots = slots;
    }
}

/// <summary>
/// 背包管理器 - 处理所有背包相关业务逻辑
/// 职责：管理背包物品、物品堆叠、物品使用、数据持久化
/// </summary>
public class InventoryManager : BaseManager<InventoryManager>
{
    private List<InventorySlot> slots = new List<InventorySlot>();
    private int maxSlots = 20;
    private int maxStackPerSlot = 999;
    private string saveFilePath;
    public InventoryManager()
    {
        // 初始化背包
        slots = new List<InventorySlot>();
        saveFilePath = Path.Combine(Application.persistentDataPath, "inventory.json");
    }
    
    /// <summary>
    /// 初始化背包（从文件加载，如果文件不存在则使用默认配置）
    /// </summary>
    public void Initialize()
    {
        if (File.Exists(saveFilePath))
        {
            LoadFromFile();
            Debug.Log("背包数据已从文件加载");
        }
        else
        {
            Debug.Log("背包文件不存在，使用默认配置");
        }
    }
    
    /// <summary>
    /// 添加物品到背包
    /// </summary>
    /// <param name="item">物品数据</param>
    /// <param name="num">数量</param>
    /// <returns>是否添加成功</returns>
    public bool AddItem(ItemData item, int num = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("尝试添加空物品到背包");
            return false;
        }
        // 尝试堆叠到已有的物品
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                int canAdd = maxStackPerSlot - slot.num;
                if (canAdd > 0)
                {
                    int addCount = Mathf.Min(num, canAdd);
                    slot.num += addCount;
                    num -= addCount;
                    if (num <= 0)
                    {
                        Debug.Log($"添加物品：{item.itemName} x{slot.num}");
                        SaveToFile();  // 立即保存
                        return true;
                    }
                }
            }
        }
        // 如果还有剩余，创建新格子
        while (num > 0 && slots.Count < maxSlots)
        {
            int addCount = Mathf.Min(num, maxStackPerSlot);
            slots.Add(new InventorySlot(item, addCount));
            num -= addCount;
        }
        if (num > 0)
        {
            Debug.LogWarning($"背包已满，无法添加 {num} 个 {item.itemName}");
            return false;
        }
        Debug.Log($"添加物品：{item.itemName} x{num}");
        SaveToFile();  // 立即保存
        return true;
    }
    
    /// <summary>
    /// 获取所有物品格子（供 UI 使用）
    /// </summary>
    public List<InventorySlot> GetAllSlots()
    {
        return new List<InventorySlot>(slots);
    }
    
    /// <summary>
    /// 保存背包数据到文件
    /// </summary>
    private void SaveToFile()
    {
        // 转换为可序列化的数据
        List<InventorySlotData> slotDataList = new List<InventorySlotData>();
        foreach (var slot in slots)
        {
            slotDataList.Add(new InventorySlotData(slot.item.itemName, slot.num));
        }
        SaveData saveData = new SaveData(maxSlots, maxStackPerSlot, slotDataList);
        // 序列化为 JSON
        string json = JsonUtility.ToJson(saveData, true);
        // 写入文件
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"背包数据已保存到：{saveFilePath}");
    }
    
    /// <summary>
    /// 从文件加载背包数据
    /// </summary>
    private void LoadFromFile()
    {
        try
        {
            // 读取 JSON
            string json = File.ReadAllText(saveFilePath);
            // 反序列化
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            // 恢复数据
            maxSlots = saveData.maxSlots;
            maxStackPerSlot = saveData.maxStackPerSlot;
            slots.Clear();
            foreach (var slotData in saveData.slots)
            {
                // 根据物品名称查找对应的 ItemData
                ItemData item = FindItemDataByName(slotData.itemName);
                if (item != null)
                {
                    slots.Add(new InventorySlot(item, slotData.num));
                }
            }
            Debug.Log($"背包数据加载成功，共 {slots.Count} 个物品");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载背包数据失败：{e.Message}");
        }
    }
    
    /// <summary>
    /// 根据物品名称查找 ItemData
    /// </summary>
    private ItemData FindItemDataByName(string itemName)
    {
        // 从 Assets/Data/Items 目录加载所有 ItemData
        string[] items = UnityEditor.AssetDatabase.FindAssets("t:ItemData", new string[] { "Assets/Data/Items" });
        foreach (string guid in items)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item != null && item.itemName == itemName)
            {
                return item;
            }
        }
        Debug.LogWarning($"未找到物品：{itemName}");
        return null;
    }
}
