using CMS_App_Api.Helpers.Consts;
using System;
using System.Text.Json.Serialization;

namespace CMS_App_Api.Models
{
    public class OutputObject
    {
        private int StatusCode { get; set; }
        private string Message { get; set; }
        private dynamic Data { get; set; }
        private string Err { get; set; }

        public OutputObject(MessageCode code, dynamic data, string message = null, string err = "")
        {
            this.StatusCode = code.Code;
            if (message == null)
            {
                this.Message = code.Value;
            }
            else
            {
                this.Message = message;
            }
            this.Data = data;
            this.Err = err;
        }

        public ResultJson Show()
        {
            ResultJson rs = new ResultJson();
            rs.Data = this.Data;
            rs.Message = this.Message;
            rs.Err = this.Err;
            rs.StatusCode = this.StatusCode;
            return rs;
        }
    }

    public class ResultJson
    {

        public ResultJson()
        {
            Time = DateTime.Now;
        }

        [JsonPropertyName("code")]
        public int StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("err")]
        public string Err { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("data")]
        [Newtonsoft.Json.JsonIgnore]
        public dynamic Data { get; set; }
    }
}
