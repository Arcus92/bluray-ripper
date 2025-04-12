using BluRayLib.FFmpeg;

namespace BluRayLib.Ripper.BluRays.Export;

/// <summary>
/// This manages the title export from BluRay disks.
/// </summary>
public class TitleExporter
{
    private readonly BluRay _bluRay;

    public TitleExporter(BluRay bluRay)
    {
        _bluRay = bluRay;
    }

    /// <summary>
    /// Exports the playlist as a video file using FFmpeg.
    /// </summary>
    /// <param name="options">The title export options.</param>
    /// <param name="output">The output file.</param>
    /// <param name="outputFormat">The FFmpeg output format.</param>
    /// <param name="onUpdate">The status update event.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task ExportAsync(TitleExportOptions options, string output, string? outputFormat = null, Action<ConverterUpdate>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(output)!;
        var fileName = Path.GetFileNameWithoutExtension(output);
        
        var ffmpeg = new Engine();
        await ffmpeg.ConvertAsync(builder =>
        {
            // Builds the concat text file in memory
            var concatStream = new MemoryStream();
            var concatWriter = new StreamWriter(concatStream);
            foreach (var segment in options.Title.Segments)
            {
                var inputStream = builder.CreateInputStream(() => _bluRay.GetM2TsStream(segment.Id));
                concatWriter.WriteLine($"file '{inputStream.GetPath()}'");
            }
            concatWriter.Flush();
            concatStream.Position = 0;

            builder.Format("concat");
            builder.Safe(0);
            var input = builder.Input(concatStream);
            
            if (options.ExportChapters)
            {
                // Builds the chapter file in memory
                var chapterStream = new MemoryStream();
                var chapterWriter = new StreamWriter(chapterStream);
                foreach (var chapter in options.Title.Chapters)
                {
                    var start = (ulong)(chapter.Start.TotalSeconds * 1000);
                    var end = (ulong)(chapter.End.TotalSeconds * 1000);
                    chapterWriter.WriteLine("[CHAPTER]");
                    chapterWriter.WriteLine("TIMEBASE=1/1000");
                    chapterWriter.WriteLine($"START={start}");
                    chapterWriter.WriteLine($"END={end}");
                    chapterWriter.WriteLine($"title={chapter.Name}");
                    chapterWriter.WriteLine();
                }

                chapterWriter.Flush();
                chapterStream.Position = 0;

                // Map the chapter
                builder.Format("ffmetadata");
                var inputChapter = builder.Input(chapterStream);
                builder.MapChapters(inputChapter);
            }

            // Define codec
            builder.Codec(StreamType.Video, options.VideoCodec);
            builder.Codec(StreamType.Audio, options.AudioCodec);
            builder.Codec(StreamType.Subtitle, options.SubtitleCodec);
            
            if (options.ConstantRateFactor.HasValue) builder.ConstantRateFactor(options.ConstantRateFactor.Value);
            if (options.MaxRate.HasValue) builder.MaxRate(options.MaxRate.Value);
            if (options.BufferSize.HasValue) builder.BufferSize(options.BufferSize.Value);
            
            // Map the output streams
            var outputStreamCount = 0;
            var firstSegment = options.Title.Segments.First();
            foreach (var stream in firstSegment.VideoStreams)
            {
                if (options.IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                builder.Map(input, stream.Index);
                outputStreamCount++;
            }
            foreach (var stream in firstSegment.AudioStreams)
            {
                if (options.IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                builder.Map(input, stream.Index);
                if (!string.IsNullOrEmpty(stream.LanguageCode))
                    builder.Metadata(outputStreamCount, "language", stream.LanguageCode);
                if (options.DefaultAudioStreamId == stream.Id)
                    builder.Disposition(outputStreamCount, "default");

                outputStreamCount++;
            }
            if (!options.ExportSubtitlesAsSeparateFiles) // Only included if not exported as separate files...
            {
                foreach (var stream in firstSegment.SubtitleStreams)
                {
                    if (options.IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                    builder.Map(input, stream.Index);
                    if (!string.IsNullOrEmpty(stream.LanguageCode))
                        builder.Metadata(outputStreamCount, "language", stream.LanguageCode);
                    if (options.DefaultSubtitleStreamId == stream.Id)
                        builder.Disposition(outputStreamCount, "default");
                    
                    outputStreamCount++;
                }
            }
            
            // Video output
            builder.OverwriteOutput();
            if (outputFormat is not null)
                builder.Format(outputFormat);
            builder.Output(output);
            
            
            // FFmpeg supports multiple outputs. We can export the subtitle files in a single run as well.
            // We just need to create a new mapping and then define a new output.
            if (options.ExportSubtitlesAsSeparateFiles)
            {
                // Export subtitle files
                foreach (var stream in firstSegment.SubtitleStreams)
                {
                    if (options.IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                    
                    builder.Map(input, stream.Index);
                    builder.Metadata(0, "language", stream.LanguageCode);
                    // Single exports don't need a default flag.
                    
                    builder.Codec(StreamType.Subtitle, options.SubtitleCodec);

                    // Subtitle output
                    builder.OverwriteOutput();
                    var path = Path.Combine(directory, $"{fileName}.{stream.LanguageCode}.{stream.Id}.sup");
                    builder.Output(path);
                }
            }
        }, onUpdate, cancellationToken);
    }
}