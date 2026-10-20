using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife
{
    public partial class Form1
    {
        private Animal editingAnimal = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            dgvAnimals.CellClick += dgvAnimals_CellClick;
            btnSearch.Click += btnSearch_Click;
            btnSaveChanges.Click += btnSaveChanges_Click;
            btnClear.Click += btnClear_Click;
        }

        // enter edit mode for a specific animal
        private void EnterEditMode(Animal animal)
        {
            editingAnimal = animal;

            txtAnimalId.Text = animal.AnimalId;
            txtName.Text = animal.Name;
            txtSpecies.Text = animal.Species;
            txtAge.Text = animal.Age.ToString();
            txtRecoveryScore.Text = animal.RecoveryScore.ToString();

            txtAnimalId.ReadOnly = true; // the ID cannot be changed after it is saved
            btnAdd.Enabled = false;
            btnSaveChanges.Enabled = true;
            txtName.Focus();
        }

        // exit edit mode and reset the form
        private void ExitEditMode()
        {
            editingAnimal = null;

            ClearInputs(); 
            txtSearchId.Clear();

            txtAnimalId.ReadOnly = false;
            btnAdd.Enabled = true;
            btnSaveChanges.Enabled = false;
        }

        // Finds an animal by its ID in the list of animals
        private Animal FindAnimalById(string id)
        {
            string wanted = (id ?? "").Trim();

            foreach (Animal animal in animals)
            {
                if (string.Equals(animal.AnimalId, wanted, StringComparison.OrdinalIgnoreCase))
                {
                    return animal;
                }
            }

            return null;
        }

        // Load clicked grid row into the input fields for editing
        private void dgvAnimals_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Animal clicked = dgvAnimals.Rows[e.RowIndex].DataBoundItem as Animal;
            if (clicked == null) return;

            Animal animal = FindAnimalById(clicked.AnimalId);
            if (animal != null)
            {
                EnterEditMode(animal);
            }
        }

        // Selects the grid row for the given animal ID and scrolls it into view 
        private void HighlightAnimalInGrid(Animal animal)
        {
            foreach (DataGridViewRow row in dgvAnimals.Rows)
            {
                string rowId = (row.DataBoundItem as Animal)?.AnimalId ?? row.Cells[0].Value?.ToString();

                if (string.Equals(rowId, animal.AnimalId, StringComparison.OrdinalIgnoreCase))
                {
                    dgvAnimals.ClearSelection();
                    dgvAnimals.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    return;
                }
            }
        }

        // Searches for the typed animal ID and loads it into the input fields for editing
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearchId.Text.Trim();

            if (searchText.Length == 0)
            {
                MessageBox.Show("Please enter an Animal ID to search.", "Search Animal",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSearchId.Focus();
                return;
            }

            Animal animal = FindAnimalById(searchText);

            if (animal == null)
            {
                MessageBox.Show($"No animal found with ID: {searchText}", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSearchId.Focus();
                txtSearchId.SelectAll();
                return;
            }

            EnterEditMode(animal);

        }

        // Validates the input fields and saves changes to the currently edited animal
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (editingAnimal == null)
            {
                MessageBox.Show("No animal is currently being edited. Load an animal first by clicking on a row in the grid OR by using the search functionality.", "Save Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ValidationResult result = AnimalValidator.Validate(
                txtAnimalId.Text,
                txtName.Text,
                txtSpecies.Text,
                txtAge.Text,
                txtRecoveryScore.Text,
                animals,
                checkId: false
            );

            if (!result.IsValid)
            {
                MessageBox.Show(result.ErrorMessage, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FocusField(result.ErrorField);
                return;
            }

            // Save the old values in case we need to revert
            string oldName = editingAnimal.Name;
            string oldSpecies = editingAnimal.Species;
            int oldAge = editingAnimal.Age;
            int oldScore = editingAnimal.RecoveryScore;

            editingAnimal.Name = result.Name;
            editingAnimal.Species = result.Species;
            editingAnimal.Age = result.Age;
            editingAnimal.RecoveryScore = result.RecoveryScore;

            // Save changes to the file and handle any errors
            if (!fileHandler.SaveAll(animals))
            {
                editingAnimal.Name = oldName;
                editingAnimal.Species = oldSpecies;
                editingAnimal.Age = oldAge;
                editingAnimal.RecoveryScore = oldScore;

                MessageBox.Show("Failed to save changes to the file. Please try again.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"{editingAnimal.AnimalId} ({editingAnimal.Name}) has been updated successfully.\n\n"
                + $"Status: {editingAnimal.Status}\nHousing Unit: {editingAnimal.HousingUnit}", "Animal Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ExitEditMode();
            RefreshGrid();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ExitEditMode();
        }

    }
}
