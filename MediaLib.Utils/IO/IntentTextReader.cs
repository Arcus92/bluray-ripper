using System.Diagnostics.CodeAnalysis;

namespace MediaLib.Utils.IO;

/// <summary>
/// A helper class to read text files line by line with intent syntax.
/// </summary>
public class IntentTextReader
{
    public IntentTextReader(TextReader reader, int spaces)
    {
        _reader = reader;
        Spaces = spaces;
    }
    
    /// <summary>
    /// The internal text reader.
    /// </summary>
    private readonly TextReader _reader;

    /// <summary>
    /// Gets the number of whitespaces to represent one intent level.
    /// </summary>
    public int Spaces { get; }

    /// <summary>
    /// The intent of the current line.
    /// </summary>
    private int _currentIntent;

    /// <summary>
    /// Gets the current intent level. 
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// Read a new block starting at the next intent level.
    /// </summary>
    public void BeginBlock()
    {
        Level++;
    }

    /// <summary>
    /// End the current block at the current intent level.
    /// </summary>
    public void EndBlock()
    {
        Skip();
    }

    /// <summary>
    /// Skips to the end of the current block at the current intent level.
    /// </summary>
    public void Skip()
    {
        Level--;
    }


    /// <summary>
    /// Reads all properties in this block.
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<(string, string)> BeginAndReadBlockPropertiesAsync()
    {
        await foreach (var line in BeginAndReadBlockAsync())
        {
            if (TryReadProperty(line, out var name, out var value))
            {
                yield return (name, value);
            }
        }
    }

    /// <summary>
    /// Reads all properties in this block.
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<string> BeginAndReadBlockAsync()
    {
        BeginBlock();
        await foreach (var line in ReadBlockAsync())
        {
            yield return line;
        }
        EndBlock();
    }
    
    /// <summary>
    /// Reads all lines in this block.
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<string> ReadBlockAsync()
    {
        var parentIntent = Level * Spaces;

        while (true)
        {
            // Read the intent level of this line
            while (_reader.Peek() == ' ')
            {
                _currentIntent++;
                _reader.Read();
            }

            var intent = _currentIntent;
            if (intent < parentIntent)
                yield break;
            
            

            var line = await _reader.ReadLineAsync();
            _currentIntent = 0;
            if (line is null)
                yield break;
            
            // Skip unknown
            if (intent != parentIntent)
                continue;
            
            yield return line;
        }
    }
    
    private static bool TryReadProperty(string line, [MaybeNullWhen(false)] out string name, [MaybeNullWhen(false)] out string value)
    {
        var index = line.IndexOf(':');
        if (index < 0)
        {
            name = default;
            value = default;
            return false;
        }

        name = line.Substring(0, index).Trim();
        value = line.Substring(index + 1).Trim();
        return true;
    }
}