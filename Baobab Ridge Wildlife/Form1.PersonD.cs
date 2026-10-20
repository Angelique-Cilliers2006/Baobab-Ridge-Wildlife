using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife
{
    
    // The buttons are created and wired in Form1.cs; the handlers live here.
    public partial class Form1
    {
        // Shared file handler for rewriting and re-reading animals.txt.
        private readonly AnimalFileHandler fileHandler = new AnimalFileHandler();

        /// <summary>
        /// Deletes the selected animal after a Yes/No confirmation, rewrites animals.txt,
        /// refreshes the grid and clears the form if that animal was loaded in it.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an animal in the grid first.", "Delete animal",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // The grid is bound to Animal objects, so read the selected row's Animal directly.
            Animal selected = dgvAnimals.SelectedRows[0].DataBoundItem as Animal;

            DeleteService deleteService = new DeleteService(animals, fileHandler);
            Animal animal = selected == null ? null : deleteService.FindById(selected.AnimalId);

            if (animal == null)
            {
                MessageBox.Show("That animal could not be found.", "Delete animal",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshGrid();
                return;
            }

            // Default button is No, so pressing Enter by accident does not delete anything.
            DialogResult answer = MessageBox.Show(
                deleteService.BuildConfirmMessage(animal),
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            if (!deleteService.Delete(animal))
            {
                MessageBox.Show(deleteService.Message, "Delete animal",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RefreshGrid();

            // If the deleted animal was loaded in the input boxes, clear them.
            if (string.Equals(txtAnimalId.Text.Trim(), animal.AnimalId, StringComparison.OrdinalIgnoreCase))
            {
                ExitEditMode();
            }

            MessageBox.Show(deleteService.Message, "Delete animal",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Re-reads animals.txt, shows the summary in the read-only text box on the form
        /// and saves it to summary.txt.
        /// </summary>
        private void btnGenerateSummary_Click(object sender, EventArgs e)
        {
            // Use the data currently in animals.txt, as the brief requires.
            List<Animal> animalsFromFile = fileHandler.LoadAll();

            if (animalsFromFile == null)
            {
                MessageBox.Show(fileHandler.LastError, "Summary report",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SummaryReport report = new SummaryReport();
            report.Calculate(animalsFromFile);
            string reportText = report.ToText();
            txtSummary.Text = reportText;

            ReportWriter writer = new ReportWriter();

            if (!writer.Save(reportText))
            {
                MessageBox.Show(writer.LastError, "Summary report",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Summary saved to summary.txt.", "Summary report",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
