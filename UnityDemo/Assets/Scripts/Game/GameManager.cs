using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    public GameObject playerPrefab;
    public GameObject hero;

    void Start()
    {
        RegisterEvents();
        NetworkManager.Instance.Connect();
    }

    void RegisterEvents()
    {
        NotificationCenter nc = NotificationCenter.Instance;

        nc.AddEventListener(NotificationType.Network_OnResponseJoin, OnResponseJoin);
        nc.AddEventListener(NotificationType.Network_OnBroadcastMove, OnBroadcastMove);
        nc.AddEventListener(NotificationType.Network_OnBroadcastJoin, OnBroadcastJoin);
        nc.AddEventListener(NotificationType.Network_OnBroadcastLeave, OnBroadcastLeave);
        nc.AddEventListener(NotificationType.Network_OnConnected, OnConnected);
        nc.AddEventListener(NotificationType.Network_OnDisconnected, OnDisconnected);
        nc.AddEventListener(NotificationType.Operate_MapPosition, OnTouchMap);
    }

    void OnResponseJoin(NotificationArg arg)
    {
        ResponseJoin data = arg.GetValue<ResponseJoin>();
        PlayerData self = data.self;
        hero = CreatePlayer(self.x, self.y, self.playerID, self.type);

        foreach (PlayerData pdata in data.list)
        {
            players.Add(pdata.playerID, CreatePlayer(pdata.x, pdata.y, pdata.playerID, pdata.type));
        }
    }

    void OnBroadcastMove(NotificationArg arg)
    {
        BroadcastMove data = arg.GetValue<BroadcastMove>();

        if (players.ContainsKey(data.playerID))
        {
            var p = players[data.playerID];
            p.GetComponent<Player>().MoveTo(data.x, data.y);
        }
    }

    void OnBroadcastJoin(NotificationArg arg)
    {
        BroadcastJoin data = arg.GetValue<BroadcastJoin>();
        var p = CreatePlayer(data.x, data.y, data.playerID, data.type);
        players.Add(data.playerID, p);
    }

    void OnBroadcastLeave(NotificationArg arg)
    {
        BroadcastLeave data = arg.GetValue<BroadcastLeave>();

        if (players.ContainsKey(data.playerID))
        {
            var p = players[data.playerID];
            Destroy(p);
        }
    }

    void OnTouchMap(NotificationArg arg)
    {
        Vector3 targetPos = arg.GetValue<Vector3>();
        hero.GetComponent<Player>().MoveTo(targetPos.x, targetPos.z);

        NetworkManager.Instance.SendMove((int)targetPos.x, (int)targetPos.z, hero.GetComponent<Player>().ID);
    }

    public GameObject CreatePlayer(float x, float y, string playerID, int type)
    {
        Vector3 position = new Vector3(x, 0.5f, y);
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = position;
        player.GetComponent<Player>().SetName(playerID);
        player.GetComponent<Player>().SetType(type);
        player.GetComponent<Player>().ID = playerID;

        return player;
    }


    void OnConnected(NotificationArg arg)
    {
        Debug.Log("OnConnected");
        NetworkManager.Instance.SendJoin();
    }

    void OnDisconnected(NotificationArg arg)
    {
        Debug.Log("OnDisconnected");
    }
}
