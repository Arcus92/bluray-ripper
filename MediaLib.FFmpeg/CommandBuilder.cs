using System.Text;

namespace MediaLib.FFmpeg;

/// <summary>
/// A class to build the ffmpeg command line.
/// </summary>
public class CommandBuilder
{
    /// <summary>
    /// The internal string builder.
    /// </summary>
    private readonly StringBuilder _arguments = new();

    /// <summary>
    /// Gets the arguments.
    /// </summary>
    public string Arguments => _arguments.ToString();

    /// <summary>
    /// Gets all input streams.
    /// </summary>
    public List<InputStream> InputStreams { get; } = [];
    
    /// <summary>
    /// The number of inputs.
    /// </summary>
    private int _inputs;

    /// <summary>
    /// Adds an input to the converter and returns it's index.
    /// </summary>
    /// <param name="path">The file path</param>
    /// <returns>Returns the id of the input.</returns>
    public int Input(string path)
    {
        _arguments.Append($"-i \"{path}\" ");
        return _inputs++;
    }

    /// <summary>
    /// Adds an input stream to the converter and returns it's index.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Returns the id of the input.</returns>
    public int Input(Stream stream)
    {
        return Input(CreateInputStream(stream));
    }
    
    /// <summary>
    /// Adds an input stream to the converter and returns it's index.
    /// </summary>
    /// <param name="streamFunc">The stream creation callback.</param>
    /// <returns>Returns the id of the input.</returns>
    public int Input(Func<Stream> streamFunc)
    {
        return Input(CreateInputStream(streamFunc));
    }
    
    /// <summary>
    /// Adds an input stream to the converter and returns it's index.
    /// </summary>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>Returns the id of the input.</returns>
    public int Input(InputStream inputStream)
    {
        return Input(inputStream.GetPath());
    }

    /// <summary>
    /// Gets the next FFmpeg file name.
    /// </summary>
    /// <returns></returns>
    private string GetNextPipeName()
    {
        return $"ffpipe_{InputStreams.Count}_{DateTime.Now.Ticks}";
    }
    
    /// <summary>
    /// Creates a new input stream.
    /// </summary>
    /// <param name="streamFunc">The stream creation callback.</param>
    /// <returns></returns>
    public InputStream CreateInputStream(Func<Stream> streamFunc)
    {
        var pipeName = GetNextPipeName();
        var inputStream = new InputStream(pipeName, streamFunc);
        InputStreams.Add(inputStream);
        return inputStream;
    }
    
    /// <summary>
    /// Creates a new input stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns></returns>
    public InputStream CreateInputStream(Stream stream)
    {
        var pipeName = GetNextPipeName();
        var inputStream = new InputStream(pipeName, stream);
        InputStreams.Add(inputStream);
        return inputStream;
    }
    
    /// <summary>
    /// Sets the decryption key.
    /// </summary>
    /// <param name="key">The decryption key.</param>
    public void DecryptionKey(string key)
    {
        _arguments.Append($"-decryption_key {key} ");
    }
    
    /// <summary>
    /// Forces an output format.
    /// </summary>
    /// <param name="extension">The output file extension without dot.</param>
    public void Format(string extension)
    {
        _arguments.Append($"-f {extension} ");
    }

    /// <summary>
    /// Sets the safe mode level.
    /// </summary>
    /// <param name="value">The save mode level.</param>
    public void Safe(int value)
    {
        _arguments.Append($"-safe {value} ");
    }

    /// <summary>
    /// Sets the progress output pipe.
    /// </summary>
    /// <param name="url">The output pipe to receive the progress data.</param>
    public void Progress(string url)
    {
        _arguments.Append($"-progress {url} ");
    }
    
    /// <summary>
    /// Sets if the output file should be overwritten if it exists.
    /// </summary>
    /// <param name="overwrite">
    /// If set to true, the file will be overwritten without asking.
    /// If set to false, the converts fails it the exists. 
    /// </param>
    public void OverwriteOutput(bool overwrite = true)
    {
        _arguments.Append(overwrite ? "-y " : "-n ");
    }

