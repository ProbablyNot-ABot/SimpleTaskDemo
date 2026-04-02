using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
    Zombie,
    Slime
}

public class GameManager : BaseMonoManager<GameManager>
{
    [Header("玩家数据配置")]
    public string playerName = "Player";
    public int playerHealth = 100;
    public int playerMoney = 5000;
    public float playerAttackSpeed = 1.0f;
    public float playerMoveSpeed = 5.0f;
    public int playerAttackDamage = 10;
    public float playerAttackRange = 2.0f;
    public PlayerData playerData;
    public Image playerHealthBar;
    public List<EnemyController> enemies;
    private bool isPlayerDead = false;
    
    /// <summary>
    /// 敌人组件缓存字典，避免重复 GetComponent
    /// </summary>
    private readonly Dictionary<GameObject, EnemyController> enemyCache = new();
    
    protected override void OnStart()
    { 
        base.OnStart();
        // 初始化玩家数据（使用 Inspector 中配置的值）
        playerData = ScriptableObject.CreateInstance<PlayerData>();
        playerData.name = playerName;
        playerData.health = playerHealth;
        playerData.money = playerMoney;
        playerData.attackSpeed = playerAttackSpeed;
        playerData.moveSpeed = playerMoveSpeed;
        playerData.attackDamage = playerAttackDamage;
        playerData.attackRange = playerAttackRange;
        // 初始化玩家生命值条
        playerHealthBar.fillAmount = playerData.health / 100f;
        // 初始化敌人数据
        enemies = new List<EnemyController>();
        // 初始化背包
        InventoryManager.Instance.Initialize();
        // 初始化任务系统
        QuestManager.Instance.Init();
    }

    public void AttackEnemy(GameObject enemy, int damage)
    {
        // 处理玩家攻击敌人的逻辑
        EnemyController enemyController = GetEnemyController(enemy);
        if (enemyController != null)
        {
            enemyController.TakeDamage(damage);
        }
    }

    public void OnEnemyKilled(EnemyController enemyController)
    {
        // 检查当前已接任务是否有击杀该敌人的目标
        var activeQuests = QuestManager.Instance.ReceivedQuestTasks;
        foreach (var quest in activeQuests)
        {
            if (quest.taskTargets != null)
            {
                foreach (var target in quest.taskTargets)
                {
                    if (target.taskType == TaskType.KillEnemy && 
                        GetFirstDigit(target.targetID) == GetFirstDigit(enemyController.enemyId))
                    {
                        // 更新任务进度
                        QuestManager.Instance.UpdateQuestProgress(target);
                    }
                }
            }
        }
    }

    private int GetFirstDigit(int number)
    {
        number = Mathf.Abs(number);  // 确保是正数
        while (number >= 10) {number /= 10;}
        return number;
    }
    
    /// <summary>
    /// 获取敌人控制器（带缓存）
    /// </summary>
    private EnemyController GetEnemyController(GameObject enemy)
    {
        if (enemy == null)
            return null;
        // 尝试从缓存获取
        if (enemyCache.TryGetValue(enemy, out EnemyController cached))
            return cached;
        // 缓存中没有，获取并缓存
        if (enemy.TryGetComponent<EnemyController>(out var controller))
            enemyCache[enemy] = controller;
        return controller;
    }
    
    /// <summary>
    /// 清除敌人缓存（当敌人被销毁时调用）
    /// </summary>
    public void RemoveEnemyFromCache(EnemyController enemyController)
    {
        if (enemyController == null)
            return;
        var keysToRemove = new List<GameObject>();
        foreach (var kvp in enemyCache)
        {
            if (kvp.Value == enemyController)
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove)
        {
            enemyCache.Remove(key);
        }
    }
    
    public void AttackPlayer(int damage)
    {
        if (isPlayerDead)
        {
            return;
        }
        playerData.health -= damage;
        Debug.Log("玩家受到了" + damage + " 伤害。当前生命值：" + playerData.health);
        // 更新玩家生命值条
        playerHealthBar.fillAmount = playerData.health / 100.0f;
        if (playerData.health <= 0)
        {
            playerData.health = 0;
            isPlayerDead = true;
            Debug.Log("玩家死亡!");
        }
    }
}