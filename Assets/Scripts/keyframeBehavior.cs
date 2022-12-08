using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class keyframeBehavior : MonoBehaviour
{
    public XRCustomGrabInteractable xrCustomGrab;
    public GameObject manager;
    public Rigidbody rb;
    GameObject[] animatables;
    public GameObject activeObj;
    public gameManager mng;
    public bool sniperMode = false;

    void Start()
    {
        animatables = GameObject.FindGameObjectsWithTag ("Animatable");
        activeObj = animatables[0];
        manager = GameObject.Find("GameManager");
        xrCustomGrab = gameObject.GetComponent<XRCustomGrabInteractable>();
        mng = manager.GetComponent<gameManager>();
    }

    void Update()
    {
        activeObj = mng.activeAnimatable;
        if(gameObject.name.Contains(activeObj.name)){
            xrCustomGrab.enabled = true;
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
        else
        {
           xrCustomGrab.enabled = false;
        }
    }

    public void sniperToggle(){
        sniperMode = !sniperMode;
    }
}
