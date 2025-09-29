using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using MediaLib.Output;

namespace MediaRipper.Models.Outputs;

public class OutputModel : ObservableObject
{
    /// <summary>
    /// Gets the output file data.
    /// </summary>
    public OutputDefinition Definition { get; }

    public OutputModel(OutputDefinition definition, string basename)
    {
        Definition = definition;
        _basename = basename;
        Files = definition.Files.Select(f => new OutputFileModel(f, basename)).ToArray();
    }

    /// <summary>
    /// Gets the file models.
    /// </summary>
    public OutputFileModel[] Files { get; }
    
    /// <inheritdoc cref="Basename"/>
    private string _basename;

    /// <summary>
    /// Gets and sets the base filename of the input file (without any extension).
    /// </summary>
    public string Basename
    {
        get => _basename;
        set => SetProperty(ref _basename, value);
    }
    
    #region Metadata
    
    public string BuildBasename()
    {
        var builder = new StringBuilder();
        if (Definition.MediaInfo.Season.HasValue && Definition.MediaInfo.Episode.HasValue)
        {
            builder.Append($"S{Definition.MediaInfo.Season.Value:00}E{Definition.MediaInfo.Episode.Value:00} ");
        }

        builder.Append(Definition.MediaInfo.Name);
        return builder.ToString();
    }
    
    #endregion Metadata
    
    #region Progress
    
    /// <inheritdoc cref="Status"/>
    private OutputStatus _status;

    /// <summary>
    /// Gets and sets the status.
    /// </summary>
    public OutputStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
    
    /// <inheritdoc cref="Progress"/>
    private double _progress;

    /// <summary>
    /// Gets and sets the progress of the current export.
    /// </summary>
    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }
    
    #endregion Progress
}