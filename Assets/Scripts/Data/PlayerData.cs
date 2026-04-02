using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public int level;
    public int health;
    public int money;
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float moveSpeed;
    public List<TaskData> tasks;
}
