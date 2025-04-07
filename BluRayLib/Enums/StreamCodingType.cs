namespace BluRayLib.Enums;

public enum StreamCodingType : byte
{
    MPEG1VideoStream = 0x01,
    MPEG2VideoStream = 0x02,
    MPEG4AVCVideoStream = 0x1B,
    MPEG4MVCVideoStream = 0x20,
    SMTPEVC1VideoStream = 0xEA,
    HEVCVideoStream = 0x24,
    MPEG1AudioStream = 0x03,
    MPEG2AudioStream = 0x04,
    LPCMAudioStream = 0x80,
    DolbyDigitalAudioStream = 0x81,
    DtsAudioStream = 0x82,
    DolbyDigitalTrueHDAudioStream = 0x83,
    DolbyDigitalPlusAudioStream = 0x84,
    DtsHDHighResolutionAudioStream = 0x85,
    DtsHDMasterAudioStream = 0x86,
    DolbyDigitalPlusSecondaryAudioStream = 0xA1,
    DtsHDSecondaryAudioStream = 0xA2,
    PresentationGraphicsStream = 0x90,
    InteractiveGraphicsStream = 0x91,
    TextSubtitleStream = 0x92,
}