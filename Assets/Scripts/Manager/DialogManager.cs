using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 对话管理器 - 处理所有对话相关业务逻辑
/// 职责：管理对话流程、处理对话数据、控制对话 UI
/// </summary>
public class DialogManager : BaseManager<DialogManager>
{
    private DialogPanel currentPanel;
    private DialogData currentDialog;
    private int currentNodeIndex = 0;  // 当前节点索引
    private DialogNode currentNode;   // 当前对话节点
    
    /// <summary>
    /// 开始对话
    /// </summary>
    /// <param name="data">对话数据</param>
    public void StartDialog(DialogData data)
    {
        if (data == null)
        {
            Debug.LogError("DialogData 不能为空");
            return;
        }
        currentDialog = data;
        currentNodeIndex = 0;
        currentNode = currentDialog.GetNode(currentDialog.startNodeId);
        // 打开对话面板
        currentPanel = UIManager.Instance.OpenPanel<DialogPanel>("DialogPanel");
        // 订阅点击事件
        currentPanel.OnClick += HandleDialogClick;
        // 显示第一页对话内容
        if (currentNode != null)
        {
            currentPanel.Show(currentNode.content[0]);
        }
    }
    
    /// <summary>
    /// 处理对话面板点击事件
    /// </summary>
    private void HandleDialogClick()
    {
        if (currentPanel == null || currentDialog == null || currentNode == null) return;
        if (currentPanel.IsTyping)  
            currentPanel.SkipTyping();  // 正在打字 → 跳过
        else    
            NextWord(); // 已显示完 → 进入下一页或显示选项
    }
    
    /// <summary>
    /// 进入下一个对话节点或显示选项
    /// </summary>
    private void NextWord()
    {
        if (currentDialog == null || currentNode == null) return;
        // 检查是否还有下一页
        currentNodeIndex++;
        if (currentNodeIndex < currentNode.content.Count)
        {
            // 还有下一页，显示下一页内容
            currentPanel.Show(currentNode.content[currentNodeIndex]);
        }
        else
        {
            // 所有对话页都显示完了，显示选项
            ShowBranchSelection();
        }
    }
    
    /// <summary>
    /// 显示分支选项
    /// </summary>
    private void ShowBranchSelection()
    {
        if (currentDialog == null || currentNode == null) return;
        
        // 过滤出符合条件的分支选项
        var validOptions = new List<(DialogOption option, int originalIndex)>();
        
        for (int i = 0; i < currentNode.options.Count; i++)
        {
            var option = currentNode.options[i];
            // 步骤 3：条件检查（三层判断）
            bool shouldShow = false;
            if (option.condition == null)
            {
                // 没有条件对象 → 显示
                shouldShow = true;
            }
            else if (!option.condition.hasCondition)
            {
                // 明确标记为无条件 → 显示
                shouldShow = true;
            }
            else
            {
                // 有条件，检查是否满足
                shouldShow = CheckCondition(option.condition);
            }
            // 步骤 4：加入有效选项
            if (shouldShow)
            {
                validOptions.Add((option, i));  // 保存选项和原始索引
                Debug.Log($"选项 '{option.optionText}' 条件满足，显示");
            }
            else
            {
                Debug.Log($"选项 '{option.optionText}' 条件不满足，隐藏");
            }
        }
        // 步骤 5：显示选项
        if (validOptions.Count > 0)
        {
            // 提取选项文字
            var optionTexts = new List<string>();
            foreach (var validOption in validOptions)
            {
                optionTexts.Add(validOption.option.optionText);
            }
            
            // 显示选项按钮，绑定回调
            currentPanel.ShowOptions(optionTexts, (index) => 
            {
                // 点击时，使用原始索引
                HandleBranchOptionSelected(validOptions[index].originalIndex);
            });
        }
        else
        {
            // 没有有效选项，直接结束对话
            Debug.Log("没有可用选项，结束对话");
            EndDialog();
        }
    }

    private bool CheckCondition(DialogCondition condition)
    {
        if (QuestManager.Instance.AllQuestTasks[int.Parse(condition.questId)-1].taskStatus == 
            condition.requiredState)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 处理选项选择
    /// </summary>
    private void HandleBranchOptionSelected(int optionIndex)
    {
        if (currentNode.options == null) return;    
        var selectedOption = currentNode.options[optionIndex];
        if (selectedOption.action.hasAction)
        {
            if (selectedOption.action.type == DialogAction.ActionType.OpenShop)
                OpenShop(selectedOption.action.shopPrefab);
            if (selectedOption.action.type == DialogAction.ActionType.StartQuest)
                StartQuest(int.Parse(selectedOption.action.questId));
            if (selectedOption.action.type == DialogAction.ActionType.CompleteQuest)
                CompleteQuest(int.Parse(selectedOption.action.questId));
        }
        else{
            if(selectedOption.nextNodeId != null)
            {
                GotoNode(selectedOption.nextNodeId);
            }
        }
        Debug.Log($"选择了选项 {optionIndex}: {selectedOption}");
    }

    private void CompleteQuest(int questId)
    {
        QuestManager.Instance.CompleteQuest(questId);
        // 检查当前选项是否有下一个节点        
        var selectedOption = currentNode.options[currentNodeIndex+1];
        if (selectedOption.nextNodeId != null)
        {
            // 有下一个节点，进入下一个节点
            GotoNode(selectedOption.nextNodeId);
            return;
        }
        // 没有下一个节点，关闭对话
        EndDialog();
    }

    private void StartQuest(int questId)
    {
        QuestManager.Instance.StartQuest(questId);
        EndDialog();
    }

    private void OpenShop(GameObject shopPrefab)
    {
        UIManager.Instance.OpenPanel<ShopPanel>(shopPrefab.name);
        EndDialog();
    }

    private void GotoNode(string nextNodeId)
    {
        if (currentDialog == null) return;
        
        var nextNode = currentDialog.GetNode(nextNodeId);
        if (nextNode == null)
        {
            Debug.Log($"找不到节点: {nextNodeId}，默认结束对话");
            EndDialog();
            return;
        }
        
        currentNode = nextNode;
        currentNodeIndex = 0;
        currentPanel.Show(currentNode.content[0]);
    }

    /// <summary>
    /// 结束对话
    /// </summary>
    private void EndDialog()
    {
        if (currentPanel != null)
        {
            currentPanel.OnClick -= HandleDialogClick;
            currentPanel.Close();
            currentPanel = null;
        }
        currentDialog = null;
        currentNodeIndex = 0;
    }
    
    /// <summary>
    /// 强制关闭对话（外部调用）
    /// </summary>
    public void ForceClose()
    {
        EndDialog();
    }
    
    /// <summary>
    /// 当前是否有对话正在进行
    /// </summary>
    public bool IsDialogActive => currentPanel != null;
    
    /// <summary>
    /// 当对话框被外部关闭时调用（如 CloseAllPanels）
    /// </summary>
    public void OnDialogPanelClosed()
    {
        if (currentPanel != null)
        {
            currentPanel.OnClick -= HandleDialogClick;
            currentPanel = null;
            currentDialog = null;
            currentNodeIndex = 0;
        }
    }
}
