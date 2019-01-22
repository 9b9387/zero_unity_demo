using System;

public abstract class BaseMsg
{
    public MsgID msgID { get; private set; }

    protected BaseMsg(MsgID Id)
    {
        this.msgID = Id;
    }

    public abstract byte[] ToData();
    public abstract void FromData(byte[] data);

    public Message ToMessage()
    {
        var data = ToData();
        return new Message(Convert.ToInt32(msgID), data);
    }

    public void FromMessage(Message msg)
    {
        if (msg.MessageID != Convert.ToInt32(msgID))
        {
            return;
        }
        FromData(msg.Data);
    }
}

