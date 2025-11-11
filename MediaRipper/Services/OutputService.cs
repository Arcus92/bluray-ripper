using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediaLib.Models;
using MediaLib.Output;
using MediaRipper.Models.Outputs;
using MediaRipper.Services.Interfaces;

namespace MediaRipper.Services;

/// <summary>
/// Handling the output directory. Can scan the output directory for existing exports.
/// </summary>
public class OutputService : IOutputService
{
    private readonly IMediaProviderService _mediaProviderService;

    /// <summary>
    /// Handling the output directory. Can scan the output directory for existing exports.
    /// </summary>
    public OutputService(IMediaProviderService mediaProviderService)
    {
        _mediaProviderService = mediaProviderService;
        _mediaProviderService.Changed += OnMediaProviderServiceChanged;
    }

    /// <summary>
    /// The media provider changed the source.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMediaProviderServiceChanged(object? sender, EventArgs e)
    {
        UpdateStatus();
    }

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
    public async Task<OutputModel> AddAsync(OutputDefinition definition)
    {
        // Prevent an item to be added twice.
        var model = GetByIdentifier(definition.Identifier);
        if (model is not null) return model;

        model = new OutputModel(definition, definition.MediaInfo.Name);
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
    public OutputModel? GetByIdentifier(MediaIdentifier identifier)
    {
        return Outputs.FirstOrDefault(m => m.Definition.Identifier.Equals(identifier));
    }

    /// <inheritdoc />
    public async Task RefreshAsync()
    {
        Outputs.Clear();
        await foreach (var (filename, info) in OutputDefinitionSerializer.DeserializeFromDirectoryAsync(OutputPath))
        {
            var model = new OutputModel(info, filename);
            UpdateStatus(model);
            Outputs.Add(model);
        }
    }

    /// <summary>
    /// Uploads the status for all current items.
    /// </summary>
    private void UpdateStatus()
    {
        foreach (var model in Outputs)
        {
            UpdateStatus(model);
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
            model.Status = OutputStatus.Queued;
            
            // Queued output file, but from a different disk.
            if (!_mediaProviderService.Contains(model.Definition.Identifier))
            {
                model.Status = OutputStatus.Missing;
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
        await OutputDefinitionSerializer.SerializeAsync(path, model.Definition);
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