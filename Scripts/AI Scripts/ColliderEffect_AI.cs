using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEffect_AI : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "boost")
        {
            FindObjectOfType<CarEngine>().Boost();
        }
    }
}
