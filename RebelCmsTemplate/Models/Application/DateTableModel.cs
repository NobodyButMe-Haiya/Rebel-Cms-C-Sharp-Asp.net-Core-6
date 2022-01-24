namespace RebelCmsTemplate.Models.Application;

public class DateTableModel
{
    public int DateTableKey { get; init; }
    public string? DateTableString { get; init; }
    public DateTime DateTableDateTime { get; init; }
    public DateOnly DateTableDate { get; init; }
    public TimeOnly DateTableTime { get; init; }
    public int IsDelete { get; init; }
    public string? ExecuteBy { get; init; }
}