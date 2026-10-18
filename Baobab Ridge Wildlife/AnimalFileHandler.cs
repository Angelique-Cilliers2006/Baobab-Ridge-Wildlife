using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Baobab_Ridge_Wildlife
{
    /// <summary>
    /// Reads and rewrites animals.txt. Used by Delete (rewrite the whole file) and the Summary
    /// (re-read the file so the report uses the data currently in animals.txt).
    /// </summary>
    public class AnimalFileHandler
    {
        // UTF-8 without a byte-order mark, as the brief asks for plain UTF-8 text.
        private static readonly Encoding FileEncoding = new UTF8Encoding(false);

        private readonly string filePath;

        /// <summary>How many malformed lines were skipped during the last LoadAll.</summary>
        public int SkippedCount { get; private set; }

        /// <summary>A friendly message describing the last file error, or "" if there was none.</summary>
        public string LastError { get; private set; } = "";

        /// <summary>Points the handler at animals.txt in the same folder as the running .exe.</summary>
        public AnimalFileHandler()
        {
            // Assign the field (not a new local variable) so LoadAll and SaveAll know which file to use.
            filePath = Path.Combine(AppContext.BaseDirectory, "animals.txt");
        }

        /// <summary>
        /// Loads every valid animal from animals.txt and counts the malformed lines it skips.
        /// Creates an empty file if it is missing. Returns null if the file cannot be read.
        /// </summary>
        public List<Animal> LoadAll()
        {
            List<Animal> animals = new List<Animal>();
            SkippedCount = 0;
            LastError = "";

            try
            {
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "", FileEncoding);
                    return animals;
                }

                foreach (string line in File.ReadAllLines(filePath, FileEncoding))
                {
                    // Blank lines are ignored rather than counted as skipped records.
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // Animal.TryParse returns false for a malformed line (wrong field count, bad ID, age or score).
                    if (Animal.TryParse(line, out Animal animal))
                    {
                        animals.Add(animal);
                    }
                    else
                    {
                        SkippedCount++;
                    }
                }

                return animals;
            }
            catch (UnauthorizedAccessException)
            {
                LastError = "The program does not have permission to read animals.txt.";
                return null;
            }
            catch (IOException)
            {
                LastError = "animals.txt could not be read. It may be open in another program.";
                return null;
            }
        }

        /// <summary>
        /// Rewrites animals.txt with the given animals, one line each, in list order.
        /// Returns false and sets LastError if the file cannot be written.
        /// </summary>
        public bool SaveAll(List<Animal> animals)
        {
            LastError = "";

            try
            {
                List<string> lines = new List<string>();

                foreach (Animal animal in animals)
                {
                    lines.Add(animal.ToFileLine());
                }

                File.WriteAllLines(filePath, lines, FileEncoding);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                LastError = "animals.txt could not be saved. It may be read-only.";
                return false;
            }
            catch (IOException)
            {
                LastError = "animals.txt could not be saved. It may be open in another program.";
                return false;
            }
        }
    }
}
