using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.InputSystem.Android;
using UnityEngine.InputSystem;
// using UnityEngine.InputSystem;
using TMPro;
// Variables for send IP form MainMenu to Track

public class CarController : MonoBehaviour
{
    #region private members 	
    /// <summary> 	
    /// TCPListener to listen for incomming TCP connection 	
    /// requests. 	
    /// </summary> 	
    private TcpListener tcpListener;
    /// <summary> 
    /// Background thread for TcpServer workload. 	
    /// </summary> 	
    private Thread tcpListenerThread;
    /// <summary> 	
    /// Create handle to connected tcp client. 	
    /// </summary> 	
    private TcpClient connectedTcpClient;
    #endregion
    public static CarController carController;
    private Vector3 initPos;
    public string ip_copy;
    public Rigidbody theRB;
    public Rigidbody carColliderRB;
    public float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 80f, gravityForce = 9.81f, dragOnGround = 3f, Impulse=2.5e10f;
    private float speedInput, turnInput;
    private bool grounded;
    public LayerMask whatIsGround;
    public float groundRayLength = .5f;
    public Transform groundRayPoint;
    public Transform leftFrontWheel, rightFrontWheel, leftRearWheel, rightRearWheel;
    public float maxWheelTurn = 25f;
    public GameObject CineCam;
    public float limitTurn = 0.022f;
    string angleY;
    string velocityAngleY;
    string velocityX;
    string displacementX;
    float sensivity = 0.75f;
    [HideInInspector] public float rot_y, diffCar;

    public int count = 0;
    public bool move = true;
    [SerializeField]
    AudioSource driving;
    [SerializeField]
    AudioSource idle;
    [SerializeField]
    AudioSource starting;
    [SerializeField]
    AudioSource stop;
    
