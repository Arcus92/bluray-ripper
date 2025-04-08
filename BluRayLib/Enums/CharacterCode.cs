namespace BluRayLib.Enums;

public enum CharacterCode : byte
{
    Utf8 = 0x01,
    Utf16Be = 0x02,
    ShiftJis = 0x03,
    EucKr = 0x04,
    Gb18030_20001 = 0x05,
    CnGb = 0x06,
    Big5 = 0x07,
}