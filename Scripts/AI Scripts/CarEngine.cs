using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public Transform trackWaypoints;
    private List<Transform> nodes;
    public float maxSteerAngle = 45f;
    public Transform leftFrontWheel, rightFrontWheel;
    [HideInInspector]
    public Vector3 relativeVector;
    private int currentNode;



    public LayerMask whatIsGround;
    public Rigidbody theRB;
    private bool grounded;
    public float groundRayLength = .5f, forwardStrength=4000f, turnStrength=10f;
    [HideInInspector] public float turnInput, Impulse = 0.0000000000001f; 
    public Transform groundRayPoint;
    public Rigidbody carColliderRB;
    public float dragOnGround = 3f, gravityForce = 9.81f;

    GameObject car;

    [SerializeField]
    AudioSource driving_bot;

    void Start()
    {
        Transform[] path = trackWaypoints.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i =1; i< path.Length; i++)
        {
            if(path[i] != trackWaypoints.transform)
            {
                nodes.Add(path[i]);
            }
            //nodes.Add(path[i]);                  
        }
        theRB.transform.parent = null;
        carColliderRB.transform.parent = null;
        driving_bot.Play();
    }

    // Update is called once per frame
    void Update()
    {   
        ApplySteer();
        transform.position = theRB.transform.position;

        car = GameObject.Find("Car");
        float distance_car = (car.transform.position - transform.position).magnitude;
        if(distance_car<30) driving_bot.volume = 1 - distance_car/30;
        else driving_bot.volume = 0;
        // Debug.Log(distance_car);
    }
    void FixedUpdate()
    {
        checkWaypointsDistance();
        Drive();
    }
    private void ApplySteer()
    {
        relativeVector = theRB.transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        // Make the wheel rotate
        if (relativeVector.x > 1f)
        {turnInput = 1;}
        else if (relativeVector.x < -1f)
        {turnInput = -1;}
        else 
        {turnInput = 0;}
         if (grounded)
        {
            // Apply the input rotation to the car
            if (Mathf.Abs(relativeVector.x) > 1)
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, newSteer * turnStrength * Time.deltaTime, 0f));
        }
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (newSteer)-180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, newSteer, rightFrontWheel.localRotation.eulerAngles.z);
    }
    private void Drive()
    {
        grounded = false;
        RaycastHit hit;
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        
        }
        carColliderRB.MoveRotation(transform.rotation);
        if(grounded)
        {
            theRB.drag =  dragOnGround;
            if (Mathf.Abs(relativeVector.x) > 1) {theRB.AddForce(transform.forward * forwardStrength/4f );}
            {theRB.AddForce(transform.forward * forwardStrength );}                           
        }
        // Car is the air
        else
        {   
            theRB.drag = 0.1f;
            theRB.AddForce(Vector3.up * gravityForce * -100f);
        }        
    }

    private void checkWaypointsDistance()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 6f)
        {
            if(currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }
    public void Boost() { theRB.AddForce(transform.forward * Impulse, ForceMode.Impulse); }

    public void stop_volume() {driving_bot.Stop();}

}
