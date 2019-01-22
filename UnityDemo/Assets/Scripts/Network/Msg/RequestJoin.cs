[System.Serializable]
public class RequestJoin : BaseMsg
{
    public RequestJoin() : base(MsgID.Request_Join)
    {

    }

    public override void FromData(byte[] data)
    {
    }

    public override byte[] ToData()
    {
        return new byte[0];
    }
}
