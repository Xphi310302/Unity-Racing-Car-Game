using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderEffect : MonoBehaviour
{
    [HideInInspector] public bool isRight;
    
    void OnTriggerEnter(Collider other)
   {
        if(other.gameObject.tag == "boost")
        {
           FindObjectOfType<CarController>().Boost();
        }
        GameObject car = GameObject.Find("Car");
        CarController carController = car.GetComponent<CarController>();
        if (carController.transform.position.x == other.transform.position.x)
        {
            isRight = false;
        }
        if (carController.transform.position.z == other.transform.position.z)
        {
            isRight = true;
                 
        }
        
        if(other.gameObject.tag == "outside")
         
         {
            //Debug.Log("Collided with outside");
            //FindObjectOfType<CarController>().reSpawn();
         } 
        
          
   }
    void OnTriggerStay(Collider roads)
   {

       if(roads.gameObject.tag == "roadStraight")
       {
           FindObjectOfType<CarController>().diffCal_RoadStraight(roads.gameObject);
           
       }
       if(roads.gameObject.tag == "roadCorner")
       {
            FindObjectOfType<CarController>().diffCal_RoadCorner(roads.gameObject);
       }
    }
  
    void OnTriggerExit(Collider roads)
    {
        if(roads.gameObject.tag == "roadStraight")
        {
            FindObjectOfType<CarController>().findRoadWidth(roads.gameObject);
        }
    }

     void OnCollisionEnter(Collision collision)
   {
         if(collision.gameObject.tag == "outside")
         
         {
            Debug.Log("Collided with outside");
          
            // Send data to server
            //ApplyForce(CarController.theRB);
         }
   }














/*
        //Collision detection
   void OnCollisioStay(Collision collision)
   {
       if(collision.gameObject.tag == "Obstacles")
       {
            Debug.Log("Stay with obstacles");

           // Send data to server
           FindObjectOfType<CarController>().Boost();
       }
   }

   void OnCollisionExit(Collision collision)
   {
       if(collision.gameObject.tag == "Obstacles")
       {
           Debug.Log("Exit with obstacles");   
           // Send data to server
       }
   }
*/
}
