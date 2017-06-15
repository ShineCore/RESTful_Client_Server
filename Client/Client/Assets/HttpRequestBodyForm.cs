using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class HttpRequestBodyForm : Singleton<HttpRequestBodyForm>, HttpSocket.IHttpRequestBody
{
    private HttpRequestBodyForm() { }

    public byte[] CreateRequestBody(HttpWebRequest request, Dictionary<string, string> requestParameters)
    {
        if (requestParameters == null || requestParameters.Count == 0)
            return null;

        StringBuilder sb = new StringBuilder();

        foreach (string key in requestParameters.Keys)
        {
            sb.Append(key);

            sb.Append("=");
            
            sb.Append(requestParameters[key]);

            sb.Append("&");
        }

        //마지막 데이터에 & 지우기
        if (sb.Length > 0)
            sb.Length = sb.Length - 1;

        Encoding encoding = Encoding.UTF8;
        byte[] result = Encoding.UTF8.GetBytes(sb.ToString());

        request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
        request.ContentLength = result.Length;

        return result;
    }
}
