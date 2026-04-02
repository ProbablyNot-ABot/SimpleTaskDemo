#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 任务数据加载器 - 编辑器扩展工具
/// 用于在编辑器模式下加载所有 TaskData 到 QuestDataContainer
/// </summary>
public class QuestDataLoader : EditorWindow
{
    [MenuItem("Tools/Quest/Load Quest Data to Container")]
    public static void LoadQuestData()
    {
        // 使用 AssetDatabase 加载所有 TaskData
        string[] guids = AssetDatabase.FindAssets("t:TaskData", new[] { "Assets/Data/Quests" });
        
        if (guids.Length == 0)
        {
            Debug.LogWarning($"在 Assets/Data/Quests 目录下未找到任何 TaskData 文件");
            EditorUtility.DisplayDialog("加载失败", "未找到 TaskData 文件", "确定");
            return;
        }
        
        // 加载或创建 QuestDataContainer
        QuestDataContainer container = GetOrCreateContainer();
        
        // 清空现有列表
        container.allTasks.Clear();
        
        // 加载并添加所有任务数据
        int successCount = 0;
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TaskData task = AssetDatabase.LoadAssetAtPath<TaskData>(path);
            
            if (task != null)
            {
                container.allTasks.Add(task);
                Debug.Log($"[QuestLoader] 加载任务：{task.taskName}");
                successCount++;
            }
            else
            {
                Debug.LogWarning($"[QuestLoader] 加载失败：{path}");
            }
        }
        
        // 标记为已修改
        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();
        
        string message = $"成功加载 {successCount} 个任务数据到容器\n路径：Assets/Data/Quests";
        Debug.Log($"[QuestLoader] {message}");
        EditorUtility.DisplayDialog("加载完成", message, "确定");
    }
    
    [MenuItem("Tools/Quest/Clear Quest Data")]
    public static void ClearQuestData()
    {
        QuestDataContainer container = GetContainer();
        
        if (container == null)
        {
            Debug.LogWarning("未找到 QuestDataContainer");
            return;
        }
        
        container.allTasks.Clear();
        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();
        
        Debug.Log("[QuestLoader] 已清空所有任务数据");
        EditorUtility.DisplayDialog("清空完成", "已清空所有任务数据", "确定");
    }
    
    [MenuItem("Tools/Quest/Create Quest Data Container")]
    public static void CreateContainer()
    {
        string path = "Assets/Resources/Data/QuestDataContainer.asset";
        
        // 确保目录存在
        string directory = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }
        
        QuestDataContainer container = ScriptableObject.CreateInstance<QuestDataContainer>();
        AssetDatabase.CreateAsset(container, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"[QuestLoader] 已创建任务数据容器：{path}");
        EditorUtility.DisplayDialog("创建完成", $"任务数据容器已创建在：\n{path}", "确定");
    }
    
    /// <summary>
    /// 获取现有的 QuestDataContainer
    /// </summary>
    private static QuestDataContainer GetContainer()
    {
        string path = "Assets/Resources/Data/QuestDataContainer.asset";
        return AssetDatabase.LoadAssetAtPath<QuestDataContainer>(path);
    }
    
    /// <summary>
    /// 获取或创建 QuestDataContainer
    /// </summary>
    private static QuestDataContainer GetOrCreateContainer()
    {
        QuestDataContainer container = GetContainer();
        
        if (container == null)
        {
            CreateContainer();
            container = GetContainer();
        }
        
        return container;
    }
}
#endif
