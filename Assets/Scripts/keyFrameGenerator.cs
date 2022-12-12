using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.XR.Interaction.Toolkit;

// Script attached to all animatable objects. Generates keyframes, and contains functions affecting all keyframes at once.

public class keyFrameGenerator : MonoBehaviour
{
    public Vector3 keyFramePosition; // The position of a given keyframe
    public Color keyFrameColor; //Declares color object which controls the color of the keyframes

    Vector3 startPos; // Starting position of the object
    Quaternion startRot; // Starting rotation of the object. Rotations are not utilized in the final product.

    public List <Vector3> keyFrameList; // List of all keyframe positions
    public List <Quaternion> keyFrameRotations; // List of all keyframe rotations

    Rigidbody rigi; // Rigidbody of the object

    public LineRenderer lineRenderer; // Line renderer, draws the line between keyframes

    public XRCustomGrabInteractable xrCustom; // The custom XR Grab Interactable Script
    public GameObject manager; // Empty object containing the gameManager script
    public gameManager mng; // GameManager

    public GameObject[] existingKFs; // Checks for already existing keyframes before the program begins. Used in Task #2, where an animation is already created.
        
    public int counter; // Counts the number of keyframes present
    public float keyFrameSpacing; // The distance to move an object before another keyframe is created
    public bool toggleVis = true; // Bool for "Toggle Visibility"
    public bool animToggle = true; // Bool for "Animating" 
    public bool isActive = false; // Bool for setting the object as the current active object


    void Start()
    {
        // Set value of variables
        keyFrameSpacing = 0.5f;
        counter = 0;
        keyFrameList = new List<Vector3>();
        keyFramePosition = new Vector3(0,0,0); 
        keyFrameColor = new Color(255f, 0f, 0f);
        startPos = gameObject.transform.position;
        startRot = gameObject.transform.rotation; 

        // Other scripts to be accessed
        xrCustom = gameObject.GetComponent<XRCustomGrabInteractable>();
        manager = GameObject.Find("GameManager");
        mng = manager.GetComponent<gameManager>();
        rigi = GetComponent<Rigidbody>();

        // Add and define the line renderer
        lineRenderer = this.gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0f,255f,0f,0.0f);
        lineRenderer.endColor = new Color(0f,255f,0f,0.0f);  

