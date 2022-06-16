using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointsManager : MonoBehaviour
{
   public trackWaypoints waypoints;
   public Transform currentWaypoint;
   public List<Transform> nodes = new List<Transform>();
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 5)] public float steerForce;
   private void Awake()
   {
       waypoints = GameObject.FindGameObjectWithTag("path").GetComponent<trackWaypoints>();
       nodes = waypoints.nodes;
       
     
   }
    private void FixedUpdate()
    {
        AISteer();
        calculateDistanceWaypoints();
    }


    private void calculateDistanceWaypoints()
    {
        Vector3 position = gameObject.transform.position;
        float distance = Mathf.Infinity;

        for(int i=0; i<nodes.Count; i++)
        {
            //Vector3 difference = nodes[i].position - position;
            //float currentDistance = difference.magnitude;
            float currentDistance = Vector3.Distance(position, nodes[i].position);
            if(currentDistance < distance)
            {
                currentWaypoint = nodes[i + distanceOffset];
                distance = currentDistance;
            }
        }
    }
    private void AISteer()
    {
        Vector3 relative = transform.InverseTransformPoint(currentWaypoint.position);
        relative /= relative.magnitude;
        //horizontal = (relative.x / relative.magnitude) * steerForce;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(currentWaypoint.position, 3);
    }
}

