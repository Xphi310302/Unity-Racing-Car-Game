using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using UnityEngine.InputSystem.Android;

public class Restart : MonoBehaviour
{
    public static Restart reStart;
    // Start is called before the first frame update
    private void Awake()
    {
        GameObject.Find("Countdown").GetComponent<CountDown_AI>().enabled = true;
    }
    private void Update()
    {
        // Init gamePad
        var gamepad = AndroidGamepad.current;

        if (gamepad.rightShoulder.ReadValue() == 1f) { ReStart(); }
        else if (gamepad.leftShoulder.ReadValue() == 1f) { Menu(); }
    }
    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        FindObjectOfType<CountDown_AI>().Reload();
    }
    public void Menu()
    {
        SceneManager.LoadScene(sceneName: "MainMenu");
    }
}
