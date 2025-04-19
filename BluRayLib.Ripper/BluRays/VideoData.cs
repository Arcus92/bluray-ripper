namespace BluRayLib.Ripper.BluRays;

public class VideoData : StreamData
{
    public VideoData(ushort id) : base(id)
    {
    }
    public override string ToString() => $"Video [0x{Id:x4}] {(IsSecondary ?" (secondary)" : "")}";
}