using BluRayLib.Enums;
using MediaLib.Utils.IO;

namespace BluRayLib.Clip;

public class ProgramStream
{
    public ushort Pid { get; set; }
    
    public StreamCodingType CodingType { get; set; }
    
    public VideoFormat VideoFormat { get; set; }
    
    public FrameRate FrameRate { get; set; }
    
    public AspectRatio AspectRatio { get; set; }
    
    public DynamicRangeType DynamicRange { get; set; }
    
    public ColorSpace ColorSpace { get; set; }
    
    public bool CrFlag { get; set; }
    public bool OcFlag { get; set; }
    public bool HdrPlusFlag { get; set; }
    
    public AudioFormat AudioFormat { get; set; }
    
    public SampleRate SampleRate { get; set; }

    public CharacterCode CharacterCode { get; set; }

    public string LanguageCode { get; set; } = "";
    
    public void Read(BigEndianBinaryReader reader)
    {
        Pid = reader.ReadUInt16();
        var length = reader.ReadByte();
        var start = reader.Position;

        CodingType = (StreamCodingType)reader.ReadByte();

        BigEndianBitReader<byte> bits;
        switch (CodingType)
        {
            case StreamCodingType.MPEG1VideoStream:
            case StreamCodingType.MPEG2VideoStream:
            case StreamCodingType.MPEG4AVCVideoStream:
            case StreamCodingType.MPEG4MVCVideoStream:
            case StreamCodingType.SMTPEVC1VideoStream:
                bits = reader.ReadBits8(); 
                VideoFormat = (VideoFormat)bits.ReadBits(4);
                FrameRate = (FrameRate)bits.ReadBits(4);
                
                bits = reader.ReadBits8(); 
                AspectRatio = (AspectRatio)bits.ReadBits(4);
                bits.Skip(2);
                OcFlag = bits.ReadBit();
                break;
            
            case StreamCodingType.HEVCVideoStream:
                bits = reader.ReadBits8(); 
                VideoFormat = (VideoFormat)bits.ReadBits(4);
                FrameRate = (FrameRate)bits.ReadBits(4);
                
                bits = reader.ReadBits8();
                AspectRatio = (AspectRatio)bits.ReadBits(4);
                bits.Skip(2);
                OcFlag = bits.ReadBit();
                CrFlag = bits.ReadBit();
                
                bits = reader.ReadBits8();
                DynamicRange = (DynamicRangeType)bits.ReadBits(4);
                ColorSpace = (ColorSpace)bits.ReadBits(4);
                
                bits = reader.ReadBits8();
                HdrPlusFlag = bits.ReadBit();
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
                bits = reader.ReadBits8(); 
                AudioFormat = (AudioFormat)bits.ReadBits(4);
                SampleRate = (SampleRate)bits.ReadBits(4);
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