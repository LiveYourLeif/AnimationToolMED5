using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changePicture : MonoBehaviour
{

    public Texture on;
    public Texture off;
    public bool currentState = true;
    public GameObject manager;
    public gameManager mng;

    void Start(){
        mng = manager.GetComponent<gameManager>();
    }

    public void changeSprite(){
        currentState = !currentState;
        if(currentState == true)
        {
            this.gameObject.GetComponent<RawImage>().texture = on;
        }
        else
        {
            this.gameObject.GetComponent<RawImage>().texture = off;
        }

    }


    void Update()
    {
        if (mng.editMode == true){
            this.gameObject.GetComponent<RawImage>().texture = on;
            currentState = true;
        }
    }
}