    /// <summary>
    /// Sets the max bit rate.
    /// </summary>
    /// <param name="value">The bit rate in k.</param>
    public void MaxRate(int value)
    {
        _arguments.Append($"-maxrate {value}k ");
    }
    
    /// <summary>
    /// Sets the buffer size.
    /// </summary>
    /// <param name="value">The buffer size in k.</param>
    public void BufferSize(int value)
    {
        _arguments.Append($"-bufsize {value}k ");
    }

    /// <summary>
    /// Sets the constant rate factor.
    /// </summary>
    /// <param name="value">The constant rate factor.</param>
    public void ConstantRateFactor(int value)
    {
        _arguments.Append($"-crf {value} ");
    }

    /// <summary>
    /// Sets the codec to use for all streams.
    /// </summary>
    /// <param name="codec">The codec to use.</param>
    public void Codec(string codec)
    {
        _arguments.Append($"-c {codec} ");
    }
    
    /// <summary>
    /// Sets the codec to use for the given stream types.
    /// </summary>
    /// <param name="streamType">The stream type to set the codec.</param>
    /// <param name="codec">The codec to use.</param>
    public void Codec(StreamType streamType, string codec)
    {
        _arguments.Append($"-c:{streamType.Identifier()} {codec} ");
    }
    
    /// <summary>
    /// Sets the codec to use for the given stream types.
    /// </summary>
    /// <param name="streamType">The stream type to set the codec.</param>
    /// <param name="streamIndex">The index of the stream to set the codec.</param>
    /// <param name="codec">The codec to use.</param>
    public void Codec(StreamType streamType, int streamIndex, string codec)
    {
        _arguments.Append($"-c:{streamType.Identifier()}:{streamIndex} {codec} ");
    }
    
    /// <summary>
    /// Maps all streams from the given input file.
    /// </summary>
    /// <param name="inputId">The id of the input file.</param>
    public void Map(int inputId)
    {
        _arguments.Append($"-map {inputId} ");
    }
    
    /// <summary>
    /// Maps all streams from the given input file.
    /// </summary>
    /// <param name="inputId">The id of the input file.</param>
    /// <param name="streamIndex">The index of the stream to set the codec.</param>
    public void Map(int inputId, int streamIndex)
    {
        _arguments.Append($"-map {inputId}:{streamIndex} ");
    }
    
    /// <summary>
    /// Maps all streams of the given type from the given input file.
    /// </summary>
    /// <param name="inputId">The id of the input file.</param>
    /// <param name="streamType">The stream type to map.</param>
    /// <param name="optional">If set, the convert will not fail if no streams were found.</param>
    public void Map(int inputId, StreamType streamType, bool optional = false)
    {
        _arguments.Append($"-map {inputId}:{streamType.Identifier()}{(optional?"?":"")} ");
    }
    
    /// <summary>
    /// Maps the streams of the given type with the given index from the given input file.
    /// </summary>
    /// <param name="inputId">The id of the input file.</param>
    /// <param name="streamType">The stream type to map.</param>
    /// <param name="streamIndex">The index of the stream to map.</param>
    /// <param name="optional">If set, the convert will not fail if no streams were found.</param>
    public void Map(int inputId, StreamType streamType, int streamIndex, bool optional = false)
    {
        _arguments.Append($"-map {inputId}:{streamType.Identifier()}:{streamIndex}{(optional?"?":"")} ");
    }
    
    /// <summary>
    /// Maps the chapters from the given input stream.
    /// </summary>
    /// <param name="inputId">The id of the input file.</param>
    public void MapChapters(int inputId)
    {
        _arguments.Append($"-map_chapters {inputId} ");
    }
    
    /// <summary>
    /// Maps the metadata from the given input stream.
    /// </summary>
    /// <param name="inputId">The id of the input file.</param>
    public void MapMetadata(int inputId)
    {
        _arguments.Append($"-map_metadata {inputId} ");
    }
    
    /// <summary>
    /// Sets a metadata property for the given stream.
    /// </summary>
    /// <param name="streamIndex">The index of the stream to set the metadata.</param>
    /// <param name="property">The metadata property name.</param>
    /// <param name="value">The metadata value.</param>
    public void Metadata(int streamIndex, string property, string value)
    {
        _arguments.Append($"-metadata:s:{streamIndex} {property}=\"{value}\" ");
    }

