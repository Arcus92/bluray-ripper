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
        var bits = reader.ReadBits8();
        MenuCall = bits.ReadBit();
        TitleSearch = bits.ReadBit();
        ChapterSearch = bits.ReadBit();
        TimeSearch = bits.ReadBit();
        SkipToNextPoint = bits.ReadBit();
        SkipToPrevPoint = bits.ReadBit();
        bits.Skip(1); // Reserved
        Stop = bits.ReadBit();
        
        bits = reader.ReadBits8();
        PauseOn = bits.ReadBit();
        bits.Skip(1); // Reserved
        StillOff = bits.ReadBit();
        ForwardPlay = bits.ReadBit();
        BackwardPlay = bits.ReadBit();
        Resume = bits.ReadBit();
        MoveUpSelectedButton = bits.ReadBit();
        MoveDownSelectedButton = bits.ReadBit();
        
        bits = reader.ReadBits8();
        MoveLeftSelectedButton = bits.ReadBit();
        MoveRightSelectedButton = bits.ReadBit();
        SelectButton = bits.ReadBit();
        ActivateButton = bits.ReadBit();
        SelectAndActivateButton = bits.ReadBit();
        PrimaryAudioStreamNumberChange = bits.ReadBit();
        bits.Skip(1); // Reserved
        AngleNumberChange = bits.ReadBit();
        
        bits = reader.ReadBits8();
        PopupOn = bits.ReadBit();
        PopupOff = bits.ReadBit();
        PrimaryPgEnableDisable = bits.ReadBit();
        PrimaryPgStreamNumberChange = bits.ReadBit();
        SecondaryVideoEnableDisable = bits.ReadBit();
        SecondaryVideoStreamNumberChange = bits.ReadBit();
        SecondaryAudioEnableDisable = bits.ReadBit();
        SecondaryAudioStreamNumberChange = bits.ReadBit();
        
        bits = reader.ReadBits8();
        bits.Skip(1); // Reserved
        SecondaryPgStreamNumberChange = bits.ReadBit();
        
        reader.Skip(3);
    }
}