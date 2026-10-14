using System;
using System.ComponentModel;

namespace Baobab_Ridge_Wildlife
{
    /// <summary>
    /// One animal in the centre's care.
    /// Status and HousingUnit are never typed by the user; they are always
    /// calculated from RecoveryScore by the Classify method.
    /// </summary>
    public class Animal
    {
        public const int MinScore = 0;
        public const int MaxScore = 100;

        /// <summary>All five statuses in band order (Person D uses this for the summary).</summary>
        public static readonly string[] AllStatuses =
            { "Critical", "Serious", "Stable", "Recovering", "Release-Ready" };

        // Backing field so that setting the score re-runs the classification.
        private int recoveryScore;

        // DisplayName sets the grid column header text.
        [DisplayName("Animal ID")]
        public string AnimalId { get; }   // no setter: the ID cannot change after saving

        public string Name { get; set; }
        public string Species { get; set; }
        public int Age { get; set; }

        [DisplayName("Recovery Score")]
        public int RecoveryScore
        {
            get { return recoveryScore; }
            set
            {
                recoveryScore = value;
                ApplyClassification(); // status and housing always follow the score
            }
        }

        public string Status { get; private set; }

        [DisplayName("Housing Unit")]
        public string HousingUnit { get; private set; }

        /// <summary>Creates an animal and calculates its status and housing unit from the score.</summary>
        public Animal(string animalId, string name, string species, int age, int recoveryScore)
        {
            AnimalId = animalId;
            Name = name;
            Species = species;
            Age = age;
            RecoveryScore = recoveryScore; // the setter calls ApplyClassification
        }

        /// <summary>
        /// The single classification method (brief Section 4).
        /// Returns the status and housing unit for a Recovery Score from 0 to 100.
        /// Add and Update both reach it through the RecoveryScore setter, so the rules exist in one place.
        /// </summary>
        public static (string Status, string HousingUnit) Classify(int score)
        {
            if (score < MinScore || score > MaxScore)
                throw new ArgumentOutOfRangeException(nameof(score), "Recovery Score must be between 0 and 100.");

            // Scores are whole numbers, so each band starts one above the previous band's end.
            if (score <= 19) return ("Critical", "Intensive Care Unit");
            if (score <= 39) return ("Serious", "High-Dependency Ward");
            if (score <= 59) return ("Stable", "Recovery Ward");
            if (score <= 79) return ("Recovering", "Outdoor Enclosure");
            return ("Release-Ready", "Pre-Release Camp");
        }

        /// <summary>Sets Status and HousingUnit from the current Recovery Score.</summary>
        private void ApplyClassification()
        {
            (Status, HousingUnit) = Classify(recoveryScore);
        }

        /// <summary>Returns this animal as one line of animals.txt: ID|Name|Species|Age|Score|Status|Housing Unit.</summary>
        public string ToFileLine()
        {
            return string.Join("|", AnimalId, Name, Species, Age, RecoveryScore, Status, HousingUnit);
        }

        /// <summary>
        /// Converts one line of animals.txt into an Animal.
        /// Returns false for a malformed line so the loader can skip it instead of crashing.
        /// </summary>
        public static bool TryParse(string line, out Animal animal)
        {
            animal = null;
            if (string.IsNullOrWhiteSpace(line)) return false;

            string[] fields = line.Split('|');
            if (fields.Length != 7) return false; // must be exactly seven fields

            string id = AnimalValidator.NormaliseId(fields[0]);
            string name = fields[1].Trim();
            string species = fields[2].Trim();

            if (!AnimalValidator.IsValidIdFormat(id)) return false;
            if (AnimalValidator.GetTextError(name, "Name") != null) return false;
            if (AnimalValidator.GetTextError(species, "Species") != null) return false;
            if (!AnimalValidator.TryParseWholeNumber(fields[3], AnimalValidator.MinAge, AnimalValidator.MaxAge, out int age)) return false;
            if (!AnimalValidator.TryParseWholeNumber(fields[4], MinScore, MaxScore, out int score)) return false;

            // Status and Housing Unit in the file are not trusted; they are recalculated from the score.
            animal = new Animal(id, name, species, age, score);
            return true;
        }
    }
}