using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;


//동기, 비동기 방식 구분
//POST, GET Method 사용 가능

public abstract class HttpSocketBase
{
    protected HttpWebRequest request = null;

    public HttpWebRequest Request
    {
        get { return request; }
    }

    protected static void GetResonse(HttpWebRequest request, Action<string> callback)
    {
        HttpWebResponse httpResonse = request.GetResponse() as HttpWebResponse;
        if (httpResonse == null)
        {
            Debug.LogError("cannot cast HttpWebResponse");

            return;
        }

        using (Stream respPostStream = httpResonse.GetResponseStream())
        {
            StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

            callback(readerPost.ReadToEnd());
        }
    }

    protected void CreateHttpWebRequest(string url, string httpMethod, Dictionary<string, string> requestParameters)
    {
        request = WebRequest.Create(url) as HttpWebRequest;

        if (request == null)
            return;

        request.ServicePoint.Expect100Continue = false;
        request.Method = httpMethod;

        CreateRequestBytes(requestParameters);
    }

    protected abstract void CreateRequestBytes(Dictionary<string, string> requestParameters);
}
