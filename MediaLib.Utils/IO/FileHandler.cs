using System.Text;

namespace MediaLib.Utils.IO;

public static class FileHandler
{
    /// <summary>
    /// Handles writing to a file from an async method. The filename will be written to a .tmp file until writing is
    /// complete. Then the file is renamed to the <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The output filename.</param>
    /// <param name="handler">The async handler.</param>
    public static async Task HandleAsync(string path, Func<string, Task> handler)
    {
        if (File.Exists(path))
            throw new ArgumentException($"File '{path}' already exists!");
        
        var pathTmp = $"{path}.tmp";
        if (File.Exists(pathTmp)) 
            File.Delete(pathTmp);

        try
        {
            await handler(pathTmp);
        }
        catch (Exception e)
        {
            File.Delete(pathTmp);
            Console.WriteLine(e);
            return;
        }
        
        File.Move(pathTmp, path);
    }

    /// <summary>
    /// Returns a viewable filesize text.
    /// </summary>
    /// <param name="fileSize">The filesize in bytes.</param>
    /// <returns></returns>
    public static string FileSizeToText(long fileSize)
    {
        var size = (double)fileSize;
        var ext = "b";
        if (size > 1024)
        {
            size /= 1024;
            ext = "kb";
        }
        if (size > 1024)
        {
            size /= 1024;
            ext = "mb";
        }
        if (size > 1024)
        {
            size /= 1024;
            ext = "gb";
        }
        if (size > 1024)
        {
            size /= 1024;
            ext = "tb";
        }

        return $"{size:0.00} {ext}";
    }

    /// <summary>
    /// Removes invalid characters from the given file name.
    /// </summary>
    /// <param name="fileName">The original file name.</param>
    /// <returns>Returns a save file name.</returns>
    public static string RemoveInvalidCharsFromFilename(string fileName)
    {
        var builder = new StringBuilder(fileName);
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            builder.Replace(c, '-');
        }
        return builder.ToString();
    }
}