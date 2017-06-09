using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class WWWProxy : IDisposable
{
    public WWWProxy(WWW www)
    {
        myWWW = www;
    }

    private WWW myWWW;
    private bool wwwDone = false;
    private string wwwError = null;
    private string wwwText = "";
    private byte[] wwwBinary = null;

    public WWW www
    {
        get { return myWWW; }
    }

    public bool isDone
    {
        get
        {
            if (www != null)
                wwwDone = www.isDone;

            return wwwDone;
        }

        set { wwwDone = value; }
    }

    public string error
    {
        get
        {
            if (www != null)
                wwwError = www.error;

            return wwwError;
        }

        set { wwwError = value; }
    }

    public string text
    {
        get
        {
            if (www != null)
                wwwText = www.text;

            return wwwText;
        }

        set { wwwText = value; }
    }

    public byte[] bytes
    {
        get
        {
            if (www != null)
                wwwBinary = www.bytes;

            return wwwBinary;
        }

        set { wwwBinary = value; }
    }

    public void Dispose()
    {
        if (myWWW != null)
            myWWW.Dispose();

        wwwText = null;
        wwwBinary = null;
    }
}
