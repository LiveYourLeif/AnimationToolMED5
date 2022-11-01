using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class gameManager : MonoBehaviour
{

    public bool sniperMode = false;
    
    public void sniperToggle(){
        sniperMode = !sniperMode;
    }
    void Update(){
    }
}
