using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话条件（只支持任务状态条件）
/// </summary>
[System.Serializable]
public class DialogCondition
{
    [Tooltip("是否需要任务条件")]
    public bool hasCondition = false;
    
    [Tooltip("任务 ID")]
    public string questId;
    
    [Tooltip("需要的任务状态")]
    public TaskStatus requiredState;
}

/// <summary>
/// 对话动作（只支持任务相关动作）
/// </summary>
[System.Serializable]
public class DialogAction
{
    [Tooltip("是否需要动作")]
    public bool hasAction = false;
    
    [Tooltip("动作类型")]
    public enum ActionType
    {
        None,           // 无动作
        StartQuest,     // 开始任务
        CompleteQuest,  // 完成任务
        OpenShop,       // 打开商店
    }
    
    public ActionType type;
    
    [Tooltip("任务 ID")]
    public string questId;
    
    [Tooltip("商店数据 ID（当 type 为 OpenShop 时）")]
    public GameObject shopPrefab;
}

/// <summary>
/// 对话选项（分支点）
/// </summary>
[System.Serializable]
public class DialogOption
{
    [Tooltip("选项显示的文字")]
    public string optionText;
    
    [Tooltip("跳转到哪个对话节点（留空则结束对话）")]
    public string nextNodeId;
    
    [Tooltip("前置条件（满足条件才显示此选项）")]
    public DialogCondition condition;
    
    [Tooltip("选择此选项后执行的动作")]
    public DialogAction action;
    
    [Tooltip("选项说明（仅编辑器可见）")]
    public string description;
}

/// <summary>
/// 对话节点（一段对话内容）
/// </summary>
[System.Serializable]
public class DialogNode
{
    [Tooltip("节点唯一标识（不能重复）")]
    public string nodeId;
    
    [Tooltip("说话者名字（留空则使用 NPC 名称）")]
    public string speakerName;
    
    [Tooltip("对话内容（支持多页）")]
    [TextArea(3, 5)]
    public List<string> content = new List<string>();
    
    [Tooltip("对话结束后的选项列表")]
    public List<DialogOption> options = new List<DialogOption>();
    
    [Tooltip("显示此对话前的前置条件")]
    public DialogCondition enterCondition;
    
    [Tooltip("节点说明（仅编辑器可见）")]
    public string description;
}

/// <summary>
/// 对话数据容器（ScriptableObject）
/// </summary>
[CreateAssetMenu(fileName = "NewDialogData", menuName = "Data/DialogData")]
public class DialogData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("对话唯一标识（建议使用格式：NPC 名_对话类型_编号）")]
    public string dialogId;
    
    [Tooltip("NPC 名称")]
    public string npcName;
    
    [Tooltip("对话描述（仅编辑器可见）")]
    [TextArea(2, 3)]
    public string description;
    
    [Header("对话节点列表")]
    [Tooltip("所有对话节点")]
    public List<DialogNode> nodes = new List<DialogNode>();
    
    [Header("起始节点")]
    [Tooltip("对话开始的节点 ID")]
    public string startNodeId;
    
    [Header("重复对话设置")]
    [Tooltip("是否允许重复对话")]
    public bool allowRepeat = false;
    
    [Tooltip("重复对话的节点 ID（留空则使用 startNodeId）")]
    public string repeatNodeId;
    
#if UNITY_EDITOR
    /// <summary>
    /// 编辑器辅助方法：获取节点
    /// </summary>
    public DialogNode GetNode(string nodeId)
    {
        return nodes.Find(n => n.nodeId == nodeId);
    }
    
    /// <summary>
    /// 编辑器辅助方法：验证数据
    /// </summary>
    public List<string> Validate()
    {
        List<string> errors = new List<string>();
        
        if (string.IsNullOrEmpty(dialogId))
            errors.Add("dialogId 不能为空");
        
        if (string.IsNullOrEmpty(startNodeId))
            errors.Add("startNodeId 不能为空");
        
        if (nodes.Count == 0)
            errors.Add("至少需要一个对话节点");
        
        // 检查节点 ID 重复
        HashSet<string> nodeIds = new HashSet<string>();
        foreach (var node in nodes)
        {
            if (string.IsNullOrEmpty(node.nodeId))
                errors.Add($"节点 {node.description} 的 nodeId 为空");
            
            if (!nodeIds.Add(node.nodeId))
                errors.Add($"节点 ID '{node.nodeId}' 重复");
        }
        
        // 检查起始节点是否存在
        if (GetNode(startNodeId) == null)
            errors.Add($"起始节点 '{startNodeId}' 不存在");
        
        // 检查选项跳转的节点是否存在
        foreach (var node in nodes)
        {
            foreach (var option in node.options)
            {
                if (!string.IsNullOrEmpty(option.nextNodeId) && GetNode(option.nextNodeId) == null)
                    errors.Add($"节点 '{node.nodeId}' 的选项 '{option.optionText}' 跳转到不存在的节点 '{option.nextNodeId}'");
            }
        }
        
        return errors;
    }
#endif
}
