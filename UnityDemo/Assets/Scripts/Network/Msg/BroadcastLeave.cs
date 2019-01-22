using UnityEngine;

[System.Serializable]
public class BroadcastLeave : BaseMsg
{
    public string playerID;
    public string name;
    public int x;
    public int y;

    public BroadcastLeave() : base(MsgID.Broadcast_Leave)
    {
    }

    public override void FromData(byte[] data)
    {
        var jsonString = System.Text.Encoding.Default.GetString(data);

        BroadcastLeave jsonData = JsonUtility.FromJson<BroadcastLeave>(jsonString);
        this.playerID = jsonData.playerID;
    }

    public override byte[] ToData()
    {
        return new byte[0];
    }
}

