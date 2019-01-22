using System.Collections.Generic;
using UnityEngine;

public class NotificationCenter : MonoBehaviour
{
    public static NotificationCenter Instance = null;

    private Dictionary<NotificationType, NotificationHandler> handlers = new Dictionary<NotificationType, NotificationHandler>();
    private object thisLock = new object();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void AddEventListener(NotificationType type, NotificationHandler listener)
    {
        if (handlers.ContainsKey(type))
        {
            handlers[type] += listener;
        }
        else
        {
            handlers.Add(type, listener);
        }
    }

    public void PushEvent(NotificationType type, object arg)
    {
        lock (thisLock)
        {
            if (!handlers.ContainsKey(type))
            {
                return;
            }

            if (handlers[type] != null)
            {
                handlers[type](new NotificationArg(arg));
            }
        }
    }
}