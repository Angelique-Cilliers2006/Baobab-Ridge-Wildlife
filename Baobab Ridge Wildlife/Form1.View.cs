using System;
using System.Linq;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife
{
    //Feature 2: View all animals and handle file loading.
    public partial class Form1
    {
        /// <summary>
        /// Loads valid animal records when the form opens and displays a message if any malformed records were skipped.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadAnimals();
            RefreshGrid();
        }

        /// <summary>
        /// Loads animals through AnimalFileHandler, handles file errors and informs the user if malformed records were skipped.
        /// </summary>
        private void LoadAnimals()
        {
            animals.Clear();

            //Reuses the existing handler for loading and file error handling.
            var loadedAnimals = fileHandler.LoadAll();

            //Stops if loading failed.
            if (loadedAnimals == null)
            {
                ShowFileError(fileHandler.LastError);
                return;
            }

            //Adds successfully loaded records to the form's existing list.
            animals.AddRange(loadedAnimals);

            //Informs the user if malformed records were skipped.
            if (fileHandler.SkippedCount > 0)
            {
                MessageBox.Show(
                    $"{fileHandler.SkippedCount} invalid record(s) in animals.txt " +
                    "were skipped. The remaining valid records have been loaded.",
                    "Invalid records skipped",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Rebinds the grid to a fresh list so changes appear without creating duplicate rows.
        /// </summary>
        private void RefreshGrid()
        {
            dgvAnimals.DataSource = null;
            dgvAnimals.DataSource = animals.ToList();
        }
    }
}
