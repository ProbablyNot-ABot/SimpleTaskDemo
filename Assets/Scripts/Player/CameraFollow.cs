using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    private Vector3 offset;
    
    void Start()
    {
        // 计算相机与玩家的初始偏移量
        offset = transform.position - player.position;
    }
    
    void Update()
    {
        // 保持相机的角度和朝向不变，只更新x和z位置
        Vector3 newPosition = new Vector3(player.position.x + offset.x, transform.position.y, player.position.z + offset.z);
        transform.position = newPosition;
    }
}