using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

/// <summary>
/// 玩家控制器 - 负责玩家操作逻辑
/// 职责：处理玩家输入、移动、寻路、基础交互触发、攻击敌人
/// </summary>
public class PlayerMove : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float npcInteractionDistance = 1.0f;
    private GameObject currentNpc;
    private GameObject currentEnemy;
    private float attackTimer = 0f;  // 攻击计时器
    private SimpleBulletShooter bulletShooter;  // 子弹发射器
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // 从 GameManager 获取玩家移动速度
        if (GameManager.Instance != null && GameManager.Instance.playerData != null)
            agent.speed = GameManager.Instance.playerData.moveSpeed;
        // 获取或添加子弹发射器组件
        bulletShooter = GetComponent<SimpleBulletShooter>();
    }
    
    void Update()
    {
        if (Time.timeScale != 0f)
        {
            CheckDialogTrigger();
            HandleInput();
            CheckEnemyAttack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIManager.Instance.OpenPanel<InventoryPanel>("InventoryPanel");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIManager.Instance.OpenPanel<QuestListPanel>("Quest/QuestListPanel");
        }
    }
    
    /// <summary>
    /// 检查是否到达 NPC 身边，触发对话
    /// </summary>
    void CheckDialogTrigger()
    {
        if (currentNpc != null && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 获取 NPC 上的 NPC 组件
            var npcComponent = currentNpc.GetComponent<NPC>();
            
            // 触发对话（由 DialogManager 处理）
            if (npcComponent != null && npcComponent.dialogData != null)
            {
                DialogManager.Instance.StartDialog(npcComponent.dialogData);
            }
            
            currentNpc = null;
        }
    }
    
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                HandleRaycastHit(hit);
            }
        }
    }
    
    void HandleRaycastHit(RaycastHit hit)
    {
        string tag = hit.collider.CompareTag("Ground") ? "Ground" : 
                     hit.collider.CompareTag("NPC") ? "NPC" : 
                     hit.collider.CompareTag("Enemy") ? "Enemy" : null;
        switch (tag)
        {
            case "Ground":
                // 点击地面时取消当前 NPC 目标
                currentNpc = null;
                currentEnemy = null;
                agent.SetDestination(hit.point);
                break;
            case "NPC":
                // 如果对话框已经打开，不再重复触发
                if (DialogManager.Instance.IsDialogActive)
                {
                    return;
                }
                MoveToNPC(hit.collider.gameObject);
                break;
            case "Enemy":
                AttackEnemy(hit.collider.gameObject);
                break;
        }
        UIManager.Instance.CloseAllPanels();
    }
    
    void MoveToNPC(GameObject npc)
    {
        Vector3 direction = (npc.transform.position - transform.position).normalized;
        Vector3 targetPosition = npc.transform.position - direction * npcInteractionDistance;
        agent.SetDestination(targetPosition);
        currentNpc = npc;
    }
    
    void AttackEnemy(GameObject enemy)
    {
        if (GameManager.Instance == null || GameManager.Instance.playerData == null)
        {
            Debug.LogWarning("GameManager 或 playerData 未初始化，无法攻击");
            return;
        }
        // 检查当前距离
        float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
        // 如果已经在攻击范围内，直接开始攻击，不设置目的地
        if (currentDistance <= GameManager.Instance.playerData.attackRange)
        {
            currentEnemy = enemy;
            return;
        }
        // 否则向敌人移动，到达攻击范围后攻击
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        Vector3 targetPosition = enemy.transform.position - direction * GameManager.Instance.playerData.attackRange;
        agent.SetDestination(targetPosition);
        // 设置目标敌人，在 Update 中检查距离并攻击
        currentEnemy = enemy;
    }

    void CheckEnemyAttack()
    {
        if (currentEnemy != null && GameManager.Instance != null && GameManager.Instance.playerData != null)
        {
            float distance = Vector3.Distance(transform.position, currentEnemy.transform.position);
            // 在攻击范围内
            if (distance <= GameManager.Instance.playerData.attackRange)
            {
                // 攻击计时器累加
                attackTimer += Time.deltaTime;
                // 达到攻击间隔，发动攻击
                if (attackTimer >= GameManager.Instance.playerData.attackSpeed)
                {
                    if (bulletShooter != null)
                    {
                        bulletShooter.Shoot(currentEnemy, () => {
                            GameManager.Instance.AttackEnemy(currentEnemy, GameManager.Instance.playerData.attackDamage);
                        });   
                    }
                    attackTimer = 0f;  // 攻击完成，重置计时器
                }
            }
            else attackTimer = 0f;// 超出攻击范围，重置计时器
        }
    }
    
    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, GameManager.Instance.playerData.attackRange);
        }
    }
}
