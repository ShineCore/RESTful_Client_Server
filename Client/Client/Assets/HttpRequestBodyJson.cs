using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class HttpRequestBodyJson : Singleton<HttpRequestBodyJson>, HttpSocket.IHttpRequestBody
{
    private HttpRequestBodyJson() { }

    public byte[] CreateRequestBody(HttpWebRequest request, Dictionary<string, string> requestParameters)
    {
        if (requestParameters == null || requestParameters.Count == 0)
            return null;

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

        //마지막 데이터에 , 지우기
        if (sb.Length > 0)
            sb.Length = sb.Length - 1;

        sb.Append("}");

        Encoding encoding = Encoding.UTF8;
        byte[] result = Encoding.UTF8.GetBytes(sb.ToString());

        request.ContentType = "application/json;charset=utf-8";
        request.ContentLength = result.Length;

        return result;
    }
}
