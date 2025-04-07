using BluRayLib.Enums;
using BluRayLib.Utils;
using BluRayLib.Utils.IO;

namespace BluRayLib.Mpls;

public class StreamAttributes
{
    public StreamCodingType CodingType { get; set; }
    
    public VideoFormat VideoFormat { get; set; }
    
    public FrameRate FrameRate { get; set; }
    
    public DynamicRangeType DynamicRange { get; set; }
    
    public ColorSpace ColorSpace { get; set; }
    
    public bool CRFlag { get; set; }
    public bool HDRPlusFlag { get; set; }
    
    public AudioFormat AudioFormat { get; set; }
    
    public SampleRate SampleRate { get; set; }

    public CharacterCode CharacterCode { get; set; }

    public string LanguageCode { get; set; } = "";
    public void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadByte();
        var start = reader.Position;
        if (length == 0) return;
        
        CodingType = (StreamCodingType)reader.ReadByte();
        byte value;

        switch (CodingType)
        {
            case StreamCodingType.MPEG1VideoStream:
            case StreamCodingType.MPEG2VideoStream:
            case StreamCodingType.MPEG4AVCVideoStream:
            case StreamCodingType.SMTPEVC1VideoStream:
                value = reader.ReadByte(); 
                VideoFormat = (VideoFormat)BitUtils.GetBitsFromLeft(value, 0, 4);
                FrameRate = (FrameRate)BitUtils.GetBitsFromLeft(value, 4, 4);
                break;
            
            case StreamCodingType.HEVCVideoStream:
                value = reader.ReadByte(); 
                VideoFormat = (VideoFormat)BitUtils.GetBitsFromLeft(value, 0, 4);
                FrameRate = (FrameRate)BitUtils.GetBitsFromLeft(value, 4, 4);
                value = reader.ReadByte(); 
                DynamicRange = (DynamicRangeType)BitUtils.GetBitsFromLeft(value, 0, 4);
                ColorSpace = (ColorSpace)BitUtils.GetBitsFromLeft(value, 4, 4);
                value = reader.ReadByte(); 
                CRFlag = (value & 1 << 0) != 0;
                HDRPlusFlag = (value & 2 << 0) != 0;
                break;
            
            case StreamCodingType.MPEG1AudioStream:
            case StreamCodingType.MPEG2AudioStream:
            case StreamCodingType.LPCMAudioStream:
            case StreamCodingType.DolbyDigitalAudioStream:
            case StreamCodingType.DtsAudioStream:
            case StreamCodingType.DolbyDigitalTrueHDAudioStream:
            case StreamCodingType.DolbyDigitalPlusAudioStream:
            case StreamCodingType.DtsHDHighResolutionAudioStream:
            case StreamCodingType.DtsHDMasterAudioStream:
            case StreamCodingType.DolbyDigitalPlusSecondaryAudioStream:
            case StreamCodingType.DtsHDSecondaryAudioStream:
                value = reader.ReadByte(); 
                AudioFormat = (AudioFormat)BitUtils.GetBitsFromLeft(value, 0, 4);
                SampleRate = (SampleRate)BitUtils.GetBitsFromLeft(value, 4, 4);
                LanguageCode = reader.ReadString(3);
                break;
            
            case StreamCodingType.PresentationGraphicsStream:
            case StreamCodingType.InteractiveGraphicsStream:
                LanguageCode = reader.ReadString(3);
                break;
            case StreamCodingType.TextSubtitleStream:
                CharacterCode = (CharacterCode)reader.ReadByte();
                LanguageCode = reader.ReadString(3);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        reader.SkipTo(start + length);
    }
}