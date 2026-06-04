namespace HashPDF.Updater
{
    internal sealed class UpdateProgress
    {
        public UpdateProgress(string title, string detail, int percent, bool indeterminate)
        {
            Title = title;
            Detail = detail;
            Percent = percent;
            Indeterminate = indeterminate;
        }

        public string Title { get; private set; }

        public string Detail { get; private set; }

        public int Percent { get; private set; }

        public bool Indeterminate { get; private set; }
    }
}
