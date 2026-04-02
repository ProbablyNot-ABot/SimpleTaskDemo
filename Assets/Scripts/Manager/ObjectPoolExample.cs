using UnityEngine;

/// <summary>
/// 对象池使用示例
/// 演示如何使用 ObjectPoolManager 管理 GameObject
/// </summary>
public class ObjectPoolExample : MonoBehaviour
{
    [Header("示例配置")]
    [Tooltip("预制体名称（从 Resources/UI 加载）")]
    public string prefabName = "QuestMark";
    
    [Tooltip("预生成数量")]
    public int preloadCount = 5;
    
    private void Start()
    {
        // 示例 1: 预加载对象池
        PreloadPool();
        
        // 示例 2: 获取对象
        SpawnExample();
        
        // 示例 3: 回收对象示例（3 秒后）
        Invoke(nameof(ReturnExample), 3f);
    }
    
    /// <summary>
    /// 预加载对象池
    /// </summary>
    private void PreloadPool()
    {
        Debug.Log($"=== 预加载对象池：{prefabName} ===");
        
        // 预先生成多个对象并回收，填充对象池
        for (int i = 0; i < preloadCount; i++)
        {
            // 获取对象
            GameObject obj = ObjectPoolManager.Instance.GetObject(prefabName);
            obj.name = $"{prefabName}_{i}";
            
            // 立即回收（模拟使用完毕后回收）
            ObjectPoolManager.Instance.ReturnObject(obj, prefabName);
        }
        
        // 查看对象池信息
        PoolInfo info = ObjectPoolManager.Instance.GetPoolInfo(prefabName);
        if (info != null)
        {
            Debug.Log($"对象池信息 - 总数：{info.totalCount}, 激活：{info.activeCount}, 未激活：{info.inactiveCount}");
        }
    }
    
    /// <summary>
    /// 获取对象示例
    /// </summary>
    private void SpawnExample()
    {
        Debug.Log("=== 获取对象示例 ===");
        
        // 方式 1: 简单获取（使用默认位置）
        GameObject obj1 = ObjectPoolManager.Instance.GetObject(prefabName);
        obj1.transform.position = new Vector3(0, 0, 0);
        Debug.Log($"创建对象 1: {obj1.name}");
        
        // 方式 2: 指定位置和旋转
        GameObject obj2 = ObjectPoolManager.Instance.GetObject(
            prefabName, 
            new Vector3(1, 0, 0), 
            Quaternion.identity
        );
        Debug.Log($"创建对象 2: {obj2.name}");
        
        // 方式 3: 指定父节点
        GameObject obj3 = ObjectPoolManager.Instance.GetObject(prefabName, transform);
        obj3.transform.localPosition = new Vector3(2, 0, 0);
        Debug.Log($"创建对象 3: {obj3.name}");
        
        // 查看对象池信息
        PoolInfo info = ObjectPoolManager.Instance.GetPoolInfo(prefabName);
        Debug.Log($"对象池信息 - 总数：{info.totalCount}, 激活：{info.activeCount}, 未激活：{info.inactiveCount}");
    }
    
    /// <summary>
    /// 回收对象示例
    /// </summary>
    private void ReturnExample()
    {
        Debug.Log("=== 回收对象示例 ===");
        
        // 查找所有激活的对象
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Untagged"); // 根据实际情况修改
        
        // 回收对象（示例中回收所有名为 prefabName 开头的对象）
        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith(prefabName))
            {
                // 方式 1: 手动指定池名称
                ObjectPoolManager.Instance.ReturnObject(obj, prefabName);
                Debug.Log($"回收对象：{obj.name}");
                
                // 方式 2: 自动推断池名称（推荐）
                // ObjectPoolManager.Instance.ReturnObject(obj);
            }
        }
        
        // 查看对象池信息
        PoolInfo info = ObjectPoolManager.Instance.GetPoolInfo(prefabName);
        Debug.Log($"对象池信息 - 总数：{info.totalCount}, 激活：{info.activeCount}, 未激活：{info.inactiveCount}");
    }
    
    /// <summary>
    /// 实际使用示例：发射子弹
    /// </summary>
    public void ShootBullet()
    {
        // 从对象池获取子弹
        GameObject bullet = ObjectPoolManager.Instance.GetObject(
            "BulletEffect", 
            transform.position, 
            transform.rotation
        );
        
        // 设置子弹方向等逻辑
        // ...
        
        // 子弹命中后回收
        // 在子弹脚本中调用：
        // ObjectPoolManager.Instance.ReturnObject(gameObject, "BulletEffect");
    }
    
    /// <summary>
    /// 实际使用示例：生成敌人
    /// </summary>
    public void SpawnEnemy()
    {
        // 从对象池获取敌人
        GameObject enemy = ObjectPoolManager.Instance.GetObject(
            "Enemy_Zombie",
            new Vector3(10, 0, 10),
            Quaternion.identity
        );
        
        // 敌人死亡后回收
        // 在敌人脚本的 OnDestroy 或死亡逻辑中调用：
        // ObjectPoolManager.Instance.ReturnObject(gameObject, "Enemy_Zombie");
    }
}
