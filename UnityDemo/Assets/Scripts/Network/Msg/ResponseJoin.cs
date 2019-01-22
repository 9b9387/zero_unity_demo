using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string playerID;
    public string name;
    public float x;
    public float y;
    public int type;
}

[Serializable]
public class ResponseJoin : BaseMsg
{
    public List<PlayerData> list;
    public PlayerData self;

    public ResponseJoin() : base(MsgID.Response_Join)
    {
    }

    public override void FromData(byte[] data)
    {
        var jsonString = System.Text.Encoding.Default.GetString(data);

        ResponseJoin jsonData = JsonUtility.FromJson<ResponseJoin>(jsonString);
        this.list = jsonData.list;
        this.self = jsonData.self;
    }

    public override byte[] ToData()
    {
        return new byte[0];
    }
}

