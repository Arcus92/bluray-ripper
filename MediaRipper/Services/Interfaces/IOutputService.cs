using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MediaLib.Models;
using MediaLib.Output;
using MediaRipper.Models.Output;

namespace MediaRipper.Services.Interfaces;

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
    /// <param name="definition"></param>
    Task<OutputModel> AddAsync(OutputDefinition definition);

    /// <summary>
    /// Removes the given file from the list.
    /// </summary>
    /// <param name="model">The output info.</param>
    Task RemoveAsync(OutputModel model);

    /// <summary>
    /// Updates the output file and writes the json to disk.
    /// </summary>
    /// <param name="model">The output info.</param>
    Task UpdateAsync(OutputModel model);
    
    /// <summary>
    /// Gets a file from the loaded output directory if found.
    /// </summary>
    /// <param name="identifier">The media identifier.</param>
    /// <returns>Returns the output info if found.</returns>
    OutputModel? GetByIdentifier(MediaIdentifier identifier);
    
    /// <summary>
    /// Updates the status of an output, by checking the existing files in the output directory.
    /// </summary>
    /// <param name="model">The model to check and update.</param>
    void UpdateStatus(OutputModel model);
}