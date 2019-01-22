using UnityEngine;

[System.Serializable]
public class BroadcastJoin : BaseMsg
{
    public string playerID;
    public string name;
    public int x;
    public int y;
    public int type;

    public BroadcastJoin() : base(MsgID.Broadcast_Join)
    {
    }

    public override void FromData(byte[] data)
    {
        var jsonString = System.Text.Encoding.Default.GetString(data);

        BroadcastJoin jsonData = JsonUtility.FromJson<BroadcastJoin>(jsonString);
        this.playerID = jsonData.playerID;
        this.name = jsonData.name;
        this.x = jsonData.x;
        this.y = jsonData.y;
        this.type = jsonData.type;
    }

    public override byte[] ToData()
    {
        return new byte[0];
    }
}
