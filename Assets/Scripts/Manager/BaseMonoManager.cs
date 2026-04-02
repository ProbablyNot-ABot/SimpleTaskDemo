/// <summary>
/// 管理器基类 - 提供单例模式和生命周期管理（Mono 版本）
/// </summary>
using UnityEngine;

public abstract class BaseMonoManager<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType<T>();
            }
            return instance;
        }

    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        this.OnStart();
    }

    protected virtual void OnStart() {  }
}