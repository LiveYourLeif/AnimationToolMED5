using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class keyframeBehavior : MonoBehaviour
{
    public XRCustomGrabInteractable xrCustomGrab;
    public GameObject animatedObject;
    public bool sniperMode = false;

    void Start()
    {
        animatedObject = GameObject.Find("Cube");
        xrCustomGrab = gameObject.GetComponent<XRCustomGrabInteractable>();
    }

    void Update()
    {
        if(xrCustomGrab.isGrabbed == true)
        {
            /*if(sniperMode == true)
            {
                animatedObject.GetComponent<keyFrameGenerator>().keyFrameSniper(gameObject.name);
                Destroy(this);
                sniperMode = false;
            }
            else
            { */
            animatedObject.GetComponent<keyFrameGenerator>().keyFramePositionUpdate(this.transform.position, gameObject.name);
            //}
        }
    }
}
