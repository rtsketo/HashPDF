# HashPDF

## Ελληνικά

Το `HashPDF` είναι μια εφαρμογή Windows που δημιουργεί `SHA-512` hash από οποιοδήποτε αρχείο και παράγει ένα συνοδευτικό `PDF` στον ίδιο φάκελο.

Στόχοι της πρώτης έκδοσης:

- Συμβατότητα με `Windows 7` χωρίς απαίτηση για `SP1`, με στόχευση στο `.NET Framework 4.0`
- Drag and drop εμπειρία για επιλογή αρχείου
- Εμφάνιση του παραγόμενου hash μέσα στο UI
- Δημιουργία PDF στον ίδιο φάκελο με το αρχικό αρχείο
- Πολυγλωσσικό UI με έμφαση σε Ελληνικά και Αγγλικά

Σημειώσεις:

- Το project έχει στηθεί ως `Windows Forms` εφαρμογή σε `C#`
- Η παραγωγή PDF γίνεται χωρίς εξωτερικές βιβλιοθήκες, ώστε η διανομή να μείνει όσο γίνεται απλή
- Η τρέχουσα φάση επικεντρώνεται στο βασικό workflow και στο δίγλωσσο interface

## English

`HashPDF` is a Windows desktop application that creates a `SHA-512` hash from any file and generates a companion `PDF` in the same directory.

Goals for the first version:

- Compatibility with `Windows 7` without requiring `SP1`, by targeting `.NET Framework 4.0`
- Drag and drop file selection
- Displaying the generated hash inside the UI
- Writing the PDF next to the original file
- Polyglot UI with a first focus on Greek and English

Notes:

- The project is set up as a `C# Windows Forms` application
- PDF generation is implemented without external dependencies to keep distribution simple
- The current phase focuses on the core workflow and bilingual interface
