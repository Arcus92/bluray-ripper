using MediaLib.Utils.IO;

namespace BluRayLib.Clip;

public class ClipInfo
{
    /// <summary>
    /// Reads the clip-info file.
    /// </summary>
    /// <param name="path">The path to the clip-info file.</param>
    public void Read(string path)
    {
        using var fileStream = File.OpenRead(path);
        Read(fileStream);
    }
    
    /// <summary>
    /// Reads the clip-info file from stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void Read(Stream stream)
    {
        var reader = new BigEndianBinaryReader(stream);
        Read(reader);
    }

    private void Read(BigEndianBinaryReader reader)
    {
        // https://github.com/lw/BluRay/wiki/CLPI
        
        var format = reader.ReadString(4);
        if (format != "HDMV")
            throw new InvalidDataException("Invalid clip magic number!");
        var version = reader.ReadString(4);
        if (version != "0200")
            throw new InvalidDataException($"Invalid clip version: {version}!");
        
        var sequenceInfoStart = reader.ReadUInt32();
        var programInfoStart = reader.ReadUInt32();
        var cpiStart = reader.ReadUInt32();
        var clipMarkStart = reader.ReadUInt32();
        var extensionDataStart = reader.ReadUInt32();

        reader.Skip(12); // Reserved

        ReadClipInfo(reader);

        reader.Position = sequenceInfoStart;
        ReadSequenceInfo(reader);
        
        reader.Position = programInfoStart;
        ReadProgramInfo(reader);
        
        reader.Position = cpiStart;
        ReadCpi(reader);
        
        reader.Position = clipMarkStart;
        ReadClipMark(reader);

        if (extensionDataStart != 0)
        {
            reader.Position = extensionDataStart;
            ReadExtensionData(reader);
        }
    }
    
    #region Clip info

    public byte ClipStreamType { get; set; }
    public byte ApplicationType { get; set; }
    public bool IsCc5 { get; set; }

    public uint TsRecordingRate { get; set; }
    
    public uint NumberOfSourcePackets { get; set; }

    public TsTypeInfoBlock TsTypeInfo { get; } = new();

    private void ReadClipInfo(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt32();
        var start = reader.Position;
        reader.Skip(2); // Reserved
        
        ClipStreamType = reader.ReadByte();
        ApplicationType = reader.ReadByte();
        var flags = reader.ReadUInt32();
        IsCc5 = flags == 1;
        TsRecordingRate = reader.ReadUInt32();
        NumberOfSourcePackets = reader.ReadUInt32();
        reader.Skip(128);
        
        TsTypeInfo.Read(reader);
        
        reader.SkipTo(start + length);
    }
    
    #endregion Clip info
    
    #region Sequence info
    
    private void ReadSequenceInfo(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt32();
        var start = reader.Position;
        reader.Skip(1); // Reserved
        
        
        
        reader.SkipTo(start + length);
    }
    
    #endregion Sequence info
    
    #region Program info

    public Program[] Programs { get; set; } = [];
    
    private void ReadProgramInfo(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt32();
        var start = reader.Position;
        reader.Skip(1); // Reserved

        var count = reader.ReadByte();
        Programs = new Program[count];
        for (var i = 0; i < count; i++)
        {
            var program = new Program();
            program.Read(reader);
            Programs[i] = program;
        }
        
        reader.SkipTo(start + length);
    }
    
    #endregion Program info
    
    #region CPI
    
    public byte CpiType { get; set; }

    public StreamPidEntry[] StreamPidEntries { get; set; } = [];

    private void ReadCpi(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt32();
        if (length == 0) return;
        var start = reader.Position;
        reader.Skip(1); // Reserved
        CpiType = reader.ReadByte();
        reader.Skip(1); // Reserved
        
        var count = reader.ReadByte();
        StreamPidEntries = new StreamPidEntry[count];
        for (var i = 0; i < count; i++)
        {
            var entry = new StreamPidEntry();
            entry.Read(reader);
            StreamPidEntries[i] = entry;
        }

        foreach (var entry in StreamPidEntries)
        {
            reader.Position = start + 2 + entry.EpMapForOneStreamPidStart;

            var epFineTableStart = reader.ReadUInt32();
            foreach (var epCoarseEntry in entry.EpCoarseEntries)
            {
                epCoarseEntry.Read(reader);
            }
            
            reader.Position = start + 2 + entry.EpMapForOneStreamPidStart + epFineTableStart;
            foreach (var epFineEntry in entry.EpFineEntries)
            {
                epFineEntry.Read(reader);
            }
        }
        
        reader.SkipTo(start + length);
    }
    
    #endregion CPI
    
    #region Clip mark
    
    private void ReadClipMark(BigEndianBinaryReader reader)
    {
        
    }
    
    #endregion Clip mark
    
    #region Extension data
    
    private void ReadExtensionData(BigEndianBinaryReader reader)
    {
        
    }
    
    #endregion Extension data
}