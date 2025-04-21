using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BluRayLib.Ripper.BluRays.Export;
using BluRayLib.Ripper.Output;
using BluRayRipper.Models.Output;

namespace BluRayRipper.Services.Interfaces;

public interface IOutputService
{
    /// <summary>
    /// Gets the loaded output path.
    /// </summary>
    string OutputPath { get; }

    /// <summary>
    /// Opens and loads the given output path.
    /// </summary>
    /// <param name="outputPath">The output path to open.</param>
    Task OpenAsync(string outputPath);

    /// <summary>
    /// Refreshes the file list.
    /// </summary>
    /// <returns></returns>
    Task RefreshAsync();
    
    /// <summary>
    /// Gets the list of loaded output files.
    /// </summary>
    ObservableCollection<OutputModel> Outputs { get; }

    /// <summary>
    /// Creates a new output file to the list.
    /// </summary>
    /// <param name="info"></param>
    Task<OutputModel> AddAsync(OutputInfo info);

    /// <summary>
    /// Removes the given file from the list.
    /// </summary>
    /// <param name="output">The output info.</param>
    Task RemoveAsync(OutputModel output);

    /// <summary>
    /// Updates the output file and writes the json to disk.
    /// </summary>
    /// <param name="output">The output info.</param>
    Task UpdateAsync(OutputModel output);
    
    /// <summary>
    /// Renames the given output file.
    /// </summary>
    /// <param name="outputInfo"></param>
    /// <param name="nameMap"></param>
    /// <returns></returns>
    Task RenameAsync(OutputInfo outputInfo, TitleNameMap nameMap);
    
    /// <summary>
    /// Gets a file from the loaded output directory if found.
    /// </summary>
    /// <param name="type">The source type.</param>
    /// <param name="diskName">The disk name.</param>
    /// <param name="playlistId">The playlist id.</param>
    /// <returns>Returns the output info if found.</returns>
    OutputModel? GetBySource(OutputSourceType type, string diskName, ushort playlistId);
    
    /// <summary>
    /// Gets a file from the loaded output directory if found.
    /// </summary>
    /// <param name="source">The output source to look for.</param>
    /// <returns>Returns the output info if found.</returns>
    OutputModel? GetBySource(OutputSource source) => GetBySource(source.Type, source.DiskName, source.PlaylistId);

    /// <summary>
    /// Updates the status of an output, by checking the existing files in the output directory.
    /// </summary>
    /// <param name="model">The model to check and update.</param>
    void UpdateStatus(OutputModel model);
}