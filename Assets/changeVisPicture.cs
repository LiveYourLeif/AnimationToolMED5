using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeVisPicture : MonoBehaviour
{
    public Texture on;
    public Texture off;
    public bool currentState = true;

    void Start(){
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

    }
}
