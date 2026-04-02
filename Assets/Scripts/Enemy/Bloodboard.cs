using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bloodboard : MonoBehaviour
{
    private Camera mainCamera;
    public Image healthBarImage;

    void Start()
    {
        mainCamera = Camera.main;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }

    public void SetHealth(int currentHealth, int health) 
    {
        healthBarImage.fillAmount = (float)currentHealth / health;
        if (currentHealth <= 0) ObjectPoolManager.Instance.ReturnObject(gameObject, "EnemyBlood");
    }
}