    /// <summary>
    /// Sets a metadata property for the given stream.
    /// </summary>
    /// <param name="streamType">The stream type to set the metadata.</param>
    /// <param name="streamIndex">The index of the stream to set the metadata.</param>
    /// <param name="property">The metadata property name.</param>
    /// <param name="value">The metadata value.</param>
    public void Metadata(StreamType streamType, int streamIndex, string property, string value)
    {
        _arguments.Append($"-metadata:s:{streamType.Identifier()}:{streamIndex} {property}=\"{value}\" ");
    }

    /// <summary>
    /// Sets the disposition flag for the given stream.
    /// </summary>
    /// <param name="streamIndex">The index of the stream to set the disposition.</param>
    /// <param name="value">The disposition flag.</param>
    public void Disposition(int streamIndex, string value)
    {
        _arguments.Append($"-disposition:{streamIndex} {value} ");
    }
    
    /// <summary>
    /// Sets the disposition flag for the given stream.
    /// </summary>
    /// <param name="streamType">The stream type to set the disposition.</param>
    /// <param name="streamIndex">The index of the stream to set the disposition.</param>
    /// <param name="value">The disposition flag.</param>
    public void Disposition(StreamType streamType, int streamIndex, string value)
    {
        _arguments.Append($"-disposition:{streamType.Identifier()}:{streamIndex} {value} ");
    }

    /// <summary>
    /// Seeks to the given timestamp.
    /// </summary>
    /// <param name="value">The timestamp to seek to.</param>
    public void Seek(TimeSpan value)
    {
        _arguments.Append($"-ss {value} ");
    }
    
    /// <summary>
    /// Seeks to the given timestamp in the input file.
    /// </summary>
    /// <param name="value">The timestamp to seek to.</param>
    /// <param name="inputId">The id of the input file</param>
    public void Seek(TimeSpan value, int inputId)
    {
        _arguments.Append($"-ss {value} {inputId} ");
    }
    
    /// <summary>
    /// Sets the duration.
    /// </summary>
    /// <param name="value">The duration.</param>
    public void Duration(TimeSpan value)
    {
        _arguments.Append($"-t {value} ");
    }
    
    /// <summary>
    /// Sets the duration of the input file.
    /// </summary>
    /// <param name="value">The duration.</param>
    /// <param name="inputId">The id of the input file</param>
    public void Duration(TimeSpan value, int inputId)
    {
        _arguments.Append($"-t {value} {inputId} ");
    }
    
    /// <summary>
    /// Sets the filter to use for the given stream types.
    /// </summary>
    /// <param name="streamType">The stream type to set the filter.</param>
    /// <param name="filter">The filter definition.</param>
    public void Filter(StreamType streamType, string filter)
    {
        _arguments.Append($"-filter:{streamType.Identifier()} \"{filter}\" ");
    }
    
    /// <summary>
    /// Adds a custom complex filter.
    /// </summary>
    /// <param name="filter">The filter definition.</param>
    public void FilterComplex(string filter)
    {
        _arguments.Append($"-filter_complex {filter} ");
    }

    /// <summary>
    /// Sets the analysis duration for the following input.
    /// </summary>
    /// <param name="value">The analysis duration.</param>
    public void AnalyzeDuration(long value)
    {
        _arguments.Append($"-analyzeduration {value} ");
    }
    
    /// <summary>
    /// Sets the probe size for the following input.
    /// </summary>
    /// <param name="value">The probe size.</param>
    public void ProbeSize(long value)
    {
        _arguments.Append($"-probesize {value} ");
    }

    /// <summary>
    /// Sets the FFmpeg log-level.
    /// </summary>
    /// <param name="logLevel">The log-level.</param>
    public void LogLevel(string logLevel)
    {
        _arguments.Append($"-loglevel {logLevel} ");
    }
    
    /// <summary>
    /// Defines the output path. Must be the last argument.
    /// </summary>
    /// <param name="path">The file path.</param>
    public void Output(string path)
    {
        _arguments.Append($"\"{path}\" ");
    }
}