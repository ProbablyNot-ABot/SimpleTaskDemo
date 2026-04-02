using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 任务管理器 - 负责管理所有任务数据
/// </summary>
public class QuestManager : BaseManager<QuestManager>
{
    private QuestDataContainer dataContainer;
    
    /// <summary>
    /// 获取所有任务数据
    /// </summary>
    public List<TaskData> AllQuestTasks => dataContainer != null ? dataContainer.allTasks : null;
    public List<TaskData> ReceivedQuestTasks = new();
    
    /// <summary>
    /// 任务状态变化事件
    /// 参数：任务数据，新的状态
    /// </summary>
    public UnityAction<TaskData> OnQuestStatusChanged;
    
    /// <summary>
    /// 触发任务状态变化事件
    /// </summary>
    protected void QuestStatusChanged(TaskData task)
    {
        OnQuestStatusChanged?.Invoke(task);
    }
    public void Init()
    {
        // 加载任务数据容器
        dataContainer = Resources.Load<QuestDataContainer>("Data/QuestDataContainer");
        if (dataContainer != null && dataContainer.allTasks != null)
        {
            Debug.Log($"[QuestManager] 初始化完成，加载了 {AllQuestTasks.Count} 个任务");
            // 加载所有状态为 InProgress 和 Completed 的任务到已接取列表
            foreach (var task in AllQuestTasks)
            {
                if (task.taskStatus == TaskStatus.InProgress || task.taskStatus == TaskStatus.Completed)
                    ReceivedQuestTasks.Add(task);
            }
            Debug.Log($"[QuestManager] 共加载 {ReceivedQuestTasks.Count} 个已接取任务");
        }
        else  Debug.LogWarning("加载任务数据失败");
        OnQuestStatusChanged += OnQuestStatusChangedHandler;
    }
    
    /// <summary>
    /// 任务状态变化事件处理函数
    /// </summary>
    private void OnQuestStatusChangedHandler(TaskData task)
    {
        if (task.taskStatus == TaskStatus.None && task.rewardItems != null && task.rewardItems.Count > 0)
            GrantReward(task);        
    }

    public void StartQuest(int questId)
    {
        // 检查任务 ID 是否有效（ID 从 1 开始，索引从 0 开始）
        if (questId >= 1 && questId <= AllQuestTasks.Count)
        {
            ReceivedQuestTasks.Add(AllQuestTasks[questId-1]);
            Debug.Log($"[QuestManager] 开始任务 {questId}");
            AllQuestTasks[questId-1].taskStatus = TaskStatus.InProgress;
            QuestStatusChanged(AllQuestTasks[questId-1]);
        }
        else
            Debug.LogWarning($"任务 ID {questId} 不存在");
    }

    public void CompleteQuest(int questId)
    {
        // 检查任务 ID 是否有效
        if (questId >= 1 && questId <= AllQuestTasks.Count)
        {
            AllQuestTasks[questId-1].taskStatus = TaskStatus.None;
            ReceivedQuestTasks.Remove(AllQuestTasks[questId-1]);
            QuestStatusChanged(AllQuestTasks[questId-1]);
            Debug.Log($"[QuestManager] 任务 {questId} 已完成");
        }
        else
            Debug.LogWarning($"任务 ID {questId} 不存在");
    }

    public void GrantReward(TaskData taskData)
    {
        // 检查任务 ID 是否有效
        if (taskData.taskId >= 1 && taskData.taskId <= AllQuestTasks.Count)
        {
            if (taskData.rewardItems != null && taskData.rewardItems.Count > 0) //发放奖励
            {
                foreach (var reward in taskData.rewardItems)
                {
                    if (reward.item != null && reward.itemAmount > 0)
                    {
                        bool success = InventoryManager.Instance.AddItem(reward.item, reward.itemAmount);
                        if (success)
                            Debug.Log($"[QuestManager] 发放任务 {taskData.taskName} 奖励：{reward.item.itemName} x{reward.itemAmount}");
                        else
                            Debug.LogWarning($"[QuestManager] 发放任务 {taskData.taskName} 奖励失败：{reward.item.itemName} x{reward.itemAmount}");
                    }
                }
            }
        }
        else
            Debug.LogWarning($"任务 ID {taskData.taskId} 不存在");
    }

    public void ShowQuestDetail(TaskData questData)
    {
        Debug.Log($"[QuestManager] 显示任务详情 {questData.taskName}");
        QuestPanel questPanel = UIManager.Instance.OpenPanel<QuestPanel>("Quest/QuestPanel", UILayerType.Midle,questData);
        questPanel.ShowQuestDetail(questData);
    }

    public void UpdateQuestAmount(TaskTargetData target)
    {
        target.currentAmount++;
        if (target.currentAmount >= target.requiredAmount && !target.isCompleted)
        {
            target.currentAmount = target.requiredAmount;
            target.isCompleted = true;
        }
        QuestPanel questPanel = UIManager.Instance.GetPanel("Quest/QuestPanel") as QuestPanel;
        if (questPanel != null)
            questPanel.UpdateTargetProgress(target.targetID, target.currentAmount, target.requiredAmount);
    }

    public void UpdateQuestProgress(TaskTargetData target)
    {
        // 先更新进度数据
        UpdateQuestAmount(target);
        // 检查任务是否完成
        CheckTaskCompletion(target);
    }
    
    /// <summary>
    /// 检查任务是否完成（所有目标都已完成）
    /// </summary>
    private void CheckTaskCompletion(TaskTargetData completedTarget)
    {
        // 找到包含该目标的任务
        foreach (var task in ReceivedQuestTasks)
        {
            if (task.taskTargets != null)
            {
                // 检查是否包含这个目标
                bool hasTarget = false;
                foreach (var target in task.taskTargets)
                {
                    if (target.targetID == completedTarget.targetID)
                    {
                        hasTarget = true;
                        break;
                    }
                }
                if (!hasTarget)
                    continue;
                // 检查所有目标是否都已完成
                bool allCompleted = true;
                foreach (var target in task.taskTargets)
                {
                    if (!target.isCompleted)
                    {
                        allCompleted = false;
                        break;
                    }
                }
                // 如果所有目标都完成，且任务状态还是进行中，则触发状态变化
                if (allCompleted && task.taskStatus == TaskStatus.InProgress)
                {
                    task.taskStatus = TaskStatus.Completed;
                    QuestStatusChanged(task);
                    Debug.Log($"[QuestManager] 任务 {task.taskName} 完成！");
                }
                break;
            }
        }
    }
}
