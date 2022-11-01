using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    public bool sniperMode = false;
    
    public void sniperToggle(){
        sniperMode = !sniperMode;
    }
    void Update(){
    }
}
