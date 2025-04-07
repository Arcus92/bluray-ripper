namespace BluRayLib.Enums;

public enum SampleRate : byte
{
    Rate48KHz = 0x01,
    Rate96KHz = 0x04,
    Rate192KHz = 0x05,
    Rate48_192KHz = 0x0C,
    Rate48_96KHz = 0x0E
}