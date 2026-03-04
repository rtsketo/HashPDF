using System.Collections.Generic;

namespace HashPDF.WinForms.Localization
{
    public static class TextCatalog
    {
        private static readonly IDictionary<string, string> Greek = new Dictionary<string, string>
        {
            { "HeaderTitle", "HashPDF" },
            { "HeaderSubtitle", "Δημιούργησε SHA-512 hash από οποιοδήποτε αρχείο και παρήγαγε PDF στον ίδιο φάκελο." },
            { "LanguageLabel", "Γλώσσα" },
            { "LanguageGreek", "Ελληνικά" },
            { "LanguageEnglish", "Αγγλικά" },
            { "DropTitle", "Σύρε ένα αρχείο εδώ" },
            { "DropHint", "Άφησε ένα μόνο αρχείο για να δημιουργηθεί το hash και το συνοδευτικό PDF." },
            { "BrowseButton", "Επιλογή αρχείου" },
            { "ResultTitle", "Αποτέλεσμα hash" },
            { "HashPlaceholder", "Το SHA-512 hash θα εμφανιστεί εδώ μετά την επεξεργασία." },
            { "BusyHashPlaceholder", "Γίνεται επεξεργασία του αρχείου και παραγωγή του PDF..." },
            { "FilePlaceholder", "Αρχείο: Δεν έχει επιλεγεί ακόμη." },
            { "PdfPlaceholder", "PDF: Δεν έχει παραχθεί ακόμη." },
            { "FileLabel", "Αρχείο:" },
            { "PdfLabel", "PDF:" },
            { "UnavailableValue", "Μη διαθέσιμο" },
            { "BusyPdfValue", "Παράγεται..." },
            { "OpenFolderButton", "Άνοιγμα φακέλου" },
            { "OpenPdfButton", "Άνοιγμα PDF" },
            { "IdleStatus", "Έτοιμο για νέο αρχείο." },
            { "BusyStatus", "Υπολογίζεται το hash και δημιουργείται το PDF..." },
            { "ReadyStatus", "Το hash και το PDF δημιουργήθηκαν επιτυχώς." },
            { "FailedStatus", "Η δημιουργία του hash ή του PDF απέτυχε." },
            { "ErrorTitle", "Σφάλμα" },
            { "BusyError", "Η εφαρμογή επεξεργάζεται ήδη άλλο αρχείο." },
            { "MissingFileError", "Το επιλεγμένο αρχείο δεν βρέθηκε." },
            { "MissingDirectoryError", "Δεν ήταν δυνατός ο εντοπισμός του φακέλου του αρχικού αρχείου." },
            { "WritePdfError", "Δεν ήταν δυνατή η δημιουργία του PDF στον ίδιο φάκελο με το αρχικό αρχείο." },
            { "GenericProcessingError", "Παρουσιάστηκε μη αναμενόμενο σφάλμα κατά την επεξεργασία του αρχείου." }
        };

        private static readonly IDictionary<string, string> English = new Dictionary<string, string>
        {
            { "HeaderTitle", "HashPDF" },
            { "HeaderSubtitle", "Create a SHA-512 hash from any file and export a PDF to the same folder." },
            { "LanguageLabel", "Language" },
            { "LanguageGreek", "Greek" },
            { "LanguageEnglish", "English" },
            { "DropTitle", "Drop a file here" },
            { "DropHint", "Drop a single file to create its hash and the companion PDF." },
            { "BrowseButton", "Choose file" },
            { "ResultTitle", "Hash result" },
            { "HashPlaceholder", "The SHA-512 hash will appear here after processing." },
            { "BusyHashPlaceholder", "The file is being processed and the PDF is being created..." },
            { "FilePlaceholder", "File: Nothing selected yet." },
            { "PdfPlaceholder", "PDF: Nothing generated yet." },
            { "FileLabel", "File:" },
            { "PdfLabel", "PDF:" },
            { "UnavailableValue", "Unavailable" },
            { "BusyPdfValue", "Generating..." },
            { "OpenFolderButton", "Open folder" },
            { "OpenPdfButton", "Open PDF" },
            { "IdleStatus", "Ready for a new file." },
            { "BusyStatus", "Calculating the hash and creating the PDF..." },
            { "ReadyStatus", "The hash and PDF were created successfully." },
            { "FailedStatus", "Hash or PDF generation failed." },
            { "ErrorTitle", "Error" },
            { "BusyError", "The application is already processing another file." },
            { "MissingFileError", "The selected file could not be found." },
            { "MissingDirectoryError", "The source file directory could not be resolved." },
            { "WritePdfError", "The PDF could not be created in the same directory as the source file." },
            { "GenericProcessingError", "An unexpected error occurred while processing the file." }
        };

        public static string Get(AppLanguage language, string key)
        {
            IDictionary<string, string> source = language == AppLanguage.Greek ? Greek : English;
            if (source.ContainsKey(key))
            {
                return source[key];
            }

            return key;
        }
    }
}
