namespace BluRayLib.Enums;

public enum SubPathType : byte
{
    PrimaryAudio = 0x02,
    InteractiveGraphicsMenu = 0x03,
    TextSubtitle = 0x04,
    OutOfMuxSynchronousType = 0x06,
    OutOfMuxAsynchronousType = 0x06,
    InMuxSynchronousType = 0x07,
    StereoscopicVideo = 0x08,
    StereoscopicInteractiveGraphicsMenu = 0x09,
    DolbyVisionEnhancementLayer = 0x0A,
}