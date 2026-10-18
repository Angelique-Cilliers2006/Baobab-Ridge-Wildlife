using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife
{
    
    /// Main window. Person A owns the layout and Feature 1 (Add).
    /// Other features go in their own partial files (Form1.View.cs, Form1.Update.cs,
    /// Form1.Delete.cs, Form1.Summary.cs) so the team does not edit the same lines.
    
    public partial class Form1 : Form
    {
        // animals.txt sits beside the running .exe, so no computer-specific paths are used.
        private static readonly string DataFilePath = Path.Combine(AppContext.BaseDirectory, "animals.txt");

        // In-memory copy of the valid records in animals.txt, kept in file order.
        private readonly List<Animal> animals = new List<Animal>();

        // Animal details (used by Add and Update)
        private readonly TextBox txtAnimalId = new TextBox();
        private readonly TextBox txtName = new TextBox();
        private readonly TextBox txtSpecies = new TextBox();
        private readonly TextBox txtAge = new TextBox();
        private readonly TextBox txtRecoveryScore = new TextBox();
        private readonly Button btnAdd = new Button();
        private readonly Button btnSaveChanges = new Button();   // wired by Person C
        private readonly Button btnClear = new Button();         // wired by Person C

        // Search (Person C)
        private readonly TextBox txtSearchId = new TextBox();
        private readonly Button btnSearch = new Button();

        // Delete and Summary (Person D)
        private readonly Button btnDelete = new Button();
        private readonly Button btnGenerateSummary = new Button();
        private readonly TextBox txtSummary = new TextBox();

        // Grid (Person B)
        private readonly DataGridView dgvAnimals = new DataGridView();

        /// <summary>Builds all controls on the form and wires up Person A's events.</summary>
        public Form1()
        {
            InitializeComponent(); // from Form1.Designer.cs

            Text = "Baobab Ridge Wildlife Rehabilitation Records System";
            ClientSize = new Size(1000, 610);
            MinimumSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterScreen;

            BuildAnimalDetailsGroup();
            BuildSearchGroup();
            BuildActionButtons();
            BuildSummaryBox();
            BuildAnimalGrid();

            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;                     // Person D, in Form1.PersonD.cs
            btnGenerateSummary.Click += btnGenerateSummary_Click;   // Person D, in Form1.PersonD.cs
            Load += Form1_Load; // defined in Form1.View.cs
        }

        // ---------------- Layout ----------------

        ///Creates the Animal Details box with the five inputs and the Add / Save Changes / Clear buttons.
        private void BuildAnimalDetailsGroup()
        {
            var grpAnimalDetails = new GroupBox { Text = "Animal Details", Location = new Point(12, 12), Size = new Size(300, 240) };

            AddLabelledTextBox(grpAnimalDetails, "Animal ID:", txtAnimalId, 28);
            AddLabelledTextBox(grpAnimalDetails, "Name:", txtName, 60);
            AddLabelledTextBox(grpAnimalDetails, "Species:", txtSpecies, 92);
            AddLabelledTextBox(grpAnimalDetails, "Age (years):", txtAge, 124);
            AddLabelledTextBox(grpAnimalDetails, "Recovery Score:", txtRecoveryScore, 156);

            ConfigureButton(btnAdd, "Add", new Point(12, 195), 85);
            ConfigureButton(btnSaveChanges, "Save Changes", new Point(102, 195), 100);
            ConfigureButton(btnClear, "Clear", new Point(207, 195), 80);
            btnSaveChanges.Enabled = false; // only enabled in edit mode (Feature 3)
            grpAnimalDetails.Controls.AddRange(new Control[] { btnAdd, btnSaveChanges, btnClear });

            Controls.Add(grpAnimalDetails);
        }

        /// Creates the Search by ID box used by Feature 3.
        private void BuildSearchGroup()
        {
            var grpSearch = new GroupBox { Text = "Search by Animal ID", Location = new Point(12, 260), Size = new Size(300, 62) };
            txtSearchId.Location = new Point(12, 26);
            txtSearchId.Width = 170;
            ConfigureButton(btnSearch, "Search", new Point(190, 23), 95);
            grpSearch.Controls.AddRange(new Control[] { txtSearchId, btnSearch });
            Controls.Add(grpSearch);
        }

        ///Creates the Delete and Generate Summary buttons used by Features 4 and 5.
        private void BuildActionButtons()
        {
            ConfigureButton(btnDelete, "Delete", new Point(12, 332), 145);
            ConfigureButton(btnGenerateSummary, "Generate Summary", new Point(167, 332), 145);
            Controls.AddRange(new Control[] { btnDelete, btnGenerateSummary });
        }

        ///Creates the read-only box where Feature 5 shows the summary on the form.
        private void BuildSummaryBox()
        {
            txtSummary.Location = new Point(12, 374);
            txtSummary.Size = new Size(300, 224);
            txtSummary.Multiline = true;
            txtSummary.ReadOnly = true;
            txtSummary.ScrollBars = ScrollBars.Vertical;
            txtSummary.Font = new Font("Consolas", 9F);
            txtSummary.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Controls.Add(txtSummary);
        }

        /// Creates the read-only, full-row-select grid that shows all animals.
        private void BuildAnimalGrid()
        {
            dgvAnimals.Location = new Point(325, 12);
            dgvAnimals.Size = new Size(663, 586);
            dgvAnimals.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvAnimals.ReadOnly = true;
            dgvAnimals.AllowUserToAddRows = false;
            dgvAnimals.AllowUserToDeleteRows = false;
            dgvAnimals.AllowUserToResizeRows = false;
            dgvAnimals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAnimals.MultiSelect = false;
            dgvAnimals.RowHeadersVisible = false;
            dgvAnimals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(dgvAnimals);
        }

        /// Adds a label and a text box on one row inside a group box.
        private static void AddLabelledTextBox(Control parent, string labelText, TextBox textBox, int top)
        {
            parent.Controls.Add(new Label { Text = labelText, Location = new Point(12, top + 3), AutoSize = true });
            textBox.Location = new Point(130, top);
            textBox.Width = 155;
            parent.Controls.Add(textBox);
        }

        ///Sets a button's text, position and size.
        private static void ConfigureButton(Button button, string text, Point location, int width)
        {
            button.Text = text;
            button.Location = location;
            button.Size = new Size(width, 30);
        }

        // ---------------- Feature 1: Add a new animal ----------------

        
        /// Validates the inputs, appends the new animal to animals.txt, shows its status and
        /// housing unit, refreshes the grid and clears the boxes. Nothing is saved if any rule fails.
       
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ValidationResult result = AnimalValidator.Validate(
                txtAnimalId.Text, txtName.Text, txtSpecies.Text, txtAge.Text, txtRecoveryScore.Text, animals);

            if (!result.IsValid)
            {
                MessageBox.Show(result.ErrorMessage, "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusField(result.ErrorField);
                return;
            }

            // The constructor calls the classification method to set Status and Housing Unit.
            var newAnimal = new Animal(result.AnimalId, result.Name, result.Species, result.Age, result.RecoveryScore);

            // Only add to the list once the file write succeeds, so list and file stay the same.
            if (!AppendAnimalToFile(newAnimal)) return;

            animals.Add(newAnimal);
            RefreshGrid(); // defined in Form1.View.cs

            MessageBox.Show(
                $"{newAnimal.AnimalId} ({newAnimal.Name}) was added.\n\n" +
                $"Status: {newAnimal.Status}\nHousing Unit: {newAnimal.HousingUnit}",
                "Animal added", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearInputs();
        }

       
        /// Appends one line to animals.txt, leaving existing lines untouched.
        /// Returns false and shows a friendly message if the file cannot be written.
        
        private bool AppendAnimalToFile(Animal animal)
        {
            try
            {
                // If the last line has no line break, add one first so the new record starts on its own line.
                string prefix = File.Exists(DataFilePath) && !FileEndsWithNewLine(DataFilePath) ? Environment.NewLine : "";
                File.AppendAllText(DataFilePath, prefix + animal.ToFileLine() + Environment.NewLine, new UTF8Encoding(false));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                ShowFileError("animals.txt is read-only or you do not have permission to change it. The animal was not saved.");
            }
            catch (DirectoryNotFoundException)
            {
                ShowFileError("The folder for animals.txt could not be found. The animal was not saved.");
            }
            catch (IOException)
            {
                ShowFileError("animals.txt is being used by another program. Close it and try again. The animal was not saved.");
            }
            return false;
        }

        ///Returns true if the file is empty or its last character is a line break.
        private static bool FileEndsWithNewLine(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (stream.Length == 0) return true;
                stream.Seek(-1, SeekOrigin.End);
                return stream.ReadByte() == '\n';
            }
        }

        ///Moves the cursor to the input box that failed validation and selects its text.
        private void FocusField(AnimalField field)
        {
            TextBox box;
            switch (field)
            {
                case AnimalField.AnimalId: box = txtAnimalId; break;
                case AnimalField.Name: box = txtName; break;
                case AnimalField.Species: box = txtSpecies; break;
                case AnimalField.Age: box = txtAge; break;
                default: box = txtRecoveryScore; break;
            }
            box.Focus();
            box.SelectAll();
        }

        ///>Empties the five input boxes and puts the cursor back in Animal ID. Person C reuses this for Clear.
        private void ClearInputs()
        {
            txtAnimalId.Clear();
            txtName.Clear();
            txtSpecies.Clear();
            txtAge.Clear();
            txtRecoveryScore.Clear();
            txtAnimalId.Focus();
        }

        ///Shows a friendly file error message (no stack trace) so the program keeps running
        private void ShowFileError(string message)
        {
            MessageBox.Show(message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}