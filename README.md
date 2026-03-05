# HashPDF

## Λήψη Αρχείου

- [Κατέβασε το latest release installer](https://github.com/rtsketo/HashPDF/releases/latest)

## Παράδειγμα Χρήσης

- Χρησιμοποίησε το `HashPDF` για την εξαγωγή `Hash Code` από `DXF` και τη δημιουργία `PDF` που θα χρησιμοποιηθεί στην ψηφιακή υπογραφή τοπογραφικών διαγραμμάτων τα οποία υποβάλλονται ηλεκτρονικά για `ΚΗΔ`
- Σύρε το `DXF` μέσα στην εφαρμογή ή επίλεξέ το από το κουμπί επιλογής αρχείου
- Η εφαρμογή υπολογίζει το `SHA-512` hash, δημιουργεί `PDF` στον ίδιο φάκελο και εμφανίζει το hash στην οθόνη
- Στη συνέχεια μπορείς να ανοίξεις είτε το παραγόμενο `PDF` είτε τον φάκελο που το περιέχει
- Σχετικό άρθρο χρήσης: [Topometrics - Αποδεικτικό Υποβολής Ηλεκτρονικού Διαγράμματος](https://topometrics.gr/2020/12/05/%CE%B1%CF%80%CE%BF%CE%B4%CE%B5%CE%B9%CE%BA%CF%84%CE%B9%CE%BA%CE%BF-%CF%85%CF%80%CE%BF%CE%B2%CE%BF%CE%BB%CE%B7%CF%83-%CE%B7%CE%BB%CE%B5%CE%BA%CF%84%CF%81%CE%BF%CE%BD%CE%B9%CE%BA%CE%BF%CF%85-%CE%B4/)

## Περιγραφή

Το `HashPDF` είναι μια εφαρμογή Windows που δημιουργεί `SHA-512` hash από οποιοδήποτε αρχείο και παράγει ένα συνοδευτικό `PDF` στον ίδιο φάκελο.
Υποστηρίζει `Windows 7+` χωρίς απαίτηση για `SP1`.

Build σημείωση:

- Το project στοχεύει `.NET Framework 4.0`
- Για local build σε Windows χρειάζονται τα αντίστοιχα reference assemblies ή το `Developer Pack`
- Ενδεικτική εντολή build: `dotnet build HashPDF.sln -c Release`
- Ο installer των release εκδόσεων κάνει bootstrap το `.NET Framework 4.0` όταν λείπει

## Description

`HashPDF` is a Windows desktop application that creates a `SHA-512` hash from any file and generates a companion `PDF` in the same directory.
It supports `Windows 7+` without requiring `SP1`.

Build note:

- The project targets `.NET Framework 4.0`
- Local Windows builds require the matching reference assemblies or `Developer Pack`
- Example build command: `dotnet build HashPDF.sln -c Release`
- The release installer bootstraps `.NET Framework 4.0` automatically when missing
