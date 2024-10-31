using System.Text;

namespace Infrastructure.Utilities
{
    public static class HexDumper
    {
        /// <summary>
        /// Converts byte data into a hex dump format string, showing hexadecimal and ASCII representation.
        /// </summary>
        /// <param name="data">The byte array to be dumped.</param>
        /// <param name="bytesPerLine">Optional number of bytes per line, default is 16.</param>
        /// <returns>Formatted hex dump string.</returns>
        public static string Dump(byte[] data, int bytesPerLine = 16)
        {
            if (data == null || data.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();
            int dataLength = data.Length;

            for (int i = 0; i < dataLength; i += bytesPerLine)
            {
                // Offset
                sb.Append($"{i:X8}  "); // Offset in hexadecimal (8 characters wide)

                // Hexadecimal Representation
                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (i + j < dataLength)
                        sb.Append($"{data[i + j]:X2} ");
                    else
                        sb.Append("   "); 
                }

                sb.Append(" "); 

                // ASCII Representation
                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (i + j < dataLength)
                    {
                        var ch = data[i + j];
                        // Printable ASCII range 32-126; otherwise, use '.'
                        sb.Append(ch >= 32 && ch <= 126 ? (char)ch : '.');
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
