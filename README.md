# HashPDF

## Παράδειγμα Χρήσης

- Χρησιμοποίησε το `HashPDF` για την εξαγωγή `Hash Code` από `DXF` και τη δημιουργία `PDF` που θα χρησιμοποιηθεί στην ψηφιακή υπογραφή τοπογραφικών διαγραμμάτων τα οποία υποβάλλονται ηλεκτρονικά για `ΚΗΔ`
- Σύρε το `DXF` μέσα στην εφαρμογή ή επίλεξέ το από το κουμπί επιλογής αρχείου
- Η εφαρμογή υπολογίζει το `SHA-512` hash, δημιουργεί `PDF` στον ίδιο φάκελο και εμφανίζει το hash στην οθόνη
- Στη συνέχεια μπορείς να ανοίξεις είτε το παραγόμενο `PDF` είτε τον φάκελο που το περιέχει
- Σχετικό άρθρο χρήσης: [Topometrics - Αποδεικτικό Υποβολής Ηλεκτρονικού Διαγράμματος](https://topometrics.gr/2020/12/05/%CE%B1%CF%80%CE%BF%CE%B4%CE%B5%CE%B9%CE%BA%CF%84%CE%B9%CE%BA%CE%BF-%CF%85%CF%80%CE%BF%CE%B2%CE%BF%CE%BB%CE%B7%CF%83-%CE%B7%CE%BB%CE%B5%CE%BA%CF%84%CF%81%CE%BF%CE%BD%CE%B9%CE%BA%CE%BF%CF%85-%CE%B4/)

Λήψη installer:

- [Κατέβασε το latest release installer](https://github.com/rtsketo/HashPDF/releases/latest)

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

Τρέχον workflow:

- Ο χρήστης ρίχνει ένα αρχείο στην περιοχή drag and drop ή το επιλέγει από dialog
- Η εφαρμογή υπολογίζει `SHA-512` hash σε background worker
- Παράγεται `PDF` με όνομα `<όνομα-αρχείου>.hash.pdf` στον ίδιο φάκελο
- Το UI εμφανίζει το hash και δίνει κουμπιά για άνοιγμα του PDF ή του φακέλου του

Build σημείωση:

- Το project στοχεύει `.NET Framework 4.0`
- Για build σε Windows χρειάζονται τα αντίστοιχα reference assemblies ή το `Developer Pack`
- Η διανομή μπορεί να γίνει με installer που θα εγκαθιστά αυτόματα το `.NET Framework 4.0`, ώστε ο τελικός χρήστης να μη χρειάζεται να ψάξει prerequisites
- Στο τρέχον macOS περιβάλλον δεν υπάρχει το `.NET Framework 4.0 Targeting Pack`, άρα δεν έγινε τοπικό compile validation

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

Current workflow:

- The user drops a file on the drag and drop surface or chooses it from a file dialog
- The application calculates a `SHA-512` hash on a background worker
- A `PDF` named `<file-name>.hash.pdf` is written next to the source file
- The UI displays the hash and provides buttons to open the PDF or its folder

Build note:

- The project targets `.NET Framework 4.0`
- Building on Windows requires the matching reference assemblies or `Developer Pack`
- Distribution can use an installer that bootstraps `.NET Framework 4.0` automatically, so the end user does not need to search for prerequisites
- The current macOS environment does not include the `.NET Framework 4.0 Targeting Pack`, so local compile validation was not possible here
