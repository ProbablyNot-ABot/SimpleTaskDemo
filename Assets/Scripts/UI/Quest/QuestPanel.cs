using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : BasePanel
{
    public Text questName;
    public Text questDescription;
    public Transform targetContainer;  // 任务目标的父容器
    public Text targetTextPrefab;      // 任务目标文本预制体（从 Panel_QuestTarget 下的 Txt_Target_1 获取）
    public Button follow;
    public Button cancel;
    private List<Text> questTargetTexts = new();  // 动态生成的目标文本列表
    
    protected override void OnInit()
    {
        base.OnInit();
        follow.onClick.AddListener(() =>
        {
            //QuestManager.Instance.FollowQuest(questData.taskId);
        });
        cancel.onClick.AddListener(() =>
        {
            UIManager.Instance.ClosePanel("Quest/QuestPanel");
        });
    }
    
    public void ShowQuestDetail(TaskData questData)
    {
        questName.text = questData.taskName;
        questDescription.text = questData.description;
        // 清空旧的目标文本
        ClearTargetTexts();
        // 动态生成任务目标文本
        if (questData.taskTargets != null && questData.taskTargets.Count > 0)
        {
            foreach (var target in questData.taskTargets)
            {
                CreateTargetText(target.targetName, target.currentAmount, target.requiredAmount);
            }
        }
    }
    
    /// <summary>
    /// 创建单个任务目标文本
    /// </summary>
    private void CreateTargetText(string targetName, int currentAmount, int requiredAmount)
    {
        Text newText;
        if (targetTextPrefab != null)
        {
            GameObject newObject = Instantiate(targetTextPrefab.gameObject, targetContainer);
            newObject.SetActive(true);  // 显示实例化的对象
            newText = newObject.GetComponent<Text>();
            // 设置文本内容
            newText.text = $"{targetName}： {currentAmount}/{requiredAmount}";
            // 添加到列表
            questTargetTexts.Add(newText);
        }
    }
    
    /// <summary>
    /// 清空所有目标文本
    /// </summary>
    private void ClearTargetTexts()
    {
        foreach (var text in questTargetTexts)
        {
            if (text != null && text.gameObject != null)
            {
                Destroy(text.gameObject);
            }
        }
        questTargetTexts.Clear();
    }
    
    /// <summary>
    /// 更新任务目标进度
    /// </summary>
    public void UpdateTargetProgress(int targetIndex, int currentProgress, int requiredAmount)
    {
        if (targetIndex >= 0 && targetIndex < questTargetTexts.Count)
        {
            // 获取目标名称（从当前文本中解析）
            string currentText = questTargetTexts[targetIndex].text;
            string targetName = currentText.Split(':')[0];
            // 更新文本
            questTargetTexts[targetIndex].text = $"{targetName}: {currentProgress}/{requiredAmount}";
        }
    }
}
