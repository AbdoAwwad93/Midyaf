namespace Midyaf;
public class GeneralResponse
{
    private string? Message { get; set; }
    private bool IsSuccess { get; set; }
    private List<String>? Errors { get; set; }
    public void SetResponse(string Message,bool  isSuccess,List<string>? Errors =  null!)
    {
        this.Message = Message;
        this.IsSuccess = isSuccess;
        this.Errors = Errors;
    }
}