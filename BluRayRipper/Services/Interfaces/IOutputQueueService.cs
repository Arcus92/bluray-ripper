using System;
using System.Diagnostics.CodeAnalysis;
using BluRayRipper.Models.Output;

namespace BluRayRipper.Services.Interfaces;

public interface IOutputQueueService
{
    /// <summary>
    /// Starts the queue.
    /// </summary>
    void Start();
    
    /// <summary>
    /// Stops the queue.
    /// </summary>
    void Stop();
}