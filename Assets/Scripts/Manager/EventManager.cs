using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局事件系统，解耦 UI 和业务逻辑
/// </summary>
public class EventManager : BaseManager<EventManager>
{
    private Dictionary<string, Delegate> events = new Dictionary<string, Delegate>();
    
    // ޲�������
    public void AddListener(string eventName, Action handler)
    {
        if (events.ContainsKey(eventName))
            events[eventName] = (Action)events[eventName] + handler;
        else
            events[eventName] = handler;
    }

    // ��һ����������
    public void AddListener<T>(string eventName, Action<T> handler)
    {
        if (events.ContainsKey(eventName))
            events[eventName] = (Action<T>)events[eventName] + handler;
        else
            events[eventName] = handler;
    }

    // �����޲����¼�
    public void Send(string eventName)
    {
        if (events.TryGetValue(eventName, out Delegate d))
            (d as Action)?.Invoke();
    }

    // ���ʹ������¼�
    public void Send<T>(string eventName, T arg)
    {
        if (events.TryGetValue(eventName, out Delegate d))
            (d as Action<T>)?.Invoke(arg);
    }

    // �Ƴ�����
    public void RemoveListener(string eventName, Action handler)
    {
        if (events.TryGetValue(eventName, out Delegate d))
        {
            d = (Action)d - handler;
            if (d == null)
                events.Remove(eventName);
            else
                events[eventName] = d;
        }
    }

    public void RemoveListener<T>(string eventName, Action<T> handler)
    {
        if (events.TryGetValue(eventName, out Delegate d))
        {
            d = (Action<T>)d - handler;
            if (d == null)
                events.Remove(eventName);
            else
                events[eventName] = d;
        }
    }
}