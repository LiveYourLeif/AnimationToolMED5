using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Script attached to every keyframe. 

public class keyframeBehavior : MonoBehaviour
{
    // Scripts from elsewhere in the scene
    public XRCustomGrabInteractable xrCustomGrab;
    public GameObject manager;
    public gameManager mng;

    // Rigidbody
    public Rigidbody rb;

    // List of all animatable objects in the scene
    GameObject[] animatables;

    // The current active object
    public GameObject activeObj;

    // Bool for "Eraser Mode" (formerly "Sniper Mode")
    public bool sniperMode = false;

    void Start()
    {
        // Set variables and other scripts to reference
        animatables = GameObject.FindGameObjectsWithTag ("Animatable");
        activeObj = animatables[0];
        manager = GameObject.Find("GameManager");
        xrCustomGrab = gameObject.GetComponent<XRCustomGrabInteractable>();
        mng = manager.GetComponent<gameManager>();
    }

    void Update()
    {

        // Use the gameManager to find the current active object
        activeObj = mng.activeAnimatable;

        // Make sure only keyframes of the current active object are grabbable
        if(gameObject.name.Contains(activeObj.name)){
            xrCustomGrab.enabled = true;

            // The keyframe is grabbed
            if(xrCustomGrab.isGrabbed == true)
            {
                // If "Eraser Mode" is active and there's more than 2 keyframes, delete it and run "keyFrameSniper" in keyFrameGenerator
                if(mng.sniperMode == true && activeObj.GetComponent<keyFrameGenerator>().keyFrameList.Count > 2)
                {
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                    activeObj.GetComponent<keyFrameGenerator>().keyFrameSniper(gameObject.name);
                } 
                // If the keyframe is simply moved, update its position in keyFrameGenerator
                else
                { 
                    activeObj.GetComponent<keyFrameGenerator>().keyFramePositionUpdate(this.transform.position, gameObject.name);
                }
            }
        }

        // Make sure keyframes of non-active objects can not be grabbed
        else
        {
           xrCustomGrab.enabled = false;
        }
    }

    // Toggle "Eraser Mode" On/Off
    public void sniperToggle(){
        sniperMode = !sniperMode;
    }
}
