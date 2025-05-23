using System.Threading.Tasks;

namespace MediaRipper.Services.Interfaces;

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