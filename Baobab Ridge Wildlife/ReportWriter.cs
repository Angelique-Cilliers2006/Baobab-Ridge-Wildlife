using System;
using System.IO;
using System.Text;

namespace Baobab_Ridge_Wildlife
{
    /// <summary> writes the summary report to summary.txt beside the .exe.</summary>
    public class ReportWriter
    {
        /// <summary>A friendly message describing the last file error, or "" if there was none.</summary>
        public string LastError { get; private set; } = "";

        /// <summary>Full path of summary.txt in the same folder as the running .exe.</summary>
        public string FilePath { get; } = Path.Combine(AppContext.BaseDirectory, "summary.txt");

        /// <summary>Overwrites summary.txt with the report text. Returns false if the file cannot be written.</summary>
        public bool Save(string reportText)
        {
            LastError = "";

            try
            {
                // WriteAllText replaces the file each time; it never appends.
                File.WriteAllText(FilePath, reportText, new UTF8Encoding(false));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                LastError = "The program does not have permission to save summary.txt. It may be read-only.";
                return false;
            }
            catch (IOException)
            {
                LastError = "summary.txt could not be saved. It may be open in another program.";
                return false;
            }
        }
    }
}
