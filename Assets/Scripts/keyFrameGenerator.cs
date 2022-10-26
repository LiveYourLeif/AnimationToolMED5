using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.XR.Interaction.Toolkit;

public class keyFrameGenerator : MonoBehaviour
{
    // This is Tobi's Branch!
    public int counter; // Declares varaible that counter the amount of keyframe present in the interface
    public Vector3 keyFramePosition;
    public List <Vector3> keyFrameList;
    public List <Quaternion> keyFrameRotations;
    Rigidbody rigi; 
    public Color keyFrameColor; //Decalres color object which controls the color of the keyframes
    LineRenderer lineRenderer;
    public XRCustomGrabInteractable xrCustom;
    public float keyFrameSpacing {get; set;}
    public bool toggleVis = true;
    public bool animToggle = true;

    
    


    // Start is called before the first frame update
    void Start()
    {
        xrCustom = gameObject.GetComponent<XRCustomGrabInteractable>();
        rigi = GetComponent<Rigidbody>();
        keyFrameList = new List<Vector3>();
        keyFramePosition = new Vector3(0,0,0); 
        counter = 0; // Assining the counter variable to 0
        keyFrameColor = new Color(255f, 0f, 0f); //Color object initiates with RGB color values
        lineRenderer = this.gameObject.AddComponent<LineRenderer>(); //spawner en thick line i 0,0,0.. fix det
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0f,255f,0f,0.0f);
        lineRenderer.endColor = new Color(0f,255f,0f,0.0f);
        keyFrameSpacing = 0.5f;
      
        
        
               
    }


//keyFrameSpawner, spawns a sphere for each keyfram. 
public GameObject keyFrameSpawner (){  
    GameObject sphere = sphereSpawn();
    sphere.transform.position = rigi.transform.position; //Set its position to the player
    keyFrameList.Add(sphere.transform.position); 
    keyFrameRotations.Add(rigi.transform.rotation);
    /*string result = "List contents: ";
    foreach (var keyframe in keyFrameRotations) //Printer coordinates pÃ¥ alle punkterne
        {
             result += keyframe.ToString() + ", ";
        }
    Debug.Log(result);   */
    counter ++;
    return sphere;
                
 }


public GameObject sphereSpawn (){
    GetComponent<Renderer>().material.color = keyFrameColor; //Change its color  
    GetComponent<SphereCollider>().isTrigger = true;     //Delete its collider
    //transform.localScale = new Vector3(0.5f,0.5f,0.5f);  //Scale the sphere down by 50%                    
    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);   //Create the sphere
    sphere.GetComponent<Renderer>().material.color = keyFrameColor; //Change its color  
    //sphere.GetComponent<SphereCollider>().isTrigger = true;     //Delete its collider
    sphere.AddComponent<keyframeBehavior>();
    XRCustomGrabInteractable sphereGrab = sphere.AddComponent<XRCustomGrabInteractable>();
    Rigidbody sphereRB = sphere.GetComponent<Rigidbody>();
    sphereRB.isKinematic = true;
    sphereRB.useGravity = false;
    sphereGrab.throwOnDetach = false;
    sphereGrab.trackRotation = false;
    sphere.tag = "Keyframe";
    sphere.transform.localScale = new Vector3(0.1f,0.1f,0.1f);  //Scale the sphere down by 50%
    sphere.name = ($"Keyframe {counter}");      //Change its name to keyframe, plus a counter
    return sphere;
}

void lineDrawer () {
    if (keyFrameList.Count > 1){
    lineRenderer.startColor = new Color(0f,255f,0f,0.4f);
    lineRenderer.endColor = new Color(0f,255f,0f,0.4f);
    lineRenderer.startWidth = 0.01f;
    lineRenderer.endWidth = 0.01f;
    lineRenderer.positionCount = keyFrameList.Count;
    lineRenderer.SetPositions(keyFrameList.ToArray());
    }
}

public void resetAnim(){

   GameObject[] k = GameObject.FindGameObjectsWithTag ("Keyframe");
     
    for(var i = 0 ; i < k.Length ; i ++)
    {
        Destroy(k[i]);
    }
    keyFrameList.Clear();
    //lineRenderer.startColor = new Color(0f,255f,0f,0f);
    //lineRenderer.endColor = new Color(0f,255f,0f,0f);
}

public void toggleVisibility(){
    if (toggleVis == true){
        GameObject[] k = GameObject.FindGameObjectsWithTag ("Keyframe");
        
        for(var i = 0 ; i < k.Length ; i ++)
        {
            k[i].GetComponent<MeshRenderer>().enabled = false;
        }

        lineRenderer.startColor = new Color(0f,255f,0f,0f);
        lineRenderer.endColor = new Color(0f,255f,0f,0f);
        toggleVis = false;
    }
    else 
    {
        GameObject[] k = GameObject.FindGameObjectsWithTag ("Keyframe");
        
        for(var i = 0 ; i < k.Length ; i ++)
        {
            k[i].GetComponent<MeshRenderer>().enabled = true;
        }

        lineRenderer.startColor = new Color(0f,255f,0f,0.4f);
        lineRenderer.endColor = new Color(0f,255f,0f,0.4f);
        toggleVis = true;
    }
}

public void animToggleVoid(){
    animToggle = !animToggle;
}

public void addKeyFrame(){
    if(keyFrameList.Count > 1){
        GameObject sphere = sphereSpawn();
        int lastPos = keyFrameList.Count - 1;
        Vector3 spawnPos = keyFrameList[lastPos] + (keyFrameList[lastPos] - keyFrameList[lastPos - 1]);
        sphere.transform.position = spawnPos; //Set its position to the player
        keyFrameList.Add(sphere.transform.position); 
        keyFrameRotations.Add(rigi.transform.rotation);
        /*string result = "List contents: ";
        foreach (var keyframe in keyFrameRotations) //Printer coordinates pÃ¥ alle punkterne
            {
                result += keyframe.ToString() + ", ";
            }
        Debug.Log(result);   */
        counter ++;
        lineDrawer(); 
    }
}

public void keyFramePositionUpdate(Vector3 position, string keyFrameID){
    string numberStr = keyFrameID.Replace("Keyframe ", "");
    int keyFrameNumber = int.Parse(numberStr);
    keyFrameList[keyFrameNumber] = position;
    lineDrawer(); 
}

/*public void keyFrameSniper(string keyFrameID){
    string numberStr = keyFrameID.Replace("Keyframe ", "");
    int keyFrameNumber = int.Parse(numberStr);
    keyFrameList.RemoveAt(keyFrameNumber);
}
*/
    // Update is called once per frame
    void Update()
    {
        if(xrCustom.isGrabbed == true && animToggle == true){
            float dist = Vector3.Distance(transform.position, keyFramePosition);    //Calculate the distance between the player and the last sphere
            if (dist > keyFrameSpacing){
                keyFramePosition = rigi.transform.position;
                keyFrameSpawner();    
                lineDrawer();  
            }
        }
    }
}