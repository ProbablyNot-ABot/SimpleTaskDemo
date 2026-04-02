using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC 管理器 - 管理所有 NPC 相关逻辑
/// 职责：NPC 状态管理、NPC 交互、NPC 事件触发
/// </summary>
public class NPCManager : BaseManager<NPCManager>
{
    private List<NPC> activeNPCs = new List<NPC>();
    
    /// <summary>
    /// 注册 NPC
    /// </summary>
    public void RegisterNPC(NPC npc)
    {
        if (npc != null && !activeNPCs.Contains(npc))
        {
            activeNPCs.Add(npc);
        }
    }
    
    /// <summary>
    /// 注销 NPC
    /// </summary>
    public void UnregisterNPC(NPC npc)
    {
        if (npc != null)
        {
            activeNPCs.Remove(npc);
        }
    }
    
    /// <summary>
    /// 获取所有活跃的 NPC
    /// </summary>
    public List<NPC> GetAllActiveNPCs()
    {
        return new List<NPC>(activeNPCs);
    }
    
    /// <summary>
    /// 根据名称查找 NPC
    /// </summary>
    public NPC FindNPCByName(string name)
    {
        foreach (var npc in activeNPCs)
        {
            if (npc.npcName == name)
            {
                return npc;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 根据 ID 查找 NPC
    /// </summary>
    public NPC FindNPCById(int npcId)
    {
        foreach (var npc in activeNPCs)
        {
            if (npc.npcID == npcId)
            {
                return npc;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 更新指定 NPC 的任务标记显示状态
    /// </summary>
    public void UpdateNPCQuestMark(NPC npc)
    {
        if (npc == null || QuestManager.Instance == null || QuestManager.Instance.AllQuestTasks == null)
            return;
        TaskStatus typeStatus = TaskStatus.None;  // 默认状态
        // 遍历所有任务，检查是否有可接取的任务
        foreach (var task in QuestManager.Instance.AllQuestTasks)
        {
            // 检查是否是这个 NPC 提供的任务
            if (task.reciveNpcId == npc.npcID)
            {
                // 检查任务是否可接取
                if (IsQuestAvailable(task))
                {
                    typeStatus = TaskStatus.CanReceive;  // 可接取任务优先级最高
                    break;
                } 
                // 检查是否有可提交的任务（已完成但未提交）
                if (task.taskStatus == TaskStatus.Completed && QuestManager.Instance.ReceivedQuestTasks.Contains(task))
                {
                    typeStatus = TaskStatus.Completed;  // 可提交任务
                }
            }
        }
        // 更新任务标记显示状态
        if (npc.questMark != null)
            npc.questMark.SetMark(typeStatus);
    }
    
    /// <summary>
    /// 检查任务是否可接取
    /// </summary>
    private bool IsQuestAvailable(TaskData task)
    {
        // 检查是否已接取
        if (QuestManager.Instance.ReceivedQuestTasks.Contains(task))
            return false;

        // 检查是否已完成
        if (task.taskStatus == TaskStatus.Completed || task.taskStatus == TaskStatus.None || 
            task.taskStatus == TaskStatus.InProgress)
            return false;
        
        // TODO: 后续添加前置任务检查
        return true;  // 任务可接取
        
    }
    
    /// <summary>
    /// 是否与 NPC 交互中
    /// </summary>
    public bool IsInteractingWithNPC { get; set; }
}
