using System;
using System.Collections.Generic;


using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance = null;

    public string host = "127.0.0.1";
    public int port = 18787;

    public Queue<Message> m_msgQueue = new Queue<Message>();
    // for queue
    private object thisLock = new object();

    private GameSocket m_gameSocket;

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


        m_gameSocket = new GameSocket(OnConnected, OnDisconnect, OnMessage);
    }

    void Update()
    {
        lock (thisLock)
        {
            if (m_msgQueue.Count > 0)
            {
                Message msg = m_msgQueue.Dequeue();
                HandleMessage(msg);
            }
        }
    }

    private void HandleMessage(Message msg)
    {
        var msgID = (MsgID)msg.MessageID;
        switch (msgID)
        {
            case MsgID.Response_Join:
                {
                    ResponseJoin data = new ResponseJoin();
                    data.FromMessage(msg);
                    NotificationCenter.Instance.PushEvent(NotificationType.Network_OnResponseJoin, data);
                    break;
                }


            case MsgID.Broadcast_Move:
                {
                    BroadcastMove data = new BroadcastMove();
                    data.FromMessage(msg);
                    NotificationCenter.Instance.PushEvent(NotificationType.Network_OnBroadcastMove, data);
                    break;
                }

            case MsgID.Broadcast_Join:
                {
                    BroadcastJoin data = new BroadcastJoin();
                    data.FromMessage(msg);
                    NotificationCenter.Instance.PushEvent(NotificationType.Network_OnBroadcastJoin, data);
                    break;
                }

            case MsgID.Broadcast_Leave:
                {
                    BroadcastLeave data = new BroadcastLeave();
                    data.FromMessage(msg);
                    NotificationCenter.Instance.PushEvent(NotificationType.Network_OnBroadcastLeave, data);
                    break;
                }
        }
    }

    /// <summary>
    /// Connect to server.
    /// </summary>
    public void Connect()
    {
        TimeSpan timeout = new TimeSpan(0, 0, 5);
        m_gameSocket.Connect(host, port, timeout);
    }

    /// <summary>
    /// Sends message.
    /// </summary>
    /// <param name="msg">Message.</param>
    public void SendMessage(Message msg)
    {
        if (m_gameSocket != null)
        {
            m_gameSocket.PushMessage(msg);
        }
    }

    public void SendJoin()
    {
        var msg = new RequestJoin().ToMessage();
        SendMessage(msg);
    }

    public void SendMove(int x, int y, string playerID)
    {
        var msg = new RequestMove();
        msg.playerID = playerID;
        msg.x = x;
        msg.y = y;

        SendMessage(msg.ToMessage());
    }

    public void OnConnected(Message msg)
    {
        NotificationCenter.Instance.PushEvent(NotificationType.Network_OnConnected, null);
    }

    public void OnDisconnect(Message msg)
    {
        m_gameSocket.Reset();
        NotificationCenter.Instance.PushEvent(NotificationType.Network_OnDisconnected, null);
    }

    public void OnMessage(Message msg)
    {
        lock (thisLock)
        {
            m_msgQueue.Enqueue(msg);
        }
    }

    private void OnDestroy()
    {
        m_gameSocket.Reset();
    }
}
