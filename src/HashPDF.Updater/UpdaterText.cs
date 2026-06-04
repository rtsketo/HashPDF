using System;

namespace HashPDF.Updater
{
    internal sealed class UpdaterText
    {
        private readonly bool english;

        public UpdaterText(string language)
        {
            english = language != null && language.Equals("english", StringComparison.OrdinalIgnoreCase);
        }

        public string WindowTitle
        {
            get { return english ? "HashPDF Update" : "Ενημέρωση HashPDF"; }
        }

        public string Starting
        {
            get { return english ? "Preparing the update..." : "Προετοιμασία ενημέρωσης..."; }
        }

        public string Elevating
        {
            get { return english ? "Administrator permission is required." : "Απαιτούνται δικαιώματα διαχειριστή."; }
        }

        public string Waiting
        {
            get { return english ? "Waiting for HashPDF to close..." : "Αναμονή για κλείσιμο του HashPDF..."; }
        }

        public string Downloading
        {
            get { return english ? "Downloading update files..." : "Λήψη αρχείων ενημέρωσης..."; }
        }

        public string Verifying
        {
            get { return english ? "Verifying downloaded files..." : "Έλεγχος αρχείων ενημέρωσης..."; }
        }

        public string Installing
        {
            get { return english ? "Installing update..." : "Εγκατάσταση ενημέρωσης..."; }
        }

        public string RollingBack
        {
            get { return english ? "Restoring previous files..." : "Επαναφορά προηγούμενων αρχείων..."; }
        }

        public string Complete
        {
            get { return english ? "Update completed." : "Η ενημέρωση ολοκληρώθηκε."; }
        }

        public string Failed
        {
            get { return english ? "Update failed." : "Η ενημέρωση απέτυχε."; }
        }

        public string Close
        {
            get { return english ? "Close" : "Κλείσιμο"; }
        }

        public string Restarting
        {
            get { return english ? "Restarting HashPDF..." : "Επανεκκίνηση HashPDF..."; }
        }
    }
}
