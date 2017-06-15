using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;

//동기, 비동기 방식 구분
//POST, GET Method 전송

public static class HttpSocket
{
    public interface IHttpRequestBody
    {
        byte[] CreateRequestBody(HttpWebRequest request, Dictionary<string, string> requestParameters);
    }

    public static void SendPOST(string url, IHttpRequestBody sendBodyClass, Dictionary<string, string> requestParameters, Action<string> responseCallback, bool async = false)
    {
        if(async)
            SendAsync("POST", url, sendBodyClass, requestParameters, responseCallback);
        else
            Send("POST", url, sendBodyClass, requestParameters, responseCallback);
    }

    public static void SendGET(string url, IHttpRequestBody sendBodyClass, Dictionary<string, string> requestParameters, Action<string> responseCallback, bool async = false)
    {
        if (async)
            SendAsync("GET", url, sendBodyClass, requestParameters, responseCallback);
        else
            Send("GET", url, sendBodyClass, requestParameters, responseCallback);
    }

    private static void Send(string httpMethod, string url, IHttpRequestBody sendBodyClass, Dictionary<string, string> requestParameters, Action<string> responseCallback)
    {
        HttpWebRequest request = CreateHttpWebRequest(url, httpMethod);

        if (request == null)
            return;

        Request(request, sendBodyClass.CreateRequestBody(request, requestParameters));
        Response(request, responseCallback);
    }

    private static void SendAsync(string httpMethod, string url, IHttpRequestBody sendBodyClass, Dictionary<string, string> requestParameters, Action<string> responseCallback)
    {
        HttpWebRequest request = CreateHttpWebRequest(url, httpMethod);

        if (request == null)
            return;

        RequestAysnc(request, sendBodyClass.CreateRequestBody(request, requestParameters), (HttpWebRequest resultRequest) =>
        {
            //비동기 요청 완료시 응답 받기
            ResponseAysnc(resultRequest, responseCallback);
        });
    }

    private static HttpWebRequest CreateHttpWebRequest(string url, string httpMethod)
    {
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

        if (request == null)
            return null;

        request.ServicePoint.Expect100Continue = false; //expect 100 헤더 전송 안함
        request.Method = httpMethod;

        return request;
    }

    private static void Request(HttpWebRequest request, byte[] body)
    {
        using (Stream requestStream = request.GetRequestStream())
        {
            requestStream.Write(body, 0, body.Length);
            requestStream.Close();
        }
    }

    private static void RequestAysnc(HttpWebRequest request, byte[] body, Action<HttpWebRequest> finishRequestCallback)
    {
        request.BeginGetRequestStream(new AsyncCallback((IAsyncResult asyncResult) =>
        {
            HttpWebRequest resultRequest = asyncResult.AsyncState as HttpWebRequest;

            using (Stream requestStream = resultRequest.EndGetRequestStream(asyncResult))
            {
                requestStream.Write(body, 0, body.Length);
                requestStream.Close();
            }

            finishRequestCallback(resultRequest);
        })
        , request);
    }

    private static void Response(HttpWebRequest request, Action<string> responseCallback)
    {
        HttpWebResponse response = request.GetResponse() as HttpWebResponse;

        Debug.Assert(response != null, "httpResonse is null");

        using (Stream responseStream = response.GetResponseStream())
        {
            using (StreamReader reader = new StreamReader(responseStream, Encoding.Default))
            {
                responseCallback(reader.ReadToEnd());
            }
        }
    }

    private static void ResponseAysnc(HttpWebRequest request, Action<string> responseCallback)
    {
        request.BeginGetResponse(new AsyncCallback((IAsyncResult asyncResult) =>
        {
            HttpWebRequest resultRequest = asyncResult.AsyncState as HttpWebRequest;

            HttpWebResponse response = resultRequest.EndGetResponse(asyncResult) as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream, Encoding.Default))
                {
                    responseCallback(reader.ReadToEnd());
                }
            }
        }),
        request);
    }
}