        // If there are existing keyframes, add them to keyFrameList and draw the line between them
        existingKFs = GameObject.FindGameObjectsWithTag("Keyframe");
        if(existingKFs != null){
            for(int i = 0; i < existingKFs.Length; i++){
                for(int j = 0; j < existingKFs.Length; j++)
                    if (existingKFs[j].name.Contains($"{this.gameObject.name} Keyframe {i}"))
                    {
                        keyFrameList.Add(existingKFs[j].transform.position);
                        Debug.Log(existingKFs[j]);
                        continue;
                    }
        }
        lineDrawer();
        }
    }


    //keyFrameSpawner: Set position of spawned keyframes
    public GameObject keyFrameSpawner (){  
        // Use "sphereSpawn()" to define the properties of the keyframe
        GameObject sphere = sphereSpawn();

        // Set the keyframes position to the object position
        sphere.transform.position = rigi.transform.position;

        // Add the position and rotation to their respective lists
        keyFrameList.Add(sphere.transform.position); 
        keyFrameRotations.Add(rigi.transform.rotation);

        // Increase counter
        counter ++;

        // Update the line renderer
        lineDrawer();
        return sphere;
                    
    }


    //sphereSpawn: Sets the properties of the keyframe
    public GameObject sphereSpawn (){
        // Set the color and set the collider to true
        GetComponent<Renderer>().material.color = keyFrameColor;
        GetComponent<SphereCollider>().isTrigger = true;         

        // Creates a sphere for the keyframe, set color and add the "keyframeBehavior" script to it
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Renderer>().material.color = keyFrameColor; 
        sphere.AddComponent<keyframeBehavior>();

        // Set the tag, scale and name of the keyframe - the name includes the name of its associated object
        sphere.tag = "Keyframe";
        sphere.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        sphere.name = ($"{this.gameObject.name} Keyframe {counter}");

        // Adds "XRCustomGrabInteractable" to the sphere, and adjusts it alongside the rigidbody
        XRCustomGrabInteractable sphereGrab = sphere.AddComponent<XRCustomGrabInteractable>();
        Rigidbody sphereRB = sphere.GetComponent<Rigidbody>();
        sphereRB.isKinematic = true;
        sphereRB.useGravity = false;
        sphereGrab.throwOnDetach = false;
        sphereGrab.trackRotation = false;

        return sphere;
    }


    //lineDrawer: Updates the line renderer
    void lineDrawer () {
        // Only update if keyframes are present
        if (keyFrameList.Count > 1){

            // Sets the color and width
            lineRenderer.startColor = new Color(255f,255f,255f,0.4f);
            lineRenderer.endColor = new Color(255f,255f,255f,0.4f);
            lineRenderer.startWidth = 0.03f;
            lineRenderer.endWidth = 0.03f;

            // Sets the positions of the line renderer to be the same as the positions of the keyframes
            lineRenderer.positionCount = keyFrameList.Count;
            lineRenderer.SetPositions(keyFrameList.ToArray());
        }
    }


    //resetAnim: Delete all this object's keyframes
    public void resetAnim(){

        // Array of all keyframes 
        GameObject[] k = GameObject.FindGameObjectsWithTag ("Keyframe");

        // Set the line renderer invisible
        lineRenderer.startColor = new Color(0f,0f,0f,0f);
        lineRenderer.endColor = new Color(0f,0f,0f,0f);
        
        // To make sure only this object's keyframes are deleted, check if the name of the object appears in the keyframe's name
        for(var i = 0 ; i < k.Length ; i ++)
        {
            if(k[i].name.Contains(gameObject.name)){
                Destroy(k[i]);
            }
        }
        
        // Empty the keyFrameList and reset the counter
        keyFrameList.Clear();
        counter = 0;

        // Place the object where it started
        this.transform.position = startPos;
        this.transform.rotation = startRot;
    }


    //toggleVisibility & changeVisibility: Turns the visibility on/off
    public void toggleVisibility(){

        // Set the visibility on if it was off, and off if it was on
        toggleVis = !toggleVis;
        changeVisibility();
    }

    public void changeVisibility(){

        // Visibility is off
        if (toggleVis == false){

            // Find each keyframe and turn off their mesh renderer
            GameObject[] k = GameObject.FindGameObjectsWithTag ("Keyframe");
            
            for(var i = 0 ; i < k.Length ; i ++)
            {
                k[i].GetComponent<MeshRenderer>().enabled = false;
            }

            // Find all line renderers in the scene, except for the ones used by the controller raycast, and disable them
            var lr = FindObjectsOfType<LineRenderer>();
            foreach (LineRenderer l in lr){
                if (l.name.Contains("Hand"))
                {
                    Debug.Log("It's a hand!");
                }
                else
                {
                    l.enabled = false;
                }
            }
        }

        // Visibility is on - exactly the same as before, but enabling instead of disabling
        else 
        {
            GameObject[] k = GameObject.FindGameObjectsWithTag ("Keyframe");
            
            for(var i = 0 ; i < k.Length ; i ++)
            {
                k[i].GetComponent<MeshRenderer>().enabled = true;
            }

            var lr = FindObjectsOfType<LineRenderer>();
            foreach (LineRenderer l in lr){
                if (l.name.Contains("Hand"))
                {
                    Debug.Log("It's a hand!");
                }
                else
                {
                    l.enabled = true;
                }
            }
        }
    }

    //animToggleVoid: Turns on/off the "animating" mode, allowing the objects to be grabbed without generating keyframes
    public void animToggleVoid(){
        animToggle = !animToggle;
    }

    //addKeyFrame: Manually adds a single keyframe to the animation, without having to pick up the object
    public void addKeyFrame(){
        if(keyFrameList.Count > 1){

            // Create the keyframe
            GameObject sphere = sphereSpawn();

            // Set the spawn position of this keyframe as the opposite vector of the last two keyframes in the list
            // Ensures that the new keyframe will not overlap with existing ones
            int lastPos = keyFrameList.Count - 1;
            Vector3 spawnPos = keyFrameList[lastPos] + (keyFrameList[lastPos] - keyFrameList[lastPos - 1]);
            sphere.transform.position = spawnPos;

            // Add it to the list like normal, increase the counter, and update the line
            keyFrameList.Add(sphere.transform.position); 
            keyFrameRotations.Add(rigi.transform.rotation);
            counter ++;
            lineDrawer(); 
        }
    }

    //keyFramePositionUpdate: Update the position of a keyframe when grabbed and moved
    public void keyFramePositionUpdate(Vector3 position, string keyFrameID){

        // Find the number of the keyframe
        string numberStr = keyFrameID.Replace($"{this.gameObject.name} Keyframe ", "");
        int keyFrameNumber = int.Parse(numberStr);

        // Update the keyFrameList with the new position and update the line
        keyFrameList[keyFrameNumber] = position;
        lineDrawer(); 
    }

    //keyFrameSniper: Deletes a single keyframe while in "Eraser Mode". Originally called "Sniper Mode", hence the name.
    public void keyFrameSniper(string keyFrameID){
        // Find all keyframe objects and adds them to a list
        GameObject[] k = GameObject.FindGameObjectsWithTag("Keyframe");
        List<Object> kfSeperatedList = new List<Object>();

        // Find the number of the keyframe, remove it from keyFrameList
        string numberStr = keyFrameID.Replace($"{this.gameObject.name} Keyframe ", "");
        int keyFrameNumber = int.Parse(numberStr);
        keyFrameList.RemoveAt(keyFrameNumber);
        counter--;

        // Add the objects remaining keyframes to a list
        for (var i = 0; i < k.Length; i++){
                    if(k[i].name.Contains(this.gameObject.name)){
                        kfSeperatedList.Add(k[i]);
                    }
                }

        // Re-number the keyframes to match the new total amount
        for(var i = 0 ; i < kfSeperatedList.Count ; i++)
            {
                kfSeperatedList[i].name = ($"{this.gameObject.name} Keyframe {i}");  
            }

        // Update the line
        lineDrawer();
    }

    void Update()
    {
        // Keyframes can only be drawn if the following conditions are met:
        // 1) Object must be grabbed 
        // 2) Object must be in "Animating Mode" 
        // 3) Object must not be in "Eraser Mode" 
        // 4) Object must be active
        if(xrCustom.isGrabbed == true && animToggle == true && mng.sniperMode == false && isActive == true){

            // Every frame, calculate the distance between the object and the last keyframe
            float dist = Vector3.Distance(transform.position, keyFramePosition);

            // If it exceeds keyFrameSpacing, create a new keyframe at that position, and update the line
            if (dist > keyFrameSpacing){
                keyFramePosition = rigi.transform.position;
                keyFrameSpawner();    
                lineDrawer();  
            }
        }

        // Sets the object as active when grabbed
        if(xrCustom.isGrabbed == true){
            if(isActive == false){
                mng.changeActive(this.gameObject);
            }
        }
    }
}