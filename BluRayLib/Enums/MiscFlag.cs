namespace BluRayLib.Enums;

[Flags]
public enum MiscFlag : byte
{
    LosslessSoundBypassMixer = 1 << 5,
    AllowAudioMixing = 1 << 6,
    HideFromMenu = 1 << 7,
}