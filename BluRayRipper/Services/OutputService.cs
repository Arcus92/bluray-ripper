using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        model = new OutputModel(info, info.MediaInfo.Name);
        Outputs.Add(model);
        await WriteOutputInfoAsync(model);
        return model;
    }

    /// <inheritdoc />
    public async Task RemoveAsync(OutputModel model)
    {
        await RemoveOutputInfoAsync(model);
        Outputs.Remove(model);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(OutputModel model)
    {
        // Checks for renaming some files.
        // Renaming is more complex the through. For example, when swapping filenames of two files, we need an
        // intermediate rename step to not block any filename. 
        var renameMap = new Dictionary<string, string>();
        
        var basename = model.BuildBasename();
        if (basename != model.Basename)
        {
            await RemoveOutputInfoAsync(model);
            model.Basename = basename;
        }
        foreach (var file in model.Files)
        {
            file.Basename = basename;
            var filename = file.BuildFilename();
            if (filename == file.Filename) continue; // No rename
            
            // TODO: Check for collisions
            renameMap.Add(file.Filename, filename);
            
            file.Filename = filename;
        }

        // Apply the rename operations
        foreach (var (oldFilename, newFilename) in renameMap)
        {
            var oldPath = Path.Combine(OutputPath, oldFilename);
            var newPath = Path.Combine(OutputPath, newFilename);
            if (!File.Exists(oldPath) || File.Exists(newPath)) continue;
            File.Move(oldPath, newPath);
        }
        
        await WriteOutputInfoAsync(model);
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
        await foreach (var (filename, info) in OutputInfoSerializer.DeserializeFromDirectoryAsync(OutputPath))
        {
            var model = new OutputModel(info, filename);
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
    
    /// <summary>
    /// Writes the given output info file to the output directory.
    /// </summary>
    /// <param name="model">The output info file.</param>
    private async Task WriteOutputInfoAsync(OutputModel model)
    {
        var path = Path.Combine(OutputPath, $"{model.Basename}.json");
        await OutputInfoSerializer.SerializeAsync(path, model.Info);
    }
    
    /// <summary>
    /// Removes the given output info file from the output directory.
    /// </summary>
    /// <param name="outputInfo">The output info file.</param>
    private Task RemoveOutputInfoAsync(OutputModel outputInfo)
    {
        var path = Path.Combine(OutputPath, $"{outputInfo.Basename}.json");
        File.Delete(path);
        return Task.CompletedTask;
    }
}