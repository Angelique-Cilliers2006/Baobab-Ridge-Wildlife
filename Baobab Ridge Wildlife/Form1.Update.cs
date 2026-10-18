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

        // Method to enter edit mode for a specific animal
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

        // Method to exit edit mode and reset the form
        private void ExitEditMode()
        {
            editingAnimal = null;

            ClearInputs();  // method to clear all input fields
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
    }
}
