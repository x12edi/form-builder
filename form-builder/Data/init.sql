CREATE TABLE IF NOT EXISTS Forms (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Metadata TEXT NOT NULL,
    CreatedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Submissions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FormId INTEGER NOT NULL,
    SubmissionData TEXT NOT NULL,
    SubmittedAt TEXT NOT NULL,
    FOREIGN KEY (FormId) REFERENCES Forms(Id)
);

--INSERT INTO Forms (Name, Metadata, CreatedAt) VALUES (
--  'Contact Us',
--  '{
--    "formName": "Contact Us",
--    "fields": [
--      { "id": "fullName", "label": "Full Name", "type": "text", "required": true },
--      { "id": "email", "label": "Email Address", "type": "email", "required": true },
--      { "id": "message", "label": "Message", "type": "textarea", "required": false },
--      { "id": "subscribe", "label": "Subscribe to newsletter", "type": "checkbox", "required": false }
--    ]
--  }',
--  datetime("now")
--);