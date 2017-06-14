using UnityEngine;
using System.Collections;

using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;

public abstract class HttpSocketForm : HttpSocketBase
{
    protected override void CreateRequestBytes(Dictionary<string, string> requestParameters)
    {
        if (requestParameters == null || requestParameters.Count == 0)
            return;

        StringBuilder sb = new StringBuilder();

        Encoding encoding = Encoding.UTF8;
        byte[] result = Encoding.UTF8.GetBytes(sb.ToString());

        request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
        request.ContentLength = result.Length;

        using (Stream postDataStream = request.GetRequestStream())
        {
            postDataStream.Write(result, 0, result.Length);
            postDataStream.Close();
        }
    }
}
