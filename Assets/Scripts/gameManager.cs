using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class gameManager : MonoBehaviour
{

    // A GameManager that runs all the settings of the animation and transfers it to the animatable objects

    public bool sniperMode = false;
    public GameObject activeAnimatable;
    public TextMeshProUGUI objText;
    GameObject[] animatables;
    GameObject[] keyFrames;
    public animationPlayer animPlayer;
    public keyFrameGenerator keyFG;
    public float animationSpeed {get; set;}
    public float KFSpacing {get; set;}
    
    void Start(){
        animatables = GameObject.FindGameObjectsWithTag ("Animatable");
        activeAnimatable = animatables[0];
        keyFG = animatables[0].GetComponent<keyFrameGenerator>();
        animPlayer = animatables[0].GetComponent<animationPlayer>();
        keyFG.isActive = true;
        animationSpeed = 5f;
        KFSpacing = 0.5f;
        objText.text = $"SELECTED:\n" + animatables[0].name;
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

    public void changeActive(GameObject newActive){
        foreach(GameObject anims in animatables)
        {
            if(newActive == anims)
            {
                activeAnimatable = newActive;
                keyFG = anims.GetComponent<keyFrameGenerator>();
                animPlayer = anims.GetComponent<animationPlayer>();
                keyFG.isActive = true;
                objText.text = $"SELECTED:\n" + anims.name;
                anims.GetComponent<Renderer>().material.color = new Color(255f, 0f, 0f);
            }
            else
            {
                keyFG = anims.GetComponent<keyFrameGenerator>();
                animPlayer = anims.GetComponent<animationPlayer>();
                keyFG.isActive = false;
                anims.GetComponent<Renderer>().material.color = Color.grey;
            }
        }
            keyFG = newActive.GetComponent<keyFrameGenerator>();
            animPlayer = newActive.GetComponent<animationPlayer>();

            keyFrames = GameObject.FindGameObjectsWithTag("Keyframe");
            
            for (var i = 0; i < keyFrames.Length; i++){
                if(keyFrames[i].name.Contains(newActive.name))
                    {
                        keyFrames[i].GetComponent<Renderer>().material.color = new Color(255f, 0f, 0f);
                    }
                    else 
                    {
                        keyFrames[i].GetComponent<Renderer>().material.color = Color.grey;
                    }
                }
             }
    void Update(){
    }
}
