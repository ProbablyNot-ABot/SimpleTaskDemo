using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务数据容器 - 用于存储所有任务数据的 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "QuestDataContainer", menuName = "Quest/Quest Data Container")]
public class QuestDataContainer : ScriptableObject
{
    [Header("任务数据列表")]
    public List<TaskData> allTasks = new List<TaskData>();
}
