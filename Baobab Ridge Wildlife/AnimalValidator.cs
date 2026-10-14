using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Baobab_Ridge_Wildlife
{
    ///The input boxes that can fail validation, so the form can move the cursor to the right one.
    public enum AnimalField { AnimalId, Name, Species, Age, RecoveryScore }

    ///The outcome of validating the form: either an error, or the cleaned-up values.
    public class ValidationResult
    {
        public string ErrorMessage { get; set; }
        public AnimalField ErrorField { get; set; }
        public bool IsValid => ErrorMessage == null;

        public string AnimalId { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public int Age { get; set; }
        public int RecoveryScore { get; set; }
    }

    /// Validation rules from brief Section 3, shared by Add (Person A) and Update (Person C).

    public static class AnimalValidator
    {
        public const int MaxTextLength = 30;
        public const int MinAge = 0;
        public const int MaxAge = 100;

        
        /// Validates all five inputs, top to bottom, and stops at the first problem.
        /// Update passes checkId = false because the ID cannot change.
  
        public static ValidationResult Validate(string idText, string nameText, string speciesText,
                                                string ageText, string scoreText,
                                                IEnumerable<Animal> existingAnimals, bool checkId = true)
        {
            // All typed text is trimmed before it is checked.
            var result = new ValidationResult
            {
                AnimalId = NormaliseId(idText),
                Name = (nameText ?? "").Trim(),
                Species = (speciesText ?? "").Trim()
            };

            if (checkId)
            {
                string idError = GetIdError(result.AnimalId, existingAnimals);
                if (idError != null) return Fail(result, AnimalField.AnimalId, idError);
            }

            string nameError = GetTextError(result.Name, "Name");
            if (nameError != null) return Fail(result, AnimalField.Name, nameError);

            string speciesError = GetTextError(result.Species, "Species");
            if (speciesError != null) return Fail(result, AnimalField.Species, speciesError);

            if (!TryParseWholeNumber(ageText, MinAge, MaxAge, out int age))
            {
                string message = IsBlank(ageText)
                    ? "Age is required."
                    : $"Age must be a whole number between {MinAge} and {MaxAge}.";
                return Fail(result, AnimalField.Age, message);
            }
            result.Age = age;

            if (!TryParseWholeNumber(scoreText, Animal.MinScore, Animal.MaxScore, out int score))
            {
                string message = IsBlank(scoreText)
                    ? "Recovery Score is required."
                    : $"Recovery Score must be a whole number between {Animal.MinScore} and {Animal.MaxScore}.";
                return Fail(result, AnimalField.RecoveryScore, message);
            }
            result.RecoveryScore = score;

            return result;
        }

        /// Trims the ID and converts it to upper case, so "wr-0007" becomes "WR-0007".
        public static string NormaliseId(string idText)
        {
            return (idText ?? "").Trim().ToUpperInvariant();
        }

        ///Returns true if the ID is "WR-" followed by exactly four digits.
        public static bool IsValidIdFormat(string animalId)
        {
            // [0-9] instead of \d so only the digits 0-9 are accepted.
            return Regex.IsMatch(animalId ?? "", @"^WR-[0-9]{4}$");
        }

        ///Returns an error message for the Animal ID, or null if it is valid and unique.
        public static string GetIdError(string animalId, IEnumerable<Animal> existingAnimals)
        {
            if (animalId == "")
                return "Animal ID is required.";
            if (!IsValidIdFormat(animalId))
                return "Animal ID must be WR- followed by exactly 4 digits, for example WR-0007.";
            if (existingAnimals != null && existingAnimals.Any(a => a.AnimalId == animalId))
                return $"Animal ID {animalId} already exists. Each animal needs a unique ID.";
            return null;
        }

        ///Checks Name or Species: required, 1-30 characters, no pipe. Returns null if valid.
        public static string GetTextError(string text, string fieldLabel)
        {
            if (string.IsNullOrEmpty(text))
                return $"{fieldLabel} is required.";
            if (text.Length > MaxTextLength)
                return $"{fieldLabel} must be between 1 and {MaxTextLength} characters.";
            if (text.Contains('|'))
                return $"{fieldLabel} must not contain the | character.";
            return null;
        }

        /// Parses a whole number within a range. Rejects decimals, text, signs and blanks
        public static bool TryParseWholeNumber(string text, int min, int max, out int value)
        {
            // NumberStyles.None accepts digits only, so "3.5", "-1" and "abc" all fail.
            bool parsed = int.TryParse((text ?? "").Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out value);
            return parsed && value >= min && value <= max;
        }

        /// Returns true if the text is empty or only spaces.
        private static bool IsBlank(string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        ///Records which field failed and why, then returns the result.
        private static ValidationResult Fail(ValidationResult result, AnimalField field, string message)
        {
            result.ErrorField = field;
            result.ErrorMessage = message;
            return result;
        }
    }
}