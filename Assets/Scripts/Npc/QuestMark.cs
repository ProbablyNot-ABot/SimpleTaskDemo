using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMark : MonoBehaviour
{
    public Image wenhaoImage;
    public Image tanhaoImage;

    public void SetMark(TaskStatus status)
    {
        switch (status)
        {
            case TaskStatus.CanReceive:
                wenhaoImage.gameObject.SetActive(true);
                tanhaoImage.gameObject.SetActive(false);
                break;
            case TaskStatus.InProgress:
                wenhaoImage.gameObject.SetActive(false);
                tanhaoImage.gameObject.SetActive(false);
                break;
            case TaskStatus.Completed:
                wenhaoImage.gameObject.SetActive(false);
                tanhaoImage.gameObject.SetActive(true);
                break;
            default:
                wenhaoImage.gameObject.SetActive(false);
                tanhaoImage.gameObject.SetActive(false);
                break;
        }
    }
}
