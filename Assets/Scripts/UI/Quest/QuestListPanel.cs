using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestListPanel : BasePanel
{
    public GameObject questItemPrefab;
    public Transform questListContainer;

    protected override void OnOpen(object data = null)
    {
        base.OnOpen(data);
        ClearQuestList();
        InitQuestList();
    }
    /// <summary>
    /// 初始化任务列表
    /// </summary>
    private void InitQuestList()
    {        
        // 遍历任务数据，创建任务项
        foreach (TaskData questData in QuestManager.Instance.ReceivedQuestTasks)
        {
            GameObject questItemObj = Instantiate(questItemPrefab, questListContainer);
            questItemObj.GetComponent<Btn_OnQuest>().Init(questData);
        }
    }
    
    private void ClearQuestList()
    {
        foreach (Transform child in questListContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
