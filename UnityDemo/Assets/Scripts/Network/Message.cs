using System;
using System.IO;

public class Message
{
    public int Size { get; set; }

    public int MessageID { get; set; }

    public uint Checksum { get; set; }

    public byte[] Data { get; set; }

    public Message()
    {

    }
    public Message(int playerID, byte[] data)
    {
        this.MessageID = playerID;
        this.Data = data;
        this.Size = this.Data.Length + 8;
        this.Checksum = CalcChecksum(playerID, this.Data);
    }

    public byte[] ToArray()
    {
        byte[] bytes;

        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Size);
                writer.Write(MessageID);
                writer.Write(Data);
                writer.Write(Checksum);
            }
            stream.Flush();
            bytes = stream.GetBuffer();
        }

        byte[] result = new byte[Size + 4];
        Array.Copy(bytes, result, 4 + Size);

        return result;
    }

    private uint CalcChecksum(int msgID, byte[] data)
    {
        uint checksum = 0;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(msgID);
                writer.Write(data);
            }

            stream.Flush();
            byte[] bytes = stream.GetBuffer();

            checksum = ComputeHash(bytes, 0, data.Length + 4);
        }
        return checksum;
    }

    public bool Verify()
    {
        return CalcChecksum(this.MessageID, this.Data) == this.Checksum;
    }

    public override string ToString()
    {
        return "msgID=" + this.MessageID + " size=" + this.Size + " checksum=" + this.Checksum;
    }

    private uint ComputeHash(byte[] bytesArray, int byteStart, int bytesToRead)
    {
        uint checksum = 1;

        int n;
        uint s1 = checksum & 0xFFFF;
        uint s2 = checksum >> 16;

        while (bytesToRead > 0)
        {
            n = (3800 > bytesToRead) ? bytesToRead : 3800;
            bytesToRead -= n;

            while (--n >= 0)
            {
                s1 = s1 + (uint)(bytesArray[byteStart++] & 0xFF);
                s2 = s2 + s1;
            }

            s1 %= 65521;
            s2 %= 65521;
        }

        checksum = (s2 << 16) | s1;
        return checksum;
    }
}
