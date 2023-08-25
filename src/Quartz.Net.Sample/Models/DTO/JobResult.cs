namespace Quartz.Net.Sample.Models.DTO;

public class JobResult
{
    /// <summary>
    /// Is successful
    /// </summary>
    public bool IsSuccess { get; set; } = false;

    /// <summary>
    /// Executed result
    /// </summary>
    public object ExecutedResult { get; set; }
}
