using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.Android;
using UnityEngine.InputSystem;

public class CountDown_AI : MonoBehaviour
{
    //https://www.youtube.com/watch?v=HvVxq7XC6k8
    public GameObject countDown;
    
    //public GameObject lapTimer;
    void Start()
    {
        Reload();
    }
    public void Reload()
    {
        StartCoroutine(CountDownRoutine());
    }

    IEnumerator CountDownRoutine()
    {
        GameObject.Find("Car").GetComponent<CarController>().enabled = true;
        countDown.SetActive(false);
        //lapTimer.SetActive(true);
        countDown.GetComponent<TextMeshProUGUI>().text = "3";
        countDown.SetActive(true);
        yield return new WaitForSeconds(1f);

        //lapTimer.SetActive(true);
        countDown.SetActive(false);
        countDown.GetComponent<TextMeshProUGUI>().text = "2";
        countDown.SetActive(true);
        yield return new WaitForSeconds(1f);

        //lapTimer.SetActive(true);
        countDown.SetActive(false);
        countDown.GetComponent<TextMeshProUGUI>().text = "1";
        countDown.SetActive(true);
        yield return new WaitForSeconds(1f);
        
        countDown.SetActive(false);
        
    }
    void Update()
    {
        var gamepad = AndroidGamepad.current;

        if(gamepad.aButton.ReadValue()==1f)
        GameObject.Find("Bot").GetComponent<CarEngine>().enabled = true;
     
    }

}


