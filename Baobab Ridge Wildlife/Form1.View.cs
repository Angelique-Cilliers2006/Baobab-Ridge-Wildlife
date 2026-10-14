using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Baobab_Ridge_Wildlife
{
    // Basic version so Feature 1 can be tested on its own.
    // Person B replaces this file with the full Feature 2 (skipped-line count message, full error handling).
    public partial class Form1
    {
        /// <summary>Loads animals.txt and shows it in the grid when the form opens.</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadAnimals();
            RefreshGrid();
        }

        /// <summary>Reads every valid line of animals.txt into the list; creates an empty file if missing.</summary>
        private void LoadAnimals()
        {
            animals.Clear();
            try
            {
                if (!File.Exists(DataFilePath))
                {
                    File.WriteAllText(DataFilePath, "", new UTF8Encoding(false));
                    return;
                }

                foreach (string line in File.ReadAllLines(DataFilePath, Encoding.UTF8))
                {
                    if (Animal.TryParse(line, out Animal animal))
                        animals.Add(animal);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                ShowFileError("animals.txt could not be read: " + ex.Message);
            }
        }

        /// <summary>Re-binds the grid to a fresh copy of the list so it shows current data with no duplicate rows.</summary>
        private void RefreshGrid()
        {
            dgvAnimals.DataSource = null;
            dgvAnimals.DataSource = animals.ToList();
        }
    }
}