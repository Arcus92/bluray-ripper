using System;
using System.Diagnostics.CodeAnalysis;
using BluRayRipper.Models.Output;

namespace BluRayRipper.Services.Interfaces;

public interface IOutputQueueService
{
    /// <summary>
    /// Clears the queue.
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Starts the queue.
    /// </summary>
    void Start();
    
    /// <summary>
    /// Stops the queue.
    /// </summary>
    void Stop();

    /// <summary>
    /// The event that is called when ever the queue state was changed.
    /// </summary>
    event EventHandler QueueProgressChanged;
    
    /// <summary>
    /// Gets the current queue progress if the output item was queued.
    /// </summary>
    /// <param name="output">The output item to get the queue from.</param>
    /// <param name="progress">The progress object for the output.</param>
    /// <returns>Returns if the output item was queued and has a progress object.</returns>
    bool TryGetProcess(OutputFile output, [MaybeNullWhen(false)] out OutputProgress progress);
}