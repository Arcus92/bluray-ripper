using System.Threading.Tasks;
using BluRayLib.Ripper.BluRays.Export;
using BluRayLib.Ripper.Output;
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
}