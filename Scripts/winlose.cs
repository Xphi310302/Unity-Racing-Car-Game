using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class winlose : MonoBehaviour
{
    public GameObject winlose_text;
    public bool win;
    void OnTriggerEnter(Collider vehicle)
    {
        if (vehicle.gameObject.tag == "Car")
            win = true;
        else if (vehicle.gameObject.tag == "Bot")
            win = false;
        if (win)
        {
            winlose_text.GetComponent<TextMeshProUGUI>().text = "You Win";
            winlose_text.SetActive(true);
        }
           
        else {
            winlose_text.GetComponent<TextMeshProUGUI>().text = "You Lose";
            winlose_text.SetActive(true);
            }
        FindObjectOfType<CarEngine>().stop_volume();
        FindObjectOfType<CarController>().stop_volume();
        GameObject.Find("Car").GetComponent<CarController>().enabled = false;
        GameObject.Find("Bot").GetComponent<CarEngine>().enabled = false;
    
        StopAllCoroutines();

    }

}