using BluRayLib.Ripper.Output;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.Models.Output;

public class OutputFileModel : ObservableObject
{
    public OutputFileModel(OutputFile file)
    {
        File = file;
    }

    /// <summary>
    /// The output file info.
    /// </summary>
    public OutputFile File { get; }

    /// <inheritdoc cref="OutputFile.Filename"/>
    public string Filename => File.Filename;
    
    /// <inheritdoc cref="Size"/>
    private long _size;

    /// <summary>
    /// Gets and sets the filesize.
    /// </summary>
    public long Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }
}