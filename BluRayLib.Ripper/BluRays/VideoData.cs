namespace BluRayLib.Ripper.BluRays;

public class VideoData : StreamData
{
    public VideoData(ushort id, string description) : base(id, description)
    {
    }
    public override string ToString() => $"{Description} [0x{Id:x4}]{(IsSecondary ?" (secondary)" : "")}";
}