using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class keyframeBehavior : MonoBehaviour
{
    public XRCustomGrabInteractable xrCustomGrab;
    public GameObject manager;
    public GameObject[] animatedObjects;
    public GameObject activeObj;
    public gameManager mng;
    public bool sniperMode = false;

    void Start()
    {
        activeObj = GameObject.FindWithTag("Animatable");
        manager = GameObject.Find("GameManager");
        xrCustomGrab = gameObject.GetComponent<XRCustomGrabInteractable>();
        mng = manager.GetComponent<gameManager>();
    }

    void Update()
    {
        if(xrCustomGrab.isGrabbed == true)
        {
            if(mng.sniperMode == true && activeObj.GetComponent<keyFrameGenerator>().keyFrameList.Count > 2)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                activeObj.GetComponent<keyFrameGenerator>().keyFrameSniper(gameObject.name);
            } else
            { 
                activeObj.GetComponent<keyFrameGenerator>().keyFramePositionUpdate(this.transform.position, gameObject.name);
            }
        }
    }

    public void sniperToggle(){
        sniperMode = !sniperMode;
    }
}
