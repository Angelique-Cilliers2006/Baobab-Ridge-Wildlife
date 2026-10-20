using System;
using System.Collections.Generic;
using System.Globalization;

namespace Baobab_Ridge_Wildlife
{
    /// <summary> calculates the summary figures and formats them in the summary.txt layout.</summary>
    public class SummaryReport
    {
        // Values start in column 27 of each line, matching the layout in brief section 5.2.
        private const int ValueColumn = 26;

        public int TotalAnimals { get; private set; }
        public double AverageAge { get; private set; }
        public double AverageScore { get; private set; }

        private readonly Dictionary<string, int> statusCounts = new Dictionary<string, int>();

        /// <summary>Counts the animals, works out both averages and counts how many are in each status.</summary>
        public void Calculate(List<Animal> animals)
        {
            TotalAnimals = animals.Count;
            AverageAge = 0;
            AverageScore = 0;

            // Start every status at 0 so all five are listed, even when none of the animals has that status.
            statusCounts.Clear();
            foreach (string status in Animal.AllStatuses)
            {
                statusCounts[status] = 0;
            }

            int ageTotal = 0;
            int scoreTotal = 0;

            foreach (Animal animal in animals)
            {
                ageTotal += animal.Age;
                scoreTotal += animal.RecoveryScore;

                if (statusCounts.ContainsKey(animal.Status))
                {
                    statusCounts[animal.Status]++;
                }
            }

            // Only divide when there are animals, to avoid dividing by zero.
            if (TotalAnimals > 0)
            {
                AverageAge = (double)ageTotal / TotalAnimals;
                AverageScore = (double)scoreTotal / TotalAnimals;
            }
        }

        /// <summary>Returns the report text in the exact layout required for summary.txt.</summary>
        public string ToText()
        {
            string separator = new string('-', 40);
            string averageAgeText = "N/A";
            string averageScoreText = "N/A";

            if (TotalAnimals > 0)
            {
                // InvariantCulture always uses a full stop, even on computers set to use a comma.
                averageAgeText = AverageAge.ToString("F2", CultureInfo.InvariantCulture);
                averageScoreText = AverageScore.ToString("F2", CultureInfo.InvariantCulture);
            }

            List<string> lines = new List<string>();
            lines.Add("BAOBAB RIDGE WILDLIFE REHABILITATION CENTRE");
            lines.Add("ANIMAL SUMMARY REPORT");
            lines.Add("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
            lines.Add(separator);
            lines.Add(FormatLine("Total animals:", TotalAnimals.ToString()));
            lines.Add(FormatLine("Average age (years):", averageAgeText));
            lines.Add(FormatLine("Average recovery score:", averageScoreText));
            lines.Add(separator);
            lines.Add("Animals per status:");

            foreach (string status in Animal.AllStatuses)
            {
                lines.Add(FormatLine("  " + status + ":", statusCounts[status].ToString()));
            }

            // Environment.NewLine (\r\n on Windows) so the lines also break inside the form's text box.
            return string.Join(Environment.NewLine, lines) + Environment.NewLine;
        }

        /// <summary>Pads the label so every value lines up in the same column.</summary>
        private static string FormatLine(string label, string value)
        {
            return label.PadRight(ValueColumn) + value;
        }
    }
}
