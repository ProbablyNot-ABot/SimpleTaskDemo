using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池管理器 - 管理所有 GameObject 对象池
/// 单例模式，全局访问
/// </summary>
public class ObjectPoolManager : BaseMonoManager<ObjectPoolManager>
{
    /// <summary>
    /// 对象池字典：预制体名称 -> 对象池
    /// </summary>
    private Dictionary<string, GameObjectPool> objectPools = new Dictionary<string, GameObjectPool>();
    
    /// <summary>
    /// 对象池的父容器字典：预制体名称 -> 父容器
    /// </summary>
    private Dictionary<string, Transform> poolContainers = new Dictionary<string, Transform>();
    
    /// <summary>
    /// 获取对象池中的对象
    /// </summary>
    /// <param name="prefabName">预制体名称（从 Resources 加载）</param>
    /// <param name="parent">父节点（可选）</param>
    /// <returns>激活的对象实例</returns>
    public GameObject GetObject(string prefabName, Transform parent = null)
    {
        // 确保对象池存在
        if (!objectPools.ContainsKey(prefabName))
        {
            CreatePool(prefabName, parent);
        }        
        // 从对象池获取对象
        GameObject obj = objectPools[prefabName].Get();   
        // 激活对象
        obj.SetActive(true);        
        Debug.Log($"[ObjectPool] 获取对象：{prefabName}，池中剩余：{objectPools[prefabName].InactiveCount}");        
        return obj;
    }
    
    /// <summary>
    /// 获取对象池中的对象（带位置和旋转）
    /// </summary>
    /// <param name="prefabName">预制体名称</param>
    /// <param name="position">位置</param>
    /// <param name="rotation">旋转</param>
    /// <param name="parent">父节点（可选）</param>
    /// <returns>激活的对象实例</returns>
    public GameObject GetObject(string prefabName, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject obj = GetObject(prefabName, parent);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }
    
    /// <summary>
    /// 回收对象到对象池
    /// </summary>
    /// <param name="obj">要回收的对象</param>
    /// <param name="prefabName">预制体名称（用于找到对应的池）</param>
    public void ReturnObject(GameObject obj, string prefabName)
    {
        if (obj == null)
        {
            Debug.LogWarning("[ObjectPool] 尝试回收空对象");
            return;
        }
        
        // 检查对象池是否存在
        if (!objectPools.ContainsKey(prefabName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池 {prefabName} 不存在，直接销毁对象");
            Destroy(obj);
            return;
        }
        
        // 回收到对象池
        objectPools[prefabName].Return(obj);        
        Debug.Log($"[ObjectPool] 回收对象：{prefabName}，池中数量：{objectPools[prefabName].InactiveCount}");
    }
    
    /// <summary>
    /// 回收对象到对象池（自动从对象名推断池名）
    /// </summary>
    /// <param name="obj">要回收的对象</param>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[ObjectPool] 尝试回收空对象");
            return;
        }
        
        // 尝试从对象名推断池名（移除可能的位置后缀）
        string poolName = GetPoolNameFromObjectName(obj.name);
        ReturnObject(obj, poolName);
    }
    
    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="prefabName">预制体名称</param>
    /// <param name="defaultParent">默认父节点（可选）</param>
    private void CreatePool(string prefabName, Transform defaultParent = null)
    {
        // 加载预制体（支持多种路径）
        GameObject prefab = null;
        
        // 尝试 1: UI 路径
        if (prefab == null)
            prefab = Resources.Load<GameObject>($"UI/{prefabName}");
        
        // 尝试 2: 敌人路径
        if (prefab == null)
            prefab = Resources.Load<GameObject>($"Enemies/{prefabName}");
        
        // 尝试 3: 特效路径
        if (prefab == null)
            prefab = Resources.Load<GameObject>($"Effects/{prefabName}");
        
        // 尝试 4: 直接加载（根目录）
        if (prefab == null)
            prefab = Resources.Load<GameObject>(prefabName);
        
        if (prefab == null)
        {
            Debug.LogError($"[ObjectPool] 无法加载预制体：{prefabName}");
            return;
        }
        
        // 创建对象池容器
        Transform container = GetPoolContainer(prefabName, defaultParent);
        
        // 创建对象池
        GameObjectPool pool = new GameObjectPool(prefab, container);
        objectPools[prefabName] = pool;
        
        Debug.Log($"[ObjectPool] 创建对象池：{prefabName}");
    }
    
    /// <summary>
    /// 获取或创建对象池容器
    /// </summary>
    /// <param name="prefabName">预制体名称</param>
    /// <param name="defaultParent">默认父节点（可选）</param>
    /// <returns>对象池容器</returns>
    private Transform GetPoolContainer(string prefabName, Transform defaultParent = null)
    {
        // 如果容器已存在，直接返回
        if (poolContainers.TryGetValue(prefabName, out Transform existingContainer))
        {
            return existingContainer;
        }
        
        // 创建容器名称（大写开头 + s 结尾）
        string containerName = FormatContainerName(prefabName);
        
        // 查找是否已存在同名容器
        GameObject existingObj = GameObject.Find(containerName);
        if (existingObj != null)
        {
            poolContainers[prefabName] = existingObj.transform;
            return existingObj.transform;
        }

        // 创建新容器
        GameObject container = new GameObject(containerName);
        
        // 设置父节点
        if (defaultParent != null)
        {
            container.transform.SetParent(defaultParent);
        }
        else
        {
            container.transform.SetParent(transform);
        }
        
        poolContainers[prefabName] = container.transform;        
        Debug.Log($"[ObjectPool] 创建对象池容器：{containerName}，父节点：{(defaultParent != null ? defaultParent.name : "ObjectPoolManager")}");        
        return container.transform;
    }
    
    /// <summary>
    /// 格式化容器名称（大写开头 + s 结尾）
    /// </summary>
    /// <param name="name">原始名称</param>
    /// <returns>格式化后的名称</returns>
    private string FormatContainerName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "Pools";
        
        // 首字母大写
        string formatted = char.ToUpper(name[0]) + (name.Length > 1 ? name.Substring(1) : "");
        
        // 如果已经以 s 结尾，直接返回
        if (formatted.EndsWith("s"))
            return formatted;
        
        // 添加 s 后缀
        return formatted + "s_Pool";
    }
    
    /// <summary>
    /// 从对象名推断池名称
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <returns>池名称</returns>
    private string GetPoolNameFromObjectName(string objectName)
    {
        // 移除可能的位置后缀（如 "(Clone)"）
        int cloneIndex = objectName.IndexOf("(Clone)");
        if (cloneIndex >= 0)
        {
            objectName = objectName.Substring(0, cloneIndex).Trim();
        }
        
        // 移除可能的数字后缀
        while (objectName.Length > 0 && char.IsDigit(objectName[objectName.Length - 1]))
        {
            objectName = objectName.Substring(0, objectName.Length - 1);
        }
        
        return objectName;
    }
    
    /// <summary>
    /// 清除对象池中的所有对象
    /// </summary>
    /// <param name="prefabName">预制体名称</param>
    public void ClearPool(string prefabName)
    {
        if (objectPools.ContainsKey(prefabName))
        {
            objectPools[prefabName].Clear();
            Debug.Log($"[ObjectPool] 清除对象池：{prefabName}");
        }
    }
    
    /// <summary>
    /// 清除所有对象池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in objectPools.Values)
        {
            pool.Clear();
        }
        objectPools.Clear();
        Debug.Log("[ObjectPool] 清除所有对象池");
    }
    
    /// <summary>
    /// 获取对象池信息
    /// </summary>
    /// <param name="prefabName">预制体名称</param>
    /// <returns>对象池信息（总数量、激活数量、未激活数量）</returns>
    public PoolInfo GetPoolInfo(string prefabName)
    {
        if (objectPools.ContainsKey(prefabName))
        {
            return new PoolInfo
            {
                totalCount = objectPools[prefabName].TotalCount,
                activeCount = objectPools[prefabName].ActiveCount,
                inactiveCount = objectPools[prefabName].InactiveCount
            };
        }
        return null;
    }
}

/// <summary>
/// GameObject 对象池
/// </summary>
public class GameObjectPool
{
    private GameObject prefab;
    private Transform container;
    private Stack<GameObject> inactiveObjects = new Stack<GameObject>();
    private HashSet<GameObject> activeObjects = new HashSet<GameObject>();
    
    public GameObjectPool(GameObject prefab, Transform container)
    {
        this.prefab = prefab;
        this.container = container;
    }
    
    /// <summary>
    /// 获取对象
    /// </summary>
    /// <param name="parent">父节点（可选）</param>
    public GameObject Get()
    {
        GameObject obj;
        // 尝试从池中获取
        if (inactiveObjects.Count > 0)
        {
            obj = inactiveObjects.Pop();
        }
        else
        {
            // 池中没有可用对象，创建新的
            obj = GameObject.Instantiate(prefab, container);
            obj.name = prefab.name;
        }
        obj.transform.SetParent(container);
        // 添加到激活集合
        activeObjects.Add(obj);
        return obj;
    }
    
    /// <summary>
    /// 回收对象
    /// </summary>
    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[ObjectPool] 尝试回收空对象");
            return;
        }
        
        // 从激活集合移除
        if (!activeObjects.Remove(obj))
            Debug.LogWarning("[ObjectPool] 回收的对象不在激活集合中");
        
        // 失活对象
        obj.SetActive(false);
        
        // 重新设置父节点为池容器
        obj.transform.SetParent(container);
        
        // 放回池中
        inactiveObjects.Push(obj);
    }
    
    /// <summary>
    /// 清除池中所有对象
    /// </summary>
    public void Clear()
    {
        foreach (var obj in inactiveObjects)
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }
        inactiveObjects.Clear();
        foreach (var obj in activeObjects)
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }
        activeObjects.Clear();
    }
    
    /// <summary>
    /// 总对象数量
    /// </summary>
    public int TotalCount => activeObjects.Count + inactiveObjects.Count;
    
    /// <summary>
    /// 激活的对象数量
    /// </summary>
    public int ActiveCount => activeObjects.Count;
    
    /// <summary>
    /// 未激活的对象数量
    /// </summary>
    public int InactiveCount => inactiveObjects.Count;
}

/// <summary>
/// 对象池信息
/// </summary>
public class PoolInfo
{
    public int totalCount;      // 总数量
    public int activeCount;     // 激活数量
    public int inactiveCount;   // 未激活数量
}
