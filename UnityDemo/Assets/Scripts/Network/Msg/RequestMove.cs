using UnityEngine;

[System.Serializable]
public class RequestMove : BaseMsg
{
    public int x;
    public int y;
    public string playerID;

    public RequestMove() : base(MsgID.Request_Move)
    {
    }

    public override byte[] ToData()
    {
        var str = JsonUtility.ToJson(this);
        return System.Text.Encoding.ASCII.GetBytes(str);
    }

    public override void FromData(byte[] data)
    {
    }
}
