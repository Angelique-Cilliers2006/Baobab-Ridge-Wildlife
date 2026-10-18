using System;
using System.Collections.Generic;

namespace Baobab_Ridge_Wildlife
{
    /// <summary>Feature 4: finds and deletes one animal, keeping the list and animals.txt in step.</summary>
    public class DeleteService
    {
        private readonly List<Animal> animals;
        private readonly AnimalFileHandler fileHandler;

        /// <summary>The result of the last Delete, ready to show to the user.</summary>
        public string Message { get; private set; } = "";

        /// <summary>Works on the form's animal list and saves through the given file handler.</summary>
        public DeleteService(List<Animal> animals, AnimalFileHandler fileHandler)
        {
            this.animals = animals;
            this.fileHandler = fileHandler;
        }

        /// <summary>Finds an animal by ID (not case-sensitive). Returns null if there is no match.</summary>
        public Animal FindById(string id)
        {
            foreach (Animal animal in animals)
            {
                if (string.Equals(animal.AnimalId, id?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return animal;
                }
            }

            return null;
        }

        /// <summary>Builds the Yes/No question in the format the brief asks for.</summary>
        public string BuildConfirmMessage(Animal animal)
        {
            return $"Delete {animal.AnimalId} ({animal.Name})? This cannot be undone.";
        }

        /// <summary>
        /// Removes the animal from the list and rewrites animals.txt.
        /// If the file cannot be saved, the animal is put back so nothing is lost.
        /// </summary>
        public bool Delete(Animal animal)
        {
            int position = animals.IndexOf(animal);

            if (position < 0)
            {
                Message = "Animal not found.";
                return false;
            }

            animals.RemoveAt(position);

            if (!fileHandler.SaveAll(animals))
            {
                // Put the animal back in the same place so the list still matches the file.
                animals.Insert(position, animal);
                Message = "Animal was not deleted. " + fileHandler.LastError;
                return false;
            }

            Message = $"{animal.AnimalId} ({animal.Name}) was deleted.";
            return true;
        }
    }
}
