using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BluRayLib.Ripper;
using BluRayLib.Ripper.BluRays;
using BluRayLib.Ripper.BluRays.Export;
using BluRayRipper.Models;
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
    ObservableCollection<OutputModel> Items { get; }

    /// <summary>
    /// Creates a new output file to the list.
    /// </summary>
    /// <param name="outputFile"></param>
    Task<OutputModel> AddAsync(OutputFile outputFile);

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
    /// Gets a file from the loaded output directory if found.
    /// </summary>
    /// <param name="diskName">The disk name.</param>
    /// <param name="playlistId">The playlist id.</param>
    /// <returns>Returns the output info if found.</returns>
    OutputModel? GetByPlaylist(string diskName, ushort playlistId);
    
    /// <summary>
    /// Creates an output info definition from the given title and output options.
    /// </summary>
    /// <param name="title">The title / playlist to export.</param>
    /// <param name="videoFormat">The video format.</param>
    /// <param name="codecOptions">The codec options.</param>
    /// <param name="baseName">The initial base name.</param>
    /// <returns>Returns the output info instance.</returns>
    OutputFile BuildOutputInfo(TitleData title, VideoFormat videoFormat, CodecOptions codecOptions, string baseName);

    /// <summary>
    /// Maps the output info to an export option object that can be used by the exporter class.
    /// </summary>
    /// <param name="outputFile">The output info.</param>
    /// <returns>Returns the title export options that can be used by the exporter class.</returns>
    TitleExportOptions BuildExportOptionsFormOutputInfo(OutputFile outputFile);
}