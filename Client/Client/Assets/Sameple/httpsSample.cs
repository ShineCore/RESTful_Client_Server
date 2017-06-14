using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public delegate void HttpCallback(WWWProxy wwwDownload);

class HTTPResponseData
{
    public string result;
    public string error;
    public string post;
    public byte[] bytes;
    public HttpCallback callback;
}

public class httpsSample : MonoBehaviour {

    private static List<HTTPResponseData> responseDataList = new List<HTTPResponseData>();
    private volatile static bool responseDataListEmpty = true;

    // Use this for initialization
    void Start () {

        Dictionary<string, string> post = new Dictionary<string, string>();
        post.Add("nickname", "testUnity");
        post.Add("password", "testPassword");
        post.Add("create_date", "2017-6-9");

        RequestPOST("http://127.0.0.1:8080/api/accounts", post, (WWWProxy wwwDownload) =>
        {
            if (wwwDownload.error != null)
            {
                Debug.LogError("HttpCallback : " + wwwDownload.error);
            }
            else if (wwwDownload.isDone == true)
            {
                Debug.Log("HttpCallback : " + wwwDownload.text);
            }
        });

    }
	
	// Update is called once per frame
	void Update () {
        if (responseDataListEmpty == false)
        {
            lock (responseDataList)
            {
                if (responseDataList.Count <= 0)
                    return;

                HTTPResponseData data = responseDataList[0];

                try
                {
                    if (data.callback != null)
                    {
                        WWWProxy wwwProxy = new WWWProxy(null);
                        wwwProxy.error = data.error;
                        wwwProxy.isDone = string.IsNullOrEmpty(data.error) == true ? true : false;
                        wwwProxy.text = data.result;
                        wwwProxy.bytes = data.bytes;

                        data.callback(wwwProxy);

                        wwwProxy = null;
                    }
                    else
                    {
                        Debug.Log("[Callback is NULL] Post : " + data.post + ", Result : " + data.result);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    responseDataList.RemoveAt(0);

                    if (responseDataList.Count <= 0)
                        responseDataListEmpty = true;
                }
            }
        }
    }

    void HTTPAsyncCallbackBinary(HttpWebRequestCallbackState reqeustCallbackState)
    {
        MemoryStream memoryStream = new MemoryStream();

        try
        {
            int currentLength = 0;
            byte[] buffer = new byte[64 * 1024];

            Stream input = reqeustCallbackState.ResponseStream;
            int size = input.Read(buffer, 0, buffer.Length);
            while (size > 0)
            {
                memoryStream.Write(buffer, 0, size);
                currentLength += size;

                size = input.Read(buffer, 0, buffer.Length);
            }
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
        finally
        {
            HTTPResponseData responseData = (HTTPResponseData)reqeustCallbackState.State;
            responseData.result = null;
            responseData.error = reqeustCallbackState.Exception != null ? reqeustCallbackState.Exception.ToString() : null;
            responseData.bytes = memoryStream.ToArray();

            if (reqeustCallbackState.ResponseStream != null)
            {
                reqeustCallbackState.ResponseStream.Close();
                reqeustCallbackState.ResponseStream.Dispose();
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
                memoryStream.Dispose();
                memoryStream = null;
            }

            lock (responseDataList)
            {
                responseDataList.Add(responseData);
                responseDataListEmpty = false;
            }
        }
    }

    void HTTPAsyncCallback(HttpWebRequestCallbackState reqeustCallbackState)
    {
        StreamReader responseReader = null;
        string responseStreamResult = "";

        try
        {
            responseReader = new StreamReader(reqeustCallbackState.ResponseStream, Encoding.UTF8);
            responseStreamResult = responseReader.ReadToEnd();
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (reqeustCallbackState.ResponseStream != null)
            {
                reqeustCallbackState.ResponseStream.Close();
                reqeustCallbackState.ResponseStream.Dispose();
            }

            if (responseReader != null)
            {
                responseReader.Close();
                responseReader.Dispose();
                responseReader = null;
            }

            HTTPResponseData responseData = (HTTPResponseData)reqeustCallbackState.State;
            responseData.result = responseStreamResult;
            responseData.error = reqeustCallbackState.Exception != null ? reqeustCallbackState.Exception.ToString() : null;

            lock (responseDataList)
            {
                responseDataList.Add(responseData);
                responseDataListEmpty = false;
            }
        }
    }

    public void RequestPOST(string url, Dictionary<string, string> post, HttpCallback callback, Dictionary<string, string> wwwHeader = null, int httpTimeout = 20000)
    {
        var sb = new StringBuilder();
        //foreach (var key in post)
        //{
        //    sb.Append(key.Key + "=" + key.Value + "&");
        //    Debug.Log("[" + url + "] " + key.Key + "=" + key.Value);
        //}

        //if (sb.Length > 0)
        //    sb.Length = sb.Length - 1;

        sb.Append("{");

        foreach (var key in post)
        {
            sb.Append("\"" + key.Key + "\"" + ":" + "\"" + key.Value + "\"" + ",");
            Debug.Log("[" + url + "] " + key.Key + "=" + key.Value);
        }

        if (sb.Length > 0)
            sb.Length = sb.Length - 1;

        sb.Append("}");

        HTTPResponseData responseData = new HTTPResponseData();
        responseData.post = sb.ToString();
        responseData.callback = callback;

        try
        {
            HttpSocket.PostAsync(url, post, HTTPAsyncCallback, responseData, wwwHeader, "application/json;charset=utf-8");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void RequestGet(string url, HttpCallback callback)
    {
        HTTPResponseData responseData = new HTTPResponseData();
        responseData.callback = callback;

        try
        {
            HttpSocket.GetAsync(url, HTTPAsyncCallback, responseData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void RequestGetBinary(string url, HttpCallback callback)
    {
        HTTPResponseData responseData = new HTTPResponseData();
        responseData.callback = callback;

        try
        {
            HttpSocket.GetAsync(url, HTTPAsyncCallbackBinary, responseData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
