using UnityEngine;

[System.Serializable]
public class BroadcastMove : BaseMsg
{
    public string playerID;
    public int x;
    public int y;

    public BroadcastMove() : base(MsgID.Broadcast_Move)
    {
    }

    public override void FromData(byte[] data)
    {
        var jsonString = System.Text.Encoding.Default.GetString(data);

        BroadcastMove jsonData = JsonUtility.FromJson<BroadcastMove>(jsonString);
        this.playerID = jsonData.playerID;
        this.x = jsonData.x;
        this.y = jsonData.y;
    }

    public override byte[] ToData()
    {
        return new byte[0];
    }
}

