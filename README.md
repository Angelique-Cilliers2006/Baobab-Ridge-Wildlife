# Baobab Ridge Wildlife Rehabilitation Records System

A Windows desktop application for managing the animals in the care of the Baobab Ridge Wildlife Rehabilitation Centre. Staff can add, view, search, update and delete animal records, and generate a summary report. All data is stored in a plain text file, so no database is needed.

Built for the **PRG2782 group project (2026)** with C# Windows Forms and .NET 8.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [How to Use the App](#how-to-use-the-app)
- [Data File Format](#data-file-format)
- [Recovery Score Classification](#recovery-score-classification)
- [Validation Rules](#validation-rules)
- [Project Structure](#project-structure)
- [Limitations](#limitations)
- [Team](#team)

---

## Features

| # | Feature | Description |
|---|---------|-------------|
| 1 | **Add** | Add a new animal. Input is validated and the record is appended to `animals.txt`. |
| 2 | **View** | All animals load into a grid when the app starts. Invalid lines in the file are skipped and reported. |
| 3 | **Search and Update** | Search by Animal ID (or click a row), edit the details and save the changes. |
| 4 | **Delete** | Delete the selected animal after a Yes/No confirmation. The file is rewritten. |
| 5 | **Summary Report** | Shows totals, averages and a count per status on the form and saves it to `summary.txt`. |

Other highlights:

- **Status and Housing Unit are never typed in.** They are calculated automatically from the Recovery Score.
- **Friendly error handling.** File problems (read-only, locked, missing) show a clear message instead of crashing.
- **No hard-coded paths.** `animals.txt` and `summary.txt` live beside the `.exe`, so the app works on any computer.
- **Empty data is handled.** The summary shows `N/A` for averages when there are no animals.

---

## Tech Stack

- **Language:** C#
- **Framework:** .NET 8 (`net8.0-windows`)
- **UI:** Windows Forms
- **Storage:** Plain text files (UTF-8, pipe-delimited)
- **Version control:** Git and GitHub

---

## Getting Started

### Prerequisites

- Windows 10 or 11
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.8 or newer) with the **.NET desktop development** workload
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (included with the workload above)
- [Git](https://git-scm.com/downloads)

### Run it

1. **Clone the repository**
   ```bash
   git clone https://github.com/Angelique-Cilliers2006/Baobab-Ridge-Wildlife.git
   cd Baobab-Ridge-Wildlife
   ```
2. **Open the solution** by double-clicking `Baobab Ridge Wildlife.sln` (or open it from Visual Studio: **File > Open > Project/Solution**).
3. **Run the app** by pressing **F5** (or click the green **Start** button).

You can also run it from the command line:

```bash
dotnet run --project "Baobab Ridge Wildlife"
```

> `animals.txt` is copied next to the `.exe` on every build (the project is set to "Copy if newer"). The app reads and writes the copy in the build folder, for example `bin/Debug/net8.0-windows/animals.txt`.

---

## How to Use the App

**Add an animal**
1. Fill in Animal ID, Name, Species, Age and Recovery Score.
2. Click **Add**. A message shows the calculated Status and Housing Unit.

**Search and update an animal**
1. Type an ID in **Search by Animal ID** and click **Search**, or click a row in the grid.
2. The details load into the form. The ID is locked because it cannot change.
3. Edit the fields and click **Save Changes**. Click **Clear** to leave edit mode.

**Delete an animal**
1. Select a row in the grid and click **Delete**.
2. Confirm with **Yes**. The default button is **No**, so pressing Enter by accident deletes nothing.

**Generate a summary**
1. Click **Generate Summary**.
2. The report appears in the text box and is saved to `summary.txt` beside the `.exe`.

---

## Data File Format

Each animal is one line in `animals.txt`, with seven fields separated by a pipe (`|`):

```
AnimalID|Name|Species|Age|RecoveryScore|Status|HousingUnit
```

Example:

```
WR-0001|Thabo|African Penguin|4|85|Release-Ready|Pre-Release Camp
WR-0004|Zola|Cape Vulture|12|19|Critical|Intensive Care Unit
```

Status and Housing Unit in the file are **not trusted**. They are recalculated from the Recovery Score whenever a line is loaded. Lines that do not match the format are skipped and the user is told how many were skipped.

### Example summary report (`summary.txt`)

```
BAOBAB RIDGE WILDLIFE REHABILITATION CENTRE
ANIMAL SUMMARY REPORT
Generated: 2026-10-20 14:30
----------------------------------------
Total animals:            12
Average age (years):      7.75
Average recovery score:   54.00
----------------------------------------
Animals per status:
  Critical:               2
  Serious:                2
  Stable:                 2
  Recovering:             3
  Release-Ready:          3
```

---

## Recovery Score Classification

Every animal has a Recovery Score from **0 to 100**. The Status and Housing Unit come from this table:

| Recovery Score | Status | Housing Unit |
|:--------------:|--------|--------------|
| 0 - 19 | Critical | Intensive Care Unit |
| 20 - 39 | Serious | High-Dependency Ward |
| 40 - 59 | Stable | Recovery Ward |
| 60 - 79 | Recovering | Outdoor Enclosure |
| 80 - 100 | Release-Ready | Pre-Release Camp |

The rules live in one place, `Animal.Classify()`, so Add and Update always agree.

---

## Validation Rules

| Field | Rule |
|-------|------|
| **Animal ID** | Required. Format `WR-` followed by exactly 4 digits (e.g. `WR-0007`). Must be unique. Lower case is converted to upper case. |
| **Name** | Required. 1 to 30 characters. Must not contain `\|`. |
| **Species** | Required. 1 to 30 characters. Must not contain `\|`. |
| **Age** | Required. Whole number from 0 to 100. |
| **Recovery Score** | Required. Whole number from 0 to 100. |

Nothing is saved if any rule fails. The cursor jumps to the first field with a problem.

---

## Project Structure

```
Baobab-Ridge-Wildlife/
├── Baobab Ridge Wildlife.sln
├── README.md
├── .gitignore
├── .gitattributes
└── Baobab Ridge Wildlife/
    ├── Program.cs              # App entry point, opens Form1
    ├── Form1.cs                # Layout and Feature 1 (Add)
    ├── Form1.Designer.cs       # Designer-generated code
    ├── Form1.View.cs           # Feature 2 (load and show animals)
    ├── Form1.Update.cs         # Feature 3 (search and update)
    ├── Form1.PersonD.cs        # Features 4 and 5 (delete and summary)
    ├── Animal.cs               # Animal model, classification, parse/format
    ├── AnimalValidator.cs      # Input validation rules
    ├── AnimalFileHandler.cs    # Reads and rewrites animals.txt
    ├── DeleteService.cs        # Finds and deletes an animal safely
    ├── SummaryReport.cs        # Calculates and formats the summary
    ├── ReportWriter.cs         # Saves summary.txt
    └── animals.txt             # Sample data (12 animals)
```

`Form1` is split into several `partial` files so each team member can work on their own feature without editing the same lines.

---

## Limitations

- **Windows only.** The app uses Windows Forms (`net8.0-windows`), so it will not run on macOS or Linux.
- **Text file storage.** Data lives in `animals.txt`, not a database. There is no searching, sorting or indexing, and performance will drop with very large files.
- **Single user.** There is no locking, so two people editing the same file at once can overwrite each other's changes. There are also no logins or user roles.
- **Data is stored beside the `.exe`.** While developing, the running app uses the copy in `bin/Debug/net8.0-windows/`, not the `animals.txt` in the source folder. Changes made while running are not written back to the source file.
- **Delete cannot be undone.** There is no recycle bin, and no automatic backup of `animals.txt`.
- **Malformed lines are lost on rewrite.** Invalid lines are skipped when loading. Any action that rewrites the whole file (such as Delete) saves only the valid records, so skipped lines are permanently removed.
- **Search is by Animal ID only.** There is no search by name or species, and no filter or sort controls in the grid.
- **Fixed field rules.** Animal IDs must follow `WR-0000`. Name and Species are limited to 30 characters and cannot contain `|`. Age is limited to 0-100 and Recovery Score to 0-100, whole numbers only.
- **Fixed classification bands.** Status and Housing Unit are calculated from hard-coded score ranges and cannot be changed without editing the code.
- **Summary report is overwritten.** Each time **Generate Summary** is clicked, `summary.txt` is replaced. Old reports are not kept.
- **Layout is code-built and fixed.** The form is built in code rather than the Visual Studio designer, with a minimum window size, and there is no multi-language support.
- **No automated tests.** The app has been tested manually only.

---

## Team

| Team member | Responsibility |
|-------------|----------------|
| **Angelique** | Form layout and Feature 1 (Add) |
| **Ditheto** | Feature 2 (View all animals, file loading, grid) |
| **Marco** | Feature 3 (Search and Update) |
| **Kudzai** | Feature 4 (Delete), Feature 5 (Summary report) and final fresh-clone test |

---

## Course Information

PRG2782, 2026. Submission deadline: 20 October 2026, 23:59 (BCConnect).
