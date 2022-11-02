using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class gameManager : MonoBehaviour
{

    // A GameManager that runs all the settings of the animation and transfers it to the animatable objects

    public bool sniperMode = false;
    GameObject[] animatables;
    public animationPlayer animPlayer;
    public keyFrameGenerator keyFG;
    public float animationSpeed {get; set;}
    public float KFSpacing {get; set;}
    
    void Start(){
        animatables = GameObject.FindGameObjectsWithTag ("Animatable");
        keyFG = animatables[0].GetComponent<keyFrameGenerator>();
        animPlayer = animatables[0].GetComponent<animationPlayer>();
        animationSpeed = 5f;
        KFSpacing = 0.5f;
    }

    // UI Elements
    
    public void sniperToggle(){
        sniperMode = !sniperMode;
    }

    public void loopingToggle(){
        animPlayer.loopingToggle();
    }

    public void pauseToggleVoid(){
        animPlayer.pauseToggleVoid();
    }

    public void toggleVisibility(){
        keyFG.toggleVisibility();
    }

    public void resetAnim(){
        keyFG.resetAnim();
    }

    public void animToggleVoid(){
        keyFG.animToggleVoid();
    }

    public void addKeyFrame(){
        keyFG.addKeyFrame();
    }

    
    void Update(){

    }
}
