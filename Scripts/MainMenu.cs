using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using System.Net;
using UnityEngine.InputSystem.Android;

public class MainMenu : MonoBehaviour
{
    // Using singleton to pass variables between scenes: https://www.youtube.com/watch?v=BZjmqMd-4vo
     public TMP_InputField inputField;
    public static string ip;
    private string difficult;
    public TMP_Dropdown difficultDropdown;
    bool allowEnter = false;
    public void Start()
    {
        string strHostName = "";
        strHostName = System.Net.Dns.GetHostName();
        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;        
        inputField.text = addr[addr.Length - 1].ToString();
    }
    private void Awake()
    {  
        dropDown();
    }
    void Update()
    {
        var gamepad = AndroidGamepad.current;
        //if (allowEnter && (inputField.text.Length > 0) && (gamepad.aButton.ReadValue() == 1f)) {
        //        SetIp();
        //        allowEnter = false;
        //     } else allowEnter = inputField.isFocused;
        if (gamepad.aButton.ReadValue() == 1f)
        {
            SetIp();
        }
    }

    public void dropDown()
    {
        int index = difficultDropdown.value;
        //difficultDropdown.
        // When we click on the dropdown, it will show the options
        if (index == 0) {difficult = "Easy map";} // Easy
        else if (index == 1) {difficult = "Hard map";} // Hard
    }
    public void SetIp()
    {
        ip = inputField.text;
        SceneManager.LoadScene(sceneName: difficult);
    }

    
}
