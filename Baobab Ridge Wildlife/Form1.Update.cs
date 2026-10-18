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
    }
}
