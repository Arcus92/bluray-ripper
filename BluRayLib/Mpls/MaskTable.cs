using BluRayLib.Utils;
using BluRayLib.Utils.IO;

namespace BluRayLib.Mpls;

public class MaskTable
{
    public bool MenuCall { get; set; }
    public bool TitleSearch { get; set; }
    public bool ChapterSearch { get; set; }
    public bool TimeSearch { get; set; }
    public bool SkipToNextPoint { get; set; }
    public bool SkipToPrevPoint { get; set; }
    public bool Stop { get; set; }
    public bool PauseOn { get; set; }
    public bool StillOff { get; set; }
    public bool ForwardPlay { get; set; }
    public bool BackwardPlay { get; set; }
    public bool Resume { get; set; }
    public bool MoveUpSelectedButton { get; set; }
    public bool MoveDownSelectedButton { get; set; }
    public bool MoveLeftSelectedButton { get; set; }
    public bool MoveRightSelectedButton { get; set; }
    public bool SelectButton { get; set; }
    public bool ActivateButton { get; set; }
    public bool SelectAndActivateButton { get; set; }
    public bool PrimaryAudioStreamNumberChange { get; set; }
    public bool AngleNumberChange { get; set; }
    public bool PopupOn { get; set; }
    public bool PopupOff { get; set; }
    public bool PrimaryPgEnableDisable { get; set; }
    public bool PrimaryPgStreamNumberChange { get; set; }
    public bool SecondaryVideoEnableDisable { get; set; }
    public bool SecondaryVideoStreamNumberChange { get; set; }
    public bool SecondaryAudioEnableDisable { get; set; }
    public bool SecondaryAudioStreamNumberChange { get; set; }
    public bool SecondaryPgStreamNumberChange { get; set; }
    public void Read(BigEndianBinaryReader reader)
    {
        var value = reader.ReadByte();
        MenuCall = BitUtils.GetBitFromLeft(value, 0);
        TitleSearch = BitUtils.GetBitFromLeft(value, 1);
        ChapterSearch = BitUtils.GetBitFromLeft(value, 2);
        TimeSearch = BitUtils.GetBitFromLeft(value, 3);
        SkipToNextPoint = BitUtils.GetBitFromLeft(value, 4);
        SkipToPrevPoint = BitUtils.GetBitFromLeft(value, 5);
        // Reserved
        Stop = BitUtils.GetBitFromLeft(value, 7);
        
        value = reader.ReadByte();
        PauseOn = BitUtils.GetBitFromLeft(value, 0);
        // Reserved
        StillOff = BitUtils.GetBitFromLeft(value, 2);
        ForwardPlay = BitUtils.GetBitFromLeft(value, 3);
        BackwardPlay = BitUtils.GetBitFromLeft(value, 4);
        Resume = BitUtils.GetBitFromLeft(value, 5);
        MoveUpSelectedButton = BitUtils.GetBitFromLeft(value, 6);
        MoveDownSelectedButton = BitUtils.GetBitFromLeft(value, 7);
        
        value = reader.ReadByte();
        MoveLeftSelectedButton = BitUtils.GetBitFromLeft(value, 0);
        MoveRightSelectedButton = BitUtils.GetBitFromLeft(value, 1);
        SelectButton = BitUtils.GetBitFromLeft(value, 2);
        ActivateButton = BitUtils.GetBitFromLeft(value, 3);
        SelectAndActivateButton = BitUtils.GetBitFromLeft(value, 4);
        PrimaryAudioStreamNumberChange = BitUtils.GetBitFromLeft(value, 5);
        // Reserved
        AngleNumberChange = BitUtils.GetBitFromLeft(value, 7);
        
        value = reader.ReadByte();
        PopupOn = BitUtils.GetBitFromLeft(value, 0);
        PopupOff = BitUtils.GetBitFromLeft(value, 1);
        PrimaryPgEnableDisable = BitUtils.GetBitFromLeft(value, 2);
        PrimaryPgStreamNumberChange = BitUtils.GetBitFromLeft(value, 3);
        SecondaryVideoEnableDisable = BitUtils.GetBitFromLeft(value, 4);
        SecondaryVideoStreamNumberChange = BitUtils.GetBitFromLeft(value, 5);
        SecondaryAudioEnableDisable = BitUtils.GetBitFromLeft(value, 6);
        SecondaryAudioStreamNumberChange = BitUtils.GetBitFromLeft(value, 7);
        
        value = reader.ReadByte();
        // Reserved
        SecondaryPgStreamNumberChange = BitUtils.GetBitFromLeft(value, 1);
        
        reader.Skip(3);
    }
}