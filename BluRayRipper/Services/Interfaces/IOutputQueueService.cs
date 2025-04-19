using System.Threading.Tasks;
using BluRayLib.Ripper.BluRays.Export;
using BluRayRipper.Models.Output;

namespace BluRayRipper.Services.Interfaces;

public interface IOutputQueueService
{
    /// <summary>
    /// Starts the queue.
    /// </summary>
    Task StartAsync();
    
    /// <summary>
    /// Stops the queue.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Maps the output info to an export option object that can be used by the exporter class.
    /// </summary>
    /// <param name="outputFile">The output info.</param>
    /// <returns>Returns the title export options that can be used by the exporter class.</returns>
    TitleExportOptions BuildExportOptionsFormOutputFile(OutputFile outputFile);
}