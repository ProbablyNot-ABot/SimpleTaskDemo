using UnityEngine;
using System.Collections;

/// <summary>
/// 完整子弹发射器 - 包含枪口火焰、子弹本体、拖尾效果、命中特效
/// </summary>
public class SimpleBulletShooter : MonoBehaviour
{
    [Header("子弹配置")]
    [SerializeField] private GameObject muzzlePrefab;        // 枪口火焰粒子效果
    [SerializeField] private GameObject bulletPrefab;        // 子弹本体粒子效果
    [SerializeField] private GameObject hitEffectPrefab;     // 命中目标时的特效
    [SerializeField] private Transform firePoint;            // 发射点
    [SerializeField] private float bulletSpeed = 20f;        // 子弹飞行速度
    
    private System.Action onBulletHitCallback;  // 子弹命中回调
    
    /// <summary>
    /// 向目标敌人发射子弹
    /// </summary>
    /// <param name="targetEnemy">目标敌人</param>
    /// <param name="onHit">子弹命中时的回调</param>
    public void Shoot(GameObject targetEnemy, System.Action onHit = null)
    {
        if (bulletPrefab == null || targetEnemy == null)
        {
            Debug.LogWarning("子弹预制体或目标敌人为空！");
            return;
        }
        // 设置回调
        onBulletHitCallback = onHit;
        // 确定发射点
        Transform spawnPoint = firePoint != null ? firePoint : transform;
        
        // 计算发射方向
        Vector3 direction = (targetEnemy.transform.position - spawnPoint.position).normalized;
        
        // 计算旋转（让子弹朝向目标）
        Quaternion rotation = Quaternion.LookRotation(direction);
        
        // 1. 从对象池获取枪口火焰
        if (muzzlePrefab != null)
        {
            GameObject muzzle = ObjectPoolManager.Instance.GetObject(muzzlePrefab.name, spawnPoint.position, rotation);
            // 使用协程回收枪口火焰
            StartCoroutine(ReturnToPoolAfterDelay(muzzle, muzzlePrefab.name, 0.1f));
        }
        
        // 2. 从对象池获取子弹本体
        GameObject bullet = ObjectPoolManager.Instance.GetObject(bulletPrefab.name, spawnPoint.position, rotation);
        
        // 启动协程控制子弹飞行
        StartCoroutine(MoveBulletCoroutine(bullet, targetEnemy.transform.position, direction));
    }
    
    /// <summary>
    /// 子弹飞行协程
    /// </summary>
    private IEnumerator MoveBulletCoroutine(GameObject bullet, Vector3 targetPosition, Vector3 direction)
    {
        Vector3 startPosition = bullet.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float timer = 0f;
        
        // 子弹飞行
        while (timer < distance / bulletSpeed)
        {
            timer += Time.deltaTime;
            bullet.transform.position += direction * bulletSpeed * Time.deltaTime;
            
            yield return null;
        }
        
        // 确保子弹到达目标位置
        bullet.transform.position = targetPosition;
        
        // 4. 从对象池获取命中特效
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = ObjectPoolManager.Instance.GetObject(hitEffectPrefab.name, targetPosition, Quaternion.LookRotation(direction));
            // 使用协程回收命中特效
            StartCoroutine(ReturnToPoolAfterDelay(hitEffect, hitEffectPrefab.name, 0.5f));
        }
        
        // 5. 调用命中回调（造成伤害）
        onBulletHitCallback?.Invoke();
        onBulletHitCallback = null;  // 清空回调
        
        // 回收子弹到对象池
        ObjectPoolManager.Instance.ReturnObject(bullet, bulletPrefab.name);
    }
    
    /// <summary>
    /// 延迟回收对象到对象池
    /// </summary>
    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, string prefabName, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (obj != null)
        {
            ObjectPoolManager.Instance.ReturnObject(obj, prefabName);
        }
    }
}
