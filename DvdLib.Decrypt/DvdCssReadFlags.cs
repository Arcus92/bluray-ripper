namespace DvdLib.Decrypt;

[Flags]
public enum DvdCssReadFlags : int
{
    None = 0,
    Decrypt = 1 << 0
}