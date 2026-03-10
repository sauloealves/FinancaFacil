public class AiIntentResult {
    public string Intent { get; set; } = null!;

    public string? Category { get; set; }

    public Guid? AccountId { get; set; }

    public DateTime? Date { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }
}