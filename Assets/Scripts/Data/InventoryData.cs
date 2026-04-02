using System;
using UnityEngine;

/// <summary>
/// 背包配置数据 - 定义背包的基础属性
/// </summary>
[Serializable]
public class InventoryData
{
    //背包最大格子数
    public int maxSlots = 20;    
    //每个格子的最大堆叠数量
    public int maxStackPerSlot = 999;
    //游戏开始时背包中的物品
    public InitialItem[] initialItems;
}

/// <summary>
/// 初始物品配置
/// </summary>
[Serializable]
public class InitialItem
{
    public ItemData item;      // 物品
    public string itemName;      // 物品名称（用于 JSON 序列化）
    public int num = 1;   // 数量
}
