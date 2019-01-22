using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System;
using UnityEngine;

public delegate void NetworkEventDelegate(Message msg);


public class GameSocket
{
    private Socket m_socket;
    private bool m_isConnected;
    // 消息发送线程
    private Thread t_send;
    // 消息接收线程
    private Thread t_recive;
    // 用于发送Message的队列
    private Queue<Message> m_sendQueue = new Queue<Message>();
    private object m_lock = new System.Object();

    private readonly NetworkEventDelegate onConnected;
    private readonly NetworkEventDelegate onDisonnected;
    private readonly NetworkEventDelegate onMessage;

    public GameSocket(NetworkEventDelegate onConnected, NetworkEventDelegate onDisonnected, NetworkEventDelegate onMessage)
    {
        this.onConnected = onConnected;
        this.onDisonnected = onDisonnected;
        this.onMessage = onMessage;

        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_isConnected = false;
    }

    public void Connect(string host, int port, TimeSpan timeout)
    {

        if (m_socket == null)
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        if (m_isConnected)
        {
            throw new ApplicationException("socket has already connected.");
        }

        IAsyncResult result = m_socket.BeginConnect(host, port, new AsyncCallback(OnConnected), m_socket);

        result.AsyncWaitHandle.WaitOne(timeout, true);
    }

    private void OnConnected(IAsyncResult result)
    {
        Socket s = result.AsyncState as Socket;
        if (s.Connected)
        {
            m_isConnected = true;
            m_socket.EndConnect(result);

            StartReciveThread();
            StartSendThread();

            onConnected(null);
        }
        else
        {
            Reset();
            Debug.LogWarning("Failed to connect server.");
            this.onDisonnected(null);
        }
    }

    private void StartReciveThread()
    {
        t_recive = new Thread(new ThreadStart(ReceiveMessage));
        t_recive.Start();
    }

    private void ReceiveMessage()
    {
        List<byte> list = new List<byte>();

        while (true)
        {
            if (!m_isConnected)
            {
                Debug.LogWarning("ReceiveMessage lost connection");
                break;
            }

            byte[] recivedData = new byte[1024];
            int n = 0;
            try
            {
                n = m_socket.Receive(recivedData);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            if (n == 0)
            {
                m_isConnected = false;
                continue;
            }

            byte[] data = new byte[n];

            Array.Copy(recivedData, data, n);
            list.AddRange(data);

            ReadMessage(list);
        }

        this.onDisonnected(null);
    }

    private void ReadMessage(List<byte> list)
    {
        // 读取数据长度
        if (list.Count < 4)
        {
            return;
        }

        byte[] sizeData = list.GetRange(0, 4).ToArray();
        int size = 0;

        using (MemoryStream stream = new MemoryStream(sizeData))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                size = reader.ReadInt32();
            }
        }

        if (list.Count < 4 + size)
        {
            return;
        }

        // 读取数据
        byte[] buffer = list.GetRange(4, size).ToArray();
        list.RemoveRange(0, 4 + size);

        using (MemoryStream stream = new MemoryStream(buffer))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {

                int msgID = reader.ReadInt32();
                byte[] data = reader.ReadBytes(size - 8);
                uint checksum = reader.ReadUInt32();

                Message msg = new Message();
                msg.Size = size;
                msg.MessageID = msgID;
                msg.Data = data;
                msg.Checksum = checksum;

                if (msg.Verify())
                {

                    // 心跳包
                    if (msg.MessageID == 0)
                    {
                        Message hb = new Message(0, new byte[] { });
                        PushMessage(hb);
                    }
                    else
                    {
                        this.onMessage(msg);
                    }
                }
            }
        }

        // 递归
        ReadMessage(list);
    }


    private void StartSendThread()
    {
        t_send = new Thread(new ThreadStart(SendMessage));
        t_send.Start();
    }

    private void SendMessage()
    {
        while (true)
        {
            if (!m_isConnected)
            {
                Debug.LogWarning("SendMessage lost connection");
                break;
            }

            if (m_sendQueue.Count > 0)
            {
                lock (m_lock)
                {
                    Message msg = m_sendQueue.Dequeue();
                    m_socket.Send(msg.ToArray());
                }
            }
        }
    }

    public void PushMessage(Message msg)
    {
        lock (m_lock)
        {
            m_sendQueue.Enqueue(msg);
        }
    }

    public void Reset()
    {

        if (m_isConnected)
        {
            m_isConnected = false;

            try
            {
                m_socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.ToString());
            }
        }

        if (m_socket != null)
        {
            m_socket.Close();
            m_socket = null;
        }

        m_sendQueue.Clear();
    }
}
