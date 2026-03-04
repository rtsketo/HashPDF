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
            { "FilePlaceholder", "Αρχείο: Δεν έχει επιλεγεί ακόμη." },
            { "PdfPlaceholder", "PDF: Δεν έχει παραχθεί ακόμη." },
            { "OpenFolderButton", "Άνοιγμα φακέλου" },
            { "OpenPdfButton", "Άνοιγμα PDF" },
            { "IdleStatus", "Έτοιμο για νέο αρχείο." },
            { "SelectionReadyStatus", "Το αρχείο επιλέχθηκε. Η επεξεργασία θα συνδεθεί στο επόμενο βήμα." }
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
            { "FilePlaceholder", "File: Nothing selected yet." },
            { "PdfPlaceholder", "PDF: Nothing generated yet." },
            { "OpenFolderButton", "Open folder" },
            { "OpenPdfButton", "Open PDF" },
            { "IdleStatus", "Ready for a new file." },
            { "SelectionReadyStatus", "The file was selected. Processing will be connected in the next step." }
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
