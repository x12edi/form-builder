# Form Builder
This is a dynamic form builder built with **ASP.NET Core MVC** that allows users to visually create custom forms with various input fields (text, email, checkbox, textarea, select), save form metadata, and handle form submissions.
---

## Features
- Visual drag-and-drop form builder
- Dynamic form field creation (text, email, textarea, checkbox, select)
- JSON metadata generation and preview
- Save/update form metadata to the database
- Form submission handling and storage
- SQLite database for lightweight local storage
- jQuery and SortableJS integration for interactivity
---

## Tech Stack

- **Backend:** ASP.NET Core MVC
- **Frontend:** HTML, Bootstrap, jQuery, SortableJS
- **Database:** SQLite
---

## Schema

CREATE TABLE Forms (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    Metadata TEXT
);

CREATE TABLE Submissions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FormId INTEGER,
    SubmissionData TEXT,
    SubmittedAt TEXT
);


