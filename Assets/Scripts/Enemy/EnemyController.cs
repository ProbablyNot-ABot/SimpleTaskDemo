using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public int enemyId; // 敌人ID
    public EnemyType enemyType; // 敌人类型
    public EnemyData enemyData;
    private NavMeshAgent agent;
    private GameObject player;
    public Bloodboard enemyHealthBar;
    private Vector3 initialPosition;
    private bool isChasing = false;
    private float chaseTimer = 0f;
    public float chaseTimeout = 2f;
    private int currentHealth;
    private float attackTimer = 0f;
    
    void Start()
    {
        enemyHealthBar = UIManager.Instance.LoadWorldUI<Bloodboard>("EnemyBlood", transform.position);
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");        
        // 保存初始位置
        initialPosition = transform.position;
        // 检查是否设置了EnemyData
        if (enemyData == null)
        {
            Debug.LogError("EnemyData not set for enemy: " + gameObject.name);
            return;
        }
        // 初始化当前生命值
        currentHealth = enemyData.health;
        // 设置移动速度
        agent.speed = enemyData.moveSpeed;
        GameManager.Instance.enemies.Add(this);
    }
    
    private void OnDestroy()
    {
        // 检查 GameManager 是否还存在
        if (GameManager.Instance != null && GameManager.Instance.enemies != null)
        {
            if (GameManager.Instance.enemies.Contains(this))
            {
                GameManager.Instance.enemies.Remove(this);
            }
            // 清除缓存
            GameManager.Instance.RemoveEnemyFromCache(this);
            GameManager.Instance.OnEnemyKilled(this);
        }
    }
    
    void Update()
    {
        if (player != null && enemyData != null)
        {
            ChasePlayer();
        }
        // 更新血条位置，使其始终在敌人头顶正上方
        UpdateHealthBarPosition();
        //Debug.Log($"敌人生命值：{currentHealth}");
    }

    private void UpdateHealthBarPosition()
    {
        if (enemyHealthBar != null)
        {            
            Vector3 healthBarPos = transform.position + transform.up * (transform.localScale.y + 0.5f);
            enemyHealthBar.transform.position = healthBarPos;
        }
    }
    
    void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        
        if (distance <= enemyData.attackRange * 2) // 追击范围是攻击范围的3倍
        {
            // 敌人追踪玩家
            agent.SetDestination(player.transform.position);
            isChasing = true;
            chaseTimer = 0f;
            
            // 检查是否在攻击范围内
            if (distance <= enemyData.attackRange)
            {
                // 更新攻击计时器
                attackTimer += Time.deltaTime;
                
                // 检查是否可以攻击
                if (attackTimer >= enemyData.attackSpeed)
                {
                    // 攻击玩家
                    GameManager.Instance.AttackPlayer(enemyData.damage);
                    attackTimer = 0f;
                }
            }
        }
        else if (isChasing)
        {
            // 玩家离开追击范围，开始计时
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= chaseTimeout)
            {
                // 放弃追踪，返回初始位置
                isChasing = false;
                agent.SetDestination(initialPosition);
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"敌人受到 {damage} 点伤害，剩余生命值：{currentHealth}");
        enemyHealthBar.SetHealth(currentHealth, enemyData.health);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject);
        }
    }
    
    private void OnDrawGizmos()
    {
        // 只在编辑器模式下显示
        if (Application.isEditor && enemyData != null)
        {
            // 绘制敌人攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);
            
            // 绘制敌人追击范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.attackRange * 2);
        }
    }
}