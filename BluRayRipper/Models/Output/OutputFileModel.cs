using System.IO;
using System.Text;
using BluRayLib.Ripper.Output;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.Models.Output;

public class OutputFileModel : ObservableObject
{
    public OutputFileModel(OutputFile file, string basename)
    {
        File = file;
        
        // Split the model file name and try to analyze the different components.
        var name = Path.GetFileNameWithoutExtension(file.Filename);
        if (name.StartsWith(basename))
        {
            // This should be the default case.
            Basename = basename;
            Name = name.Substring(basename.Length);
        }
        else
        {
            // In case this is different, we just pretend there is no basename and just use the whole name as postfix.
            Basename = ""; 
            Name = name;
        }
        Extension = Path.GetExtension(file.Filename);
    }

    /// <summary>
    /// The output file info.
    /// </summary>
    public OutputFile File { get; }

    /// <inheritdoc cref="OutputFile.Filename"/>
    public string Filename
    {
        get => File.Filename;
        set => SetProperty(File.Filename, value, v => File.Filename = v);
    }
    
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
    
    #region Filename builder
    
    /// <inheritdoc cref="Basename"/>
    private string _basename = "";

    /// <summary>
    /// Gets and sets the basename.
    /// This is the start of the filename and should be equal for all sup-files of an output.
    /// </summary>
    public string Basename
    {
        get => _basename;
        set => SetProperty(ref _basename, value);
    }

    /// <inheritdoc cref="Extension"/>
    private string _extension = "";

    /// <summary>
    /// Gets and sets the file extension.
    /// This will be the end of the filename.
    /// </summary>
    public string Extension
    {
        get => _extension;
        set => SetProperty(ref _extension, value);
    }
    
    /// <inheritdoc cref="Name"/>
    private string _name = "";

    /// <summary>
    /// Gets and sets the name's postfix. This is placed between the <see cref="Basename"/> and <see cref="Extension"/>.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string BuildFilename()
    {
        var builder = new StringBuilder();
        builder.Append(_basename);
        builder.Append(_name);
        builder.Append(_extension);
        return builder.ToString();
    }
    
    #endregion Filename builder
}