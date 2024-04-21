namespace API.RequestModels.Periods;

public class UpdatePeriodRequest
{
    public DateTime FirstSubmissionDeadline { get; set; }
    public DateTime SecondSubmissionDeadline { get; set; }
}