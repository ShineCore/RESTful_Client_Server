using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

public class HttpTest : MonoBehaviour {
    void OnGUI()
    {
        if(GUI.Button(new Rect(10, 10, Screen.width - 20, 150), "Create Account") == true)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("nickname", "BB");
            parameters.Add("password", "BB_PASSWORD");

            HttpSocketJson.RequestPost("http://127.0.0.1:8080/api/accounts", parameters, (string result) =>
            {
                Debug.Log("---- " + result);
            });
        }
    }
}
