using UnityEngine;
using System.Collections.Generic;

public class HttpTest : MonoBehaviour {
    private string inputId = "";
    private string inputNickname = "";
    private string serverReturnString = "";

    GUIStyle editTextStyle = new GUIStyle();
    GUIStyle returnTextStyle = new GUIStyle();

    void Start()
    {
        editTextStyle.fontSize = 30;
        returnTextStyle.fontSize = 20;
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 50, 50, 50) , "ID       : ", editTextStyle);
        GUI.TextArea(new Rect(10, 110, 50, 50), "Nickname : ", editTextStyle);

        inputId = GUI.TextField(new Rect(170, 50, Screen.width - 20, 50), inputId, 100, editTextStyle);
        inputNickname = GUI.TextField(new Rect(170, 110, Screen.width - 20, 50), inputNickname, 100, editTextStyle);

        if (GUI.Button(new Rect(10, 200, Screen.width - 20, 150), "Create Account") == true)
        {
            CreateAccount(inputId, inputNickname);
        }

        if (GUI.Button(new Rect(10, 200 + 170 * 1, Screen.width - 20, 150), "Get Account List") == true)
        {
            GetAccountList();
        }

        if (GUI.Button(new Rect(10, 200 + 170 * 2, Screen.width - 20, 150), "Get Account Info") == true)
        {
            GetAccountInfo(inputNickname);
        }

        if (GUI.Button(new Rect(10, 200 + 170 * 3, Screen.width - 20, 150), "Change Account Nickname") == true)
        {
            ChangeNickname(inputId, inputNickname);
        }

        if (GUI.Button(new Rect(10, 200 + 170 * 4, Screen.width - 20, 150), "Delete Account") == true)
        {
            DeleteAccountInfo(inputId);
        }

        GUI.TextArea(new Rect(10, 200 + 170 * 5 , Screen.width - 20, 150), serverReturnString);
    }

    void CreateAccount(string id, string nickname)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("id", id);
        parameters.Add("nickname", nickname);

        HttpSocket.SendPOST("http://127.0.0.1:8080/api/accounts", HttpRequestBodyJson.Instance, parameters, (string result) =>
        {
            Debug.Log("---- " + result);

            serverReturnString = result;
        }, true);
    }

    void GetAccountList()
    {
        HttpSocket.SendGET("http://127.0.0.1:8080/api/accounts", (string result) =>
        {
            Debug.Log("---- " + result);

            serverReturnString = result;
        }, true);
    }

    void GetAccountInfo(string nickname)
    {
        HttpSocket.SendGET("http://127.0.0.1:8080/api/accounts/" + nickname, (string result) =>
        {
            Debug.Log("---- " + result);

            serverReturnString = result;
        }, true);
    }

    void ChangeNickname(string id, string nickname)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("id", id);
        parameters.Add("nickname", nickname);

        HttpSocket.SendPUT("http://127.0.0.1:8080/api/accounts/" + id, HttpRequestBodyJson.Instance, parameters, (string result) =>
        {
            Debug.Log("---- " + result);

            serverReturnString = result;
        }, true);
    }

    void DeleteAccountInfo(string id)
    {
        HttpSocket.SendDELETE("http://127.0.0.1:8080/api/accounts/" + id, (string result) =>
        {
            Debug.Log("---- " + result);

            serverReturnString = result;
        }, true);
    }
}
