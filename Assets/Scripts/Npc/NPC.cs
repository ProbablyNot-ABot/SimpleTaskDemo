using UnityEngine;

/// <summary>
/// NPC 组件 - 挂载到 NPC GameObject 上
/// 职责：存储 NPC 数据、自动注册到 NPCManager
/// </summary>
public class NPC : MonoBehaviour
{
    [Header("NPC 信息")]
    public int npcID = 0;
    public string npcName;
    public QuestMark questMark;
    [Header("对话数据")]
    public DialogData dialogData;
    
    public void Start()
    {   
        questMark = UIManager.Instance.LoadWorldUI<QuestMark>("QuestMark", transform.position + Vector3.up * 1.0f);
        QuestManager.Instance.OnQuestStatusChanged += OnQuestStatusChanged;
        // 检查自身是否有可接取的任务（由 NPCManager 统一管理）
        NPCManager.Instance.UpdateNPCQuestMark(this);
    }
    void OnQuestStatusChanged(TaskData task)
    {
        NPCManager.Instance.UpdateNPCQuestMark(this);
    }
    private void OnEnable()
    {
        // 自动注册到 NPCManager
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.RegisterNPC(this);
        }
    }
    
    private void OnDisable()
    {
        // 自动注销
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.UnregisterNPC(this);
        }
    }
}
