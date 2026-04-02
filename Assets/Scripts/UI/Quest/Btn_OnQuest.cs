using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Btn_OnQuest : MonoBehaviour
{
    private Button btnShowDetail;
    private TaskData questData;
    private void Start()
    {
        btnShowDetail = GetComponent<Button>();
        btnShowDetail.onClick.AddListener(() =>
        {
            QuestManager.Instance.ShowQuestDetail(questData);
        });
    }
    // 静态字典，只创建一次
    private static readonly Dictionary<TaskStatus, string> StatusMap = new()
    {
        { TaskStatus.InProgress, "进行中" },
        { TaskStatus.Completed, "已完成" },
        { TaskStatus.CanReceive, "可接取" }
    };
    public void Init(TaskData questData)
    {
        this.questData = questData;
        if (StatusMap.TryGetValue(questData.taskStatus, out string statusText))
        {
            GetComponentInChildren<Text>().text = $"[{statusText}] {questData.taskName}";
        }
        
    }
}
