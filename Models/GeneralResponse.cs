namespace Midyaf.Models;
public class GeneralResponse
{
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public List<String>? Errors { get; set; }
    public object Data { get; set; }  
    public void SetResponse(string Message,bool  isSuccess,object Data = null!, List<string>? Errors = null!)
    {
        this.Message = Message;
        this.IsSuccess = isSuccess;
        this.Errors = Errors;
        this.Data = Data;
    }

    public override string ToString()
    {
        return base.ToString()+this.Message;
    }
}