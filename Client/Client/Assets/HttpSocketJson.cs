using UnityEngine;

using System;
using System.Net;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class HttpSocketJson : HttpSocketBase
{
    public static void RequestPost(string url, Dictionary<string, string> requestParameters, Action<string> callback)
    {
        HttpSocketJson requestJson = new HttpSocketJson();

        requestJson.CreateHttpWebRequest(url, "POST", requestParameters);

        GetResonse(requestJson.Request, callback);
    }

    public static void RequestGet(string url, Dictionary<string, string> requestParameters, Action<string> callback)
    {
        HttpSocketJson requestJson = new HttpSocketJson();

        requestJson.CreateHttpWebRequest(url, "GET", requestParameters);

        GetResonse(requestJson.Request, callback);
    }

    protected override void CreateRequestBytes(Dictionary<string, string> requestParameters)
    {
        if (requestParameters == null || requestParameters.Count == 0)
            return;

        StringBuilder sb = new StringBuilder();

        sb.Append("{");

        foreach(string key in requestParameters.Keys)
        {
            sb.Append("\"");
            sb.Append(key);
            sb.Append("\"");

            sb.Append(":");

            sb.Append("\"");
            sb.Append(requestParameters[key]);
            sb.Append("\"");

            sb.Append(",");
        }

        if (sb.Length > 0)
            sb.Length = sb.Length - 1;

        sb.Append("}");

        Encoding encoding = Encoding.UTF8;
        byte[] result = Encoding.UTF8.GetBytes(sb.ToString());

        request.ContentType = "application/json;charset=utf-8";
        request.ContentLength = result.Length;

        using (Stream postDataStream = request.GetRequestStream())
        {
            postDataStream.Write(result, 0, result.Length);
            postDataStream.Close();
        }
    }
}
