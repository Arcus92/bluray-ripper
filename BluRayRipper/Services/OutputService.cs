using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BluRayLib.Ripper.BluRays.Export;
using BluRayLib.Ripper.Output;
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
    public ObservableCollection<OutputModel> Outputs { get; } = [];
    
    /// <inheritdoc />
    public async Task<OutputModel> AddAsync(OutputInfo info)
    {
        // Prevent an item to be added twice.
        var model = GetBySource(info.Source.Type, info.Source.DiskName, info.Source.PlaylistId);
        if (model is not null) return model;

        model = new OutputModel(info);
        Outputs.Add(model);
        await UpdateOutputInfoAsync(info);
        return model;
    }

    /// <inheritdoc />
    public async Task RemoveAsync(OutputModel output)
    {
        await RemoveOutputInfoAsync(output.Info);
        Outputs.Remove(output);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(OutputModel output)
    {
        await UpdateOutputInfoAsync(output.Info);
    }

    /// <inheritdoc />
    public OutputModel? GetBySource(OutputSourceType type, string diskName, ushort playlistId)
    {
        return Outputs.FirstOrDefault(f =>
            f.Info.Source.Type == type && f.Info.Source.DiskName == diskName &&
            f.Info.Source.PlaylistId == playlistId);
    }

    /// <inheritdoc />
    public async Task RefreshAsync()
    {
        Outputs.Clear();
        await foreach (var info in OutputInfoSerializer.DeserializeFromDirectoryAsync(OutputPath))
        {
            var model = new OutputModel(info);
            UpdateStatus(model);
            Outputs.Add(model);
        }
    }

    /// <inheritdoc />
    public void UpdateStatus(OutputModel model)
    {
        // Check the initial status
        var hasAllFiles = true;
        foreach (var file in model.Files)
        {
            var path = Path.Combine(OutputPath, file.Filename);
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                file.Size = fileInfo.Length;
            }
            else
            {
                file.Size = 0;
                hasAllFiles = false;
            }
        }
            
        if (hasAllFiles)
        {
            model.Status = OutputStatus.Completed;
        }
        else
        {
            // Queued output file, but from a different disk.
            if (diskService.DiskName != model.Info.Source.DiskName)
            {
                model.Status = OutputStatus.QueuedMismatchDisk;
            }
        }
    }
    
    #endregion List
    
    #region Rename

    /// <inheritdoc />
    public async Task RenameAsync(OutputInfo outputInfo, TitleNameMap nameMap)
    {
        // TODO
    }
    
    #endregion Rename
    
    /// <summary>
    /// Writes the given output info file to the output directory.
    /// </summary>
    /// <param name="outputInfo">The output info file.</param>
    private async Task UpdateOutputInfoAsync(OutputInfo outputInfo)
    {
        var path = Path.Combine(OutputPath, $"{outputInfo.Name}.json");
        await OutputInfoSerializer.SerializeAsync(path, outputInfo);
    }
    
    /// <summary>
    /// Removes the given output info file from the output directory.
    /// </summary>
    /// <param name="outputInfo">The output info file.</param>
    private Task RemoveOutputInfoAsync(OutputInfo outputInfo)
    {
        var path = Path.Combine(OutputPath, $"{outputInfo.Name}.json");
        File.Delete(path);
        return Task.CompletedTask;
    }
}