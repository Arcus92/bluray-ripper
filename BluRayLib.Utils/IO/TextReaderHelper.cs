namespace BluRayLib.Utils.IO;

public static class TextReaderHelper
{
    public static async IAsyncEnumerable<(string, int)> ReadIntentLineAsync(this TextReader reader, int parentIntent = -1)
    {
        while (true)
        {
            // Read the intent level of this line
            var intent = 0;
            while (reader.Peek() == ' ')
            {
                intent++;
                reader.Read();
            }
            if (intent <= parentIntent)
                yield break;

            var line = await reader.ReadLineAsync();
            if (line is not null)
            {
                yield return (line, intent);
            }
        }
    }
}