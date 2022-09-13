using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMS_Lib.DI;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CMS_Lib.Services.HttpContext;

public interface IHttpContextService : IScoped
{
    Task<HttpResponseMessage> PostJsonAsync(HttpClient httpClient, string url, object body,
        Dictionary<string, string> headers,string mediaType = "application/json");

    Task<HttpResponseMessage> PutJsonAsync(HttpClient httpClient, string url, object body,
        Dictionary<string, string> headers, string mediaType = "application/json");

    Task<HttpResponseMessage> PostFormAsync(HttpClient httpClient, string url, FormUrlEncodedContent param,
        Dictionary<string, string> headers = null);

    Task<HttpResponseMessage> GetJsonAsync(HttpClient httpClient, string url, Dictionary<string, string> headers);

    Task<HttpResponseMessage> DeleteJsonAsync(HttpClient httpClient, string url, object body,
        Dictionary<string, string> headers, string mediaType = "application/json",
        CancellationToken cancellationToken = default);

}

public class HttpContextService : IHttpContextService
{
    public Task<HttpResponseMessage> PostJsonAsync(HttpClient httpClient, string url, object body,
        Dictionary<string, string> headers, string mediaType = "application/json")
    {
        var bodyJson = JsonSerializer.Serialize(body);
        var stringContent = new StringContent(bodyJson, Encoding.UTF8, mediaType);
        if (headers is { Count: > 0 })
        {
            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }   
        }
        return httpClient.PostAsync(url, stringContent);
    }
    
    public Task<HttpResponseMessage> PutJsonAsync(HttpClient httpClient, string url, object body,
        Dictionary<string, string> headers, string mediaType = "application/json")
    {
        var bodyJson = JsonSerializer.Serialize(body);
        var stringContent = new StringContent(bodyJson, Encoding.UTF8, mediaType);
        if (headers is { Count: > 0 })
        {
            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }   
        }
        return httpClient.PutAsync(url, stringContent);
    }

    public Task<HttpResponseMessage> GetJsonAsync(HttpClient httpClient, string url, Dictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        return httpClient.GetAsync(url);
    }

    public Task<HttpResponseMessage> PostFormAsync(HttpClient httpClient, string url, FormUrlEncodedContent param,Dictionary<string, string> headers = null)
    {
        if (headers != null)
        {
            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }   
        }
        return httpClient.PostAsync(url, param);
    }

    // public Task<HttpResponseMessage> DeleteJsonAsync(HttpClient httpClient, string url, object body,
    //     Dictionary<string, string> headers, string mediaType = "application/json")
    // {
    //     var bodyJson = JsonSerializer.Serialize(body);
    //     var stringContent = new StringContent(bodyJson, Encoding.UTF8, mediaType);
    //     if (headers is { Count: > 0 })
    //     {
    //         foreach (var header in headers)
    //         {
    //             httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
    //         }   
    //     }
    //     return httpClient.DeleteAsync(url,CancellationToken.None);
    // }
    
    public Task<HttpResponseMessage> DeleteJsonAsync(HttpClient httpClient, string url, object body,Dictionary<string, string> headers,string mediaType = "application/json",
        CancellationToken cancellationToken = default)
    {
        var bodyJson = JsonSerializer.Serialize(body);
        var stringContent = new StringContent(bodyJson, Encoding.UTF8, mediaType);
        if (headers is { Count: > 0 })
        {
            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }   
        }
        var request = new HttpRequestMessage {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(url),
            Content = stringContent
        };
        return httpClient.SendAsync(request, cancellationToken);
        
        // using var request = new HttpRequestMessage(HttpMethod.Delete, url)
        // {
        //     Content = stringContent,
        // };
        // return httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, new CancellationToken(false));
    }
}