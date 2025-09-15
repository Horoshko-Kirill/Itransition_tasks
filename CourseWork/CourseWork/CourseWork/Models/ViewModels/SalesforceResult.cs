namespace CourseWork.Models.ViewModels
{
    public class SalesforceResult
    {
        public bool Success { get; set; }
        public string? AccountId { get; set; }
        public string? ContactId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