    void Start()
    {
       
        if (ip_copy == "")
        {
        //ip_copy = MainMenu.mainMenu.ip;
        ip_copy = MainMenu.ip;
        }

        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests))
        {
            IsBackground = true
        };
        tcpListenerThread.Start();
        // idle.Play();

        // Set the rigidbody of the sphere not belonging to the car
        theRB.transform.parent = null;
        carColliderRB.transform.parent = null;
        initPos = theRB.position;
    
        idle.Play();
    }


    // Update is called once per frame
    void Update()
    {
        // Init gamePad
        var gamepad = AndroidGamepad.current;

        speedInput = gamepad.leftStick.y.ReadValue();
        speedInput = speedInput > 0 ? speedInput * forwardAccel * 600f : speedInput * reverseAccel * 600f;

        turnInput = gamepad.rightStick.x.ReadValue()* sensivity;
   
        //speedInput = Input.GetAxis("Vertical") > 0 ? Input.GetAxis("Vertical") * forwardAccel * 600f : Input.GetAxis("Vertical") * reverseAccel * 600f;
        //turnInput = Input.GetAxis("Horizontal");

        if (grounded)
        {
            // Apply the input rotation to the car
            int turn = transform.InverseTransformDirection(theRB.velocity).z != 0 ? 1 : -1;
            if (Mathf.Abs(theRB.velocity.z) > limitTurn) 
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * turn * Time.deltaTime, 0f));
            
        }

        // Create the wheel's movement
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn)-180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);
        
        // Car follows the sphere
        transform.position = theRB.transform.position;
        SendMessage();

        if(Mathf.Abs(speedInput)>0 && count == 0)
        {
            StopAllCoroutines();
            StartCoroutine(FadeClip(idle,starting,0.1f));
            count = 1;
        }
        else if(Mathf.Abs(speedInput)>1 && move)
        {
            StopAllCoroutines();
            StartCoroutine(FadeClip(starting,driving,0.1f));
            move = false;
        }
        else if(Mathf.Abs(speedInput)<0.1 && count==1)
        {
            StopAllCoroutines();
            StartCoroutine(FadeClip(driving,stop,0.1f));
            count = 0;
            move = true;
        }
    }

    private void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;
        // Check if the car is grounded
        // Physics.Raycast basically checks if there is a collider with the ground
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            // Simple math to afect the rotation of the car
            // hit.normal basically tells us whatever the angle of the ground is that we hit against 
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
         
        }

        // Apply the rotation to CarCollider
        carColliderRB.MoveRotation(transform.rotation);
        if(grounded)
        {
            // Update the car's drag
            theRB.drag =  dragOnGround;
            // Check if the car is moving
            if(Mathf.Abs(speedInput) > 0)
            {
                // Car moves forward
                theRB.AddForce(transform.forward * speedInput);
                //theRB.AddForce(0,0,speedInput, ForceMode.Acceleration);
            }
        }
        // Car is the air
        else
        {   
            // Reduce the drag so that the car falls slower
            theRB.drag = 0.1f;
            // Apply gravity to the car
            theRB.AddForce(Vector3.up * gravityForce * -100f);
        }
        double velocityAngleY_temp = Mathf.Abs(transform.InverseTransformDirection(carColliderRB.angularVelocity).y);
        //velocityAngleY_temp = velocityAngleY_temp * 10 * 180 / 3.14;
        //Debug.Log(velocityAngleY_temp);
        

    }

    //Fucntion for boosting velocity
    public void Boost() {theRB.AddForce(transform.forward * Impulse, ForceMode.Impulse);}  
   
    /*
    //Function for respawning the car
    public void reSpawn()
    {
        //Start the coroutine we define below named delayForRespawn.
        
        StartCoroutine(delayForRespawn(2f));
         
        StartCoroutine(delay(2f));
    }
    // IEnumerator for delaying
    IEnumerator delayForRespawn(float waitTime)
    {   
      
        theRB.transform.position = initPos;
        CineCam.transform.position = new Vector3(3.7f, 5f, 14f);
        transform.rotation = Quaternion.Euler(new Vector3(0f,0f,0f)); 
        theRB.velocity = Vector3.zero;       
        //canMove = false;
        yield return new WaitForSeconds(waitTime);
        
        
       
    }
    IEnumerator delay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canMove = true;
        
    }
    */

    //Function to calculate distance between the car and the origin of the road for straight line movement
    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        // n is normal vector to determine clockwise or counter-clockwise
        // angle in [0,180]
        float angle = Vector3.Angle(a,b);
        float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;
        return signed_angle;
    }
    public void diffCal_RoadStraight(GameObject road)
    {
        float angleDistance = transform.rotation.eulerAngles.y -  road.transform.eulerAngles.y;
        angleDistance = (angleDistance > 180) ? angleDistance - 360 : angleDistance;
        //Debug.Log("angleDistance: " + angleDistance);
        float roadWidth;
        Vector3 direction;
        //Debug.Log("Size of the road: " + road.GetComponent<Renderer>().bounds.size);
        float sizex = road.GetComponent<Renderer>().bounds.size.x;
        float sizez = road.GetComponent<Renderer>().bounds.size.z;
        if (sizex <= sizez)
        {
            roadWidth = sizex;
            Vector3 relativePos = transform.position - road.transform.position;
            diffCar = Mathf.Abs(relativePos[0]) - roadWidth/2;
            
            //print("dir z");
            if ((road.transform.eulerAngles.y == 90) | (road.transform.eulerAngles.y == 270)) { direction = road.transform.right; }
            else direction = road.transform.forward;
            if (Vector3.Dot(transform.forward, direction) < 0) { direction *= -1; }


        }
        else 
        {
            roadWidth = sizez;
            Vector3 relativePos = transform.position - road.transform.position;
            //Rotate the car 's postion to take the exact position of the car with respect to the road's axis
            diffCar = Mathf.Abs(relativePos[2]) - roadWidth/2;
            
            //print("dir x");
            if ((road.transform.eulerAngles.y == 90) | (road.transform.eulerAngles.y == 270)) { direction = road.transform.forward; }
            else direction = road.transform.right;
            if (Vector3.Dot(transform.forward, direction) < 0) { direction *= -1; }

        }
        diffCar = (Mathf.Abs(angleDistance) < 90) ? diffCar : -diffCar;
        diffCar *= 100;
        rot_y = SignedAngleBetween(transform.forward, direction, Vector3.up);
        //Debug.Log("rot_y " + rot_y);
    }
    private float roadWidthCorner;
    public void findRoadWidth(GameObject road)
    {
        roadWidthCorner = road.GetComponent<Renderer>().bounds.size.x <= road.GetComponent<Renderer>().bounds.size.z ? road.GetComponent<Renderer>().bounds.size.x : road.GetComponent<Renderer>().bounds.size.z;
        roadWidthCorner = 10;
    }


    //Function to calculate distance between the car and the origin of the road for curve movement
        // Sub-Function for calculating whether the car is on the left or right side of curve's center point
        float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) 
        {
            //Return 1 if is on the right, -1 if is on the left, 0 if is on the straight line
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);
            if (dir > 0f) {
                return 1f;
            } else if (dir < 0f) {
                return -1f;
            } else {
                return 0f;
            }
        }
    [HideInInspector] public void diffCal_RoadCorner(GameObject road)
    {
        float radius;
        Vector3 center = new Vector3(0,0,0);
        //Debug.Log("Size of the road: " + road.GetComponent<Renderer>().bounds.size);
        float sizex = road.GetComponent<Renderer>().bounds.size.x;
        float sizez = road.GetComponent<Renderer>().bounds.size.z;
        if (sizex != sizez) {Debug.Log("Road is not radial");}
        radius = sizex - roadWidthCorner/2;
        if (road.transform.eulerAngles.y == 0) {center = road.transform.position + new Vector3(sizex, 0, 0);}
        else if (road.transform.eulerAngles.y == 90) {center = road.transform.position - new Vector3(0, 0, sizex);}
        else if (road.transform.eulerAngles.y == 180 | road.transform.eulerAngles.y == -180) {center = road.transform.position - new Vector3(sizex, 0, 0);}
        else if (road.transform.eulerAngles.y == 270) {center = road.transform.position + new Vector3(0, 0, sizex);}
        diffCar = Vector3.Distance(transform.position,center);
        diffCar -= radius;
        //Debug.Log("Car's direction" + transform.forward);
        Vector3 heading = center - transform.position ;
        float dirNum = AngleDir(transform.forward, heading, transform.up);
        if (dirNum == 1f){diffCar *= -1;}
        diffCar *= 100;
        Vector3 radiusVector = transform.position - center;
        //print(radiusVector);

        rot_y = Mathf.Abs(SignedAngleBetween(transform.forward, radiusVector, Vector3.up)) - 90;
        rot_y *= dirNum == 1f ? 1 : -1;
        //print("rot_y " + rot_y);
    }

    // Function for TCP connection
    private void ListenForIncommingRequests()
    {
        try
        {
            // Create listener on localhost port 8052. 			
            tcpListener = new TcpListener(IPAddress.Parse("192.168.0.103"), 3256);
            tcpListener.Start();
            Debug.Log("Server is listening");
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    // Get a stream object for reading 					
                    using NetworkStream stream = connectedTcpClient.GetStream();
                    int length;
                    // Read incomming stream into byte arrary. 						
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {     
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 							
                        string clientMessage = Encoding.ASCII.GetString(incommingData);
                        //Debug.Log("client message received as: " + clientMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }    
    //Function to send message to Track
    private void SendMessage()
    {
        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                rot_y = rot_y + 0.111111111111111f;
                angleY = rot_y.ToString();
               
                float velocityAngleY_temp = Mathf.Abs(transform.InverseTransformDirection(carColliderRB.angularVelocity).y)+0.111111111111111f;
                if (velocityAngleY_temp < 0.001f) { velocityAngleY_temp= 0.001f; }
                velocityAngleY = velocityAngleY_temp.ToString();
                Debug.Log(velocityAngleY_temp);
                
                float velocityX_temp = Mathf.Abs(transform.InverseTransformDirection(theRB.velocity).x)+ 0.111111111111111f;
                if (velocityX_temp < 0.001f) { velocityX_temp = 0.001f; }
                velocityX = velocityX_temp.ToString();
                diffCar += 0.111111111111111f;
                displacementX = diffCar.ToString();


                //angleY = angleY.Substring(0,8);
                //velocityAngleY = velocityAngleY.Substring(0,8);
                //velocityX = velocityX.Substring(0,8);
                //displacementX = displacementX.+Substring(0,8);
                
                string serverMessage = displacementX+ ':' + angleY +  ':' + velocityX + ':' + velocityAngleY + "\r\n" ;

                //Debug.Log(serverMessage);
                // Convert string message to byte array.                 
                byte [] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                // Write byte array to socketConnection stream.               
                stream.Write(serverMessageAsByteArray, 0, serverMessage.Length);
            }

        }
        catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }

    private IEnumerator FadeClip(AudioSource s1, AudioSource s2,float timetofade)
    {
        float timeElapsed = 0;
        s2.Play();
        while (timeElapsed < timetofade)
        {
            s2.volume = Mathf.Lerp(0, 1, timeElapsed / timetofade);
            s1.volume = Mathf.Lerp(1, 0, timeElapsed / timetofade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        s1.Stop();
    }

    public void stop_volume() {driving.Stop();}
}
