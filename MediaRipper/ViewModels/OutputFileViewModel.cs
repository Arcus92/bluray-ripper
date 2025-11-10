using System.Text;
using Avalonia.Controls;
using MediaLib.Output;
using MediaRipper.Models.Outputs;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class OutputFileViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the output file.
    /// </summary>
    public OutputFileModel Model { get; }
    
    public OutputFileViewModel(OutputFileModel model)
    {
        Model = model;
    }

    /// <summary>
    /// Gets the readable description of this file. The list of included streams.
    /// </summary>
    public string DisplayName
    {
        get
        {
            var builder = new StringBuilder();

            foreach (var stream in Model.File.Streams)
            {
                if (!stream.Enabled) continue;
                
                builder.Append('[');
                builder.Append(stream.Type);
                if (!string.IsNullOrEmpty(stream.LanguageCode))
                {
                    builder.Append(':');
                    builder.Append(stream.LanguageCode);

                    if (stream.Flags != OutputStreamFlags.None)
                    {
                        builder.Append(" (");
                        builder.Append(stream.Flags);
                        builder.Append(')');
                    }
                }
                builder.Append("] ");
            }
            
            return builder.ToString();
        }
    }

    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputFileView();
    }
}