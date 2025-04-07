namespace BluRayLib.Enums;

public enum FrameRate : byte
{
    Fps24000_1001 = 0x01,
    Fps24 = 0x02,
    Fps25 = 0x03,
    Fps30000_1001 = 0x04,
    Fps50 = 0x06,
    Fps60000_1001 = 0x07,
}

public static class FrameRateHelper
{
    public static double GetSeconds(this FrameRate frameRate, long frames)
    {
        GetTimeScale(frameRate, out var dividend, out var divisor);
        return (double)frames * divisor / dividend;
    }
    
    public static void GetTimeScale(this FrameRate frameRate, out int dividend, out int divisor)
    {
        switch (frameRate)
        {
            case FrameRate.Fps24000_1001:
                dividend = 24000;
                divisor = 1001;
                break;
            case FrameRate.Fps24:
                dividend = 24;
                divisor = 1;
                break;
            case FrameRate.Fps25:
                dividend = 25;
                divisor = 1;
                break;
            case FrameRate.Fps30000_1001:
                dividend = 30000;
                divisor = 1001;
                break;
            case FrameRate.Fps50:
                dividend = 50;
                divisor = 1;
                break;
            case FrameRate.Fps60000_1001:
                dividend = 60000;
                divisor = 1001;
                break;
            default:
                dividend = 1;
                divisor = 1;
                break;
        }
    }
}