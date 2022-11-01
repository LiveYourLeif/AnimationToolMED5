using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class keyframeBehavior : MonoBehaviour
{
    public XRCustomGrabInteractable xrCustomGrab;
    public GameObject animatedObject;
    public GameObject manager;
    public gameManager mng;
    public bool sniperMode = false;

    void Start()
    {
        animatedObject = GameObject.Find("Cube");
        manager = GameObject.Find("GameManager");
        xrCustomGrab = gameObject.GetComponent<XRCustomGrabInteractable>();
        mng = manager.GetComponent<gameManager>();
    }

    void Update()
    {
        if(xrCustomGrab.isGrabbed == true)
        {
            if(mng.sniperMode == true)
            {
                Destroy(gameObject);
                animatedObject.GetComponent<keyFrameGenerator>().keyFrameSniper(gameObject.name);
            } else
            { 
                animatedObject.GetComponent<keyFrameGenerator>().keyFramePositionUpdate(this.transform.position, gameObject.name);
            }
        }
    }

    public void sniperToggle(){
        sniperMode = !sniperMode;
    }
}
