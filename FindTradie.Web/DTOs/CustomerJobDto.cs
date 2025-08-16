namespace FindTradie.Web.DTOs;

public class CustomerJobDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PostedDate { get; set; } = string.Empty;
    public string Budget { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int QuotesCount { get; set; }
    public int UnreadQuotesCount { get; set; }
    public int TradieId { get; set; }
    public bool HasReview { get; set; }
}
