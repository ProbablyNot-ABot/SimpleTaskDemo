using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 任务类型枚举
public enum TaskType
{
    KillEnemy,      // 击杀指定怪物
    TalkToNPC,      // 与NPC对话
    CollectItem     // 收集物品
}
//任务状态枚举
public enum TaskStatus
{
    None, // 无状态
    CanReceive, // 可接收
    InProgress, // 进行中
    Completed     // 已完成
}

[System.Serializable]
public class RewardItemData
{
    [Tooltip("奖励的物品")]
    public ItemData item;
    
    [Tooltip("奖励的数量"), Min(1)]
    public int itemAmount;
}

[System.Serializable]
public class TaskTargetData
{
    [Tooltip("任务类型")]
    public TaskType taskType;
    [Tooltip("目标 ID")]
    public int targetID;
    [Tooltip("目标名称（用于显示）")]
    public string targetName;
    [Tooltip("需要完成的数量"), Min(1)]
    public int requiredAmount = 1;
    [Tooltip("当前完成的数量")]
    public int currentAmount = 0;
    [Tooltip("是否完成")]
    public bool isCompleted = false;
}

[CreateAssetMenu(fileName = "NewTaskData", menuName = "Data/TaskData")]
public class TaskData : ScriptableObject
{
    public int taskId;
    public string taskName;
    public int reciveNpcId;
    public int submitNpcId;
    [TextArea(3, 5)]
    public string description;
    public List<RewardItemData> rewardItems;
    public TaskStatus taskStatus;
    public List<TaskTargetData> taskTargets;
    //前置任务ID列表
    public List<int> prerequisiteTaskIds;
}
