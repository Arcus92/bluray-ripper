namespace MediaLib.Models;

public class VideoInfo : StreamInfo
{
    public VideoInfo(ushort id, string description) : base(id, description)
    {
    }
    public override string ToString() => $"{Description} [0x{Id:x4}]{(IsSecondary ?" (secondary)" : "")}";
}