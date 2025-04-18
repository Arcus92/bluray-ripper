using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BluRayLib.Ripper;
using BluRayLib.Ripper.BluRays;
using BluRayLib.Ripper.BluRays.Export;
using BluRayRipper.Models;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;

namespace BluRayRipper.Services;

/// <summary>
/// Handling the output directory. Can scan the output directory for existing exports.
/// </summary>
public class OutputService(IDiskService diskService) : IOutputService
{
    /// <inheritdoc />
    public string OutputPath { get; private set; } = "";

    /// <inheritdoc />
    public async Task OpenAsync(string outputPath)
    {
        OutputPath = outputPath;
        await RefreshAsync();
    }

    #region List

    /// <inheritdoc />
    public ObservableCollection<OutputModel> Items { get; } = [];
    
    /// <inheritdoc />
    public async Task<OutputModel> AddAsync(OutputFile outputFile)
    {
        // Prevent an item to be added twice.
        var model = GetByPlaylist(outputFile.DiskName, outputFile.PlaylistId);
        if (model is not null) return model;

        model = new OutputModel(outputFile);
        Items.Add(model);
        await UpdateOutputInfoAsync(outputFile);
        return model;
    }

    /// <inheritdoc />
    public async Task RemoveAsync(OutputModel output)
    {
        await RemoveOutputInfoAsync(output.File);
        Items.Remove(output);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(OutputModel output)
    {
        await UpdateOutputInfoAsync(output.File);
    }

    /// <inheritdoc />
    public OutputModel? GetByPlaylist(string diskName, ushort playlistId)
    {
        return Items.FirstOrDefault(f => f.DiskName == diskName && f.PlaylistId == playlistId);
    }

    /// <inheritdoc />
    public async Task RefreshAsync()
    {
        Items.Clear();
        await foreach (var file in OutputFileSerializer.DeserializeFromDirectoryAsync(OutputPath))
        {
            var model = new OutputModel(file);
            Items.Add(model);
        }
    }
    
    #endregion List
    
    /// <summary>
    /// Writes the given output info file to the output directory.
    /// </summary>
    /// <param name="outputFile">The output info file.</param>
    private async Task UpdateOutputInfoAsync(OutputFile outputFile)
    {
        var path = Path.Combine(OutputPath, $"{outputFile.BaseName}.json");
        await OutputFileSerializer.SerializeAsync(path, outputFile);
    }
    
    /// <summary>
    /// Removes the given output info file from the output directory.
    /// </summary>
    /// <param name="outputFile">The output info file.</param>
    private Task RemoveOutputInfoAsync(OutputFile outputFile)
    {
        var path = Path.Combine(OutputPath, $"{outputFile.BaseName}.json");
        File.Delete(path);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Renames the given output info to the new filename.
    /// </summary>
    /// <param name="outputFile">The output info file.</param>
    /// <param name="newFilename">The new filename.</param>
    private async Task RenameOutputInfoAsync(OutputFile outputFile, string newFilename)
    {
        var oldPath = Path.Combine(OutputPath, $"{outputFile.BaseName}.json");
        var newPath = Path.Combine(OutputPath, $"{newFilename}.json");
        
        if (File.Exists(oldPath))
            File.Move(oldPath, newPath);
        
        outputFile.BaseName = newPath;
        await UpdateOutputInfoAsync(outputFile);
    }
    
    #region Output
    
    /// <inheritdoc />
    public OutputFile BuildOutputInfo(TitleData title, VideoFormat videoFormat, CodecOptions codecOptions, string baseName)
    {
        if (title.Segments.Length == 0)
            throw new ArgumentException("Cannot create output for title without segments!", nameof(title));
        
        var segment = title.Segments[0];
        var exportSubtitlesAsSeparateFiles = !videoFormat.SupportPgs;
        var outputInfo = new OutputFile()
        {
            PlaylistId = title.Id,
            BaseName = baseName,
            Extension = videoFormat.Extension,
            Source = OutputSource.BluRay,
            DiskName = diskService.DiskName,
            Codec = codecOptions,
            SegmentIds = title.Segments.Select(s => s.Id).ToArray(),
            VideoStreams = segment.VideoStreams.Select(s => new OutputFileStream()
            {
                Id = s.Id,
            }).ToArray(),
            AudioStreams = segment.AudioStreams.Select(s => new OutputFileStream()
            {
                Id = s.Id,
                LanguageCode = s.LanguageCode,
            }).ToArray(),
            SubtitleStreams = segment.SubtitleStreams.Select(s => new OutputFileStream()
            {
                Id = s.Id,
                LanguageCode = s.LanguageCode,
                Extension = exportSubtitlesAsSeparateFiles ? $".{s.LanguageCode}.{s.Id}.sup" : null,
            }).ToArray()
        };

        // Select the default streams
        if (outputInfo.VideoStreams.Length > 0)
            outputInfo.VideoStreams[0].Default = true;
        if (outputInfo.AudioStreams.Length > 0)
            outputInfo.AudioStreams[0].Default = true;
        if (outputInfo.SubtitleStreams.Length > 0)
            outputInfo.SubtitleStreams[0].Default = true;

        return outputInfo;
    }
    
    /// <inheritdoc />
    public TitleExportOptions BuildExportOptionsFormOutputInfo(OutputFile outputFile)
    {
        var title = diskService.GetTitle(outputFile.PlaylistId);
        var format = VideoFormat.FromExtension(outputFile.Extension) ?? VideoFormat.Mkv;
        var options = TitleExportOptions.From(title, outputFile.BaseName);
        options.Extension = format.Extension;
        options.Codec = outputFile.Codec;
        options.ExportSubtitlesAsSeparateFiles = !format.SupportPgs;
        
        // Subtitle filenames
        options.StreamFilenames = new Dictionary<ushort, string>();
        foreach (var stream in outputFile.SubtitleStreams)
        {
            if (stream.Extension is null) continue;
            var filename = $"{options.Basename}{stream.Extension}";
            options.StreamFilenames.Add(stream.Id, filename);
        }
        
        return options;
    }
    
    #endregion Output
}