namespace BluRayLib.PresentationGraphicStream;

public interface IPresentationGraphicStream
{
    /// <summary>
    /// Reads the presentation graphic stream and returns all display sets.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<DisplaySet> ReadAsync();

    /// <summary>
    /// Reads the presentation graphic stream and removes all double display sets.
    /// Some BlueRays repeat the same subtitle every few milliseconds and blow up the file size.
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<DisplaySet> ReadAndCleanUpAsync()
    {
        DisplaySet? lastDisplaySet = null;
        
        await foreach (var displaySet in ReadAsync())
        {
            // Ignores the previous display set with the exact same data.
            if (lastDisplaySet is not null && ObjectDefinitionIsEqual(lastDisplaySet, displaySet))
            {
                continue;
            }
            
            yield return displaySet;
            lastDisplaySet = displaySet;
        }
    }

    /// <summary>
    /// Compares the image data of the two display sets and returns if they are equal.
    /// </summary>
    /// <param name="a">Display set A</param>
    /// <param name="b">Display set B</param>
    /// <returns></returns>
    private static bool ObjectDefinitionIsEqual(DisplaySet a, DisplaySet b)
    {
        if (a.ObjectDefinitions.Count != b.ObjectDefinitions.Count)
            return false;

        for (var i = 0; i < a.ObjectDefinitions.Count; i++)
        {
            var objectA = a.ObjectDefinitions[i];
            var objectB = b.ObjectDefinitions[i];

            if (objectA.Data.Length != objectB.Data.Length)
                return false;

            for (var p = 0; p < objectA.Data.Length; p++)
            {
                if (objectA.Data[p] != objectB.Data[p])
                    return false;
            }
        }
        
        return true;
    }
}