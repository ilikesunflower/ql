using System;
using System.Text.Json.Serialization;

namespace CMS.Models;

public class OutputObject
{
    private int StatusCode { get; set; }
    private string? Message { get; set; }
    private dynamic? Data { get; set; }
    private string? Err { get; set; }

    public OutputObject(int code, dynamic? data, string? message = null, string err = "")
    {
        this.StatusCode = code;
        this.Message = message;
        this.Data = data;
        this.Err = err;
    }

    public ResultJson Show()
    {
        ResultJson rs = new ResultJson
        {
            Data = this.Data,
            Message = this.Message,
            Err = this.Err,
            StatusCode = this.StatusCode
        };
        return rs;
    }
}

public class ResultJson
{
    public ResultJson()
    {
        Time = DateTime.Now;
    }

    [JsonPropertyName("statusCode")] public int StatusCode { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("err")] public string? Err { get; set; }

    [JsonPropertyName("time")] public DateTime Time { get; }

    [JsonPropertyName("data")]
    [Newtonsoft.Json.JsonIgnore]
    public dynamic? Data { get; set; }
}