using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

public static class HttpSocket
{
    public static void Init()
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
        delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                System.Security.Cryptography.X509Certificates.X509Chain chain,
                                System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true; // **** Always accept
        };
    }

    static HttpWebRequest CreateHttpWebRequest(string url, string httpMethod, string contentType)
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.ServicePoint.Expect100Continue = false;
        httpWebRequest.ContentType = contentType;
        httpWebRequest.Method = httpMethod;
        return httpWebRequest;
    }

    static byte[] GetRequestBytes(Dictionary<string, string> postParameters)
    {
        if (postParameters == null || postParameters.Count == 0)
            return new byte[0];

        var sb = new StringBuilder();

        //foreach (var key in postParameters)
        //{
        //    sb.Append(Uri.EscapeDataString(key.Key) + "=" + Uri.EscapeDataString(key.Value) + "&");
        //}

        //sb.Length = sb.Length - 1;

        sb.Append("{");

        foreach (var key in postParameters)
        {
            sb.Append(  "\"" + Uri.EscapeDataString(key.Key) + "\"" + ":" + "\"" + Uri.EscapeDataString(key.Value) + "\"" + ",");
        }

        if (sb.Length > 0)
            sb.Length = sb.Length - 1;

        sb.Append("}");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    static void BeginGetRequestStreamCallback(IAsyncResult asyncResult)
    {
        HttpWebRequestAsyncState asyncState = null;
        try
        {
            asyncState = (HttpWebRequestAsyncState)asyncResult.AsyncState;
            using (var requestStream = asyncState.HttpWebRequest.EndGetRequestStream(asyncResult))
            {
                requestStream.Write(asyncState.RequestBytes, 0, asyncState.RequestBytes.Length);

                asyncState.HttpWebRequest.BeginGetResponse(BeginGetResponseCallback,
                  new HttpWebRequestAsyncState
                  {
                      HttpWebRequest = asyncState.HttpWebRequest,
                      ResponseCallback = asyncState.ResponseCallback,
                      State = asyncState.State
                  });

                requestStream.Close();
            }
        }
        catch (Exception ex)
        {
            if (asyncState != null)
                asyncState.ResponseCallback(new HttpWebRequestCallbackState(ex, asyncState.State));
            else
                throw;
        }
    }

    static void BeginGetResponseCallback(IAsyncResult asyncResult)
    {
        HttpWebRequestAsyncState asyncState = null;
        try
        {
            asyncState = (HttpWebRequestAsyncState)asyncResult.AsyncState;
            using (var webResponse = asyncState.HttpWebRequest.EndGetResponse(asyncResult))
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    var webRequestCallbackState = new HttpWebRequestCallbackState(responseStream, asyncState.State);
                    asyncState.ResponseCallback(webRequestCallbackState);

                    responseStream.Close();
                }

                webResponse.Close();
            }
        }
        catch (Exception ex)
        {
            if (asyncState != null)
                asyncState.ResponseCallback(new HttpWebRequestCallbackState(ex, asyncState.State));
            else
                throw;
        }
    }

    public static string GetResponseText(Stream responseStream)
    {
        using (var reader = new StreamReader(responseStream))
        {
            return reader.ReadToEnd();
        }
    }

    public static void PostAsync(string url, Dictionary<string, string> postParameters, Action<HttpWebRequestCallbackState> responseCallback, object state = null, Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded;charset=utf-8", int httpTimeout = 20000)
    {
        var httpWebRequest = CreateHttpWebRequest(url, "POST", contentType);
        var requestBytes = GetRequestBytes(postParameters);
        httpWebRequest.ContentLength = requestBytes.Length;
        httpWebRequest.Timeout = httpTimeout;

        if (headers != null)
        {
            foreach (KeyValuePair<string, string> header in headers)
                httpWebRequest.Headers.Add(header.Key, header.Value);
        }

        try
        {
            httpWebRequest.BeginGetRequestStream(BeginGetRequestStreamCallback,
              new HttpWebRequestAsyncState()
              {
                  RequestBytes = requestBytes,
                  HttpWebRequest = httpWebRequest,
                  ResponseCallback = responseCallback,
                  State = state
              });
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }

    public static void GetAsync(string url, Action<HttpWebRequestCallbackState> responseCallback, object state = null, string contentType = "application/x-www-form-urlencoded;charset=utf-8", int httpTimeout = 20000)
    {
        var httpWebRequest = CreateHttpWebRequest(url, "GET", contentType);
        httpWebRequest.Timeout = httpTimeout;

        httpWebRequest.BeginGetResponse(BeginGetResponseCallback,
          new HttpWebRequestAsyncState()
          {
              HttpWebRequest = httpWebRequest,
              ResponseCallback = responseCallback,
              State = state
          });
    }
}

class HttpWebRequestAsyncState
{
    public byte[] RequestBytes { get; set; }
    public HttpWebRequest HttpWebRequest { get; set; }
    public Action<HttpWebRequestCallbackState> ResponseCallback { get; set; }
    public object State { get; set; }
}

public class HttpWebRequestCallbackState
{
    public Stream ResponseStream { get; private set; }
    public Exception Exception { get; private set; }
    public object State { get; set; }

    public HttpWebRequestCallbackState(Stream responseStream, object state)
    {
        ResponseStream = responseStream;
        Exception = null;
        State = state;
    }

    public HttpWebRequestCallbackState(Exception exception, object state)
    {
        ResponseStream = null;
        Exception = exception;
        State = state;
    }
}