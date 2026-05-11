namespace SolarPhobia.Application.Messages.Payloads
{
    public class SelectionValidationResult
    {
        public bool IsValid { get; set; }
        public int SavedCount { get; set; }
        public int AbandonedCount { get; set; }
        public int TotalSouls { get; set; }
        public string ErrorMessage { get; set; }
    }
}
