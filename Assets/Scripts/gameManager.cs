using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{

    // A GameManager that runs all the settings of the animation and transfers it to the animatable objects

    public bool sniperMode = false;
    public GameObject activeAnimatable;
    public TextMeshProUGUI objText;
    public TextMeshProUGUI delaySec;
    public TextMeshProUGUI durationText;
    GameObject[] animatables;
    GameObject[] keyFrames;
    public animationPlayer animPlayer;
    public keyFrameGenerator keyFG;

    public Toggle inGameAnimToggle;
    public Toggle inGameVisToggle;

    public Slider speedSlider;
    public Slider masterTimer;

    public TMP_Dropdown dropdown;

    public GameObject timelineMenu;
    public bool timelineVis = false;

    public bool editMode = true;
    public bool isWaiting = false;
    public float waitTime {get; set;}
    public float duration {get; set;}
    float countUp = 0;
    
    public float totalAnimationTime = 10f;

    public Vector3 timelineStartPos;

    
    
    void Start(){
        animatables = GameObject.FindGameObjectsWithTag ("Animatable");
        foreach (GameObject anim in animatables)
        {
            Debug.Log(anim);
        }
        activeAnimatable = animatables[0];
        keyFG = animatables[0].GetComponent<keyFrameGenerator>();
        animPlayer = animatables[0].GetComponent<animationPlayer>();
        changeActive(animatables[0]);
        keyFG.isActive = true;
        speedSlider.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
        speedSlider.value = 5f;
        waitTime = 2f;
        duration = 5f;
        timelineStartPos = timelineMenu.transform.localScale;
        timelineMenu.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        timelineMenu.SetActive(false);
    }

    // UI Elements
    
    public void sniperToggle(){
        sniperMode = !sniperMode;
    }

    public void toggleVisibility(){
        keyFG.toggleVisibility();
    }

    public void resetAnim(){
        keyFG = activeAnimatable.GetComponent<keyFrameGenerator>();
        animPlayer = activeAnimatable.GetComponent<animationPlayer>();
        keyFG.resetAnim();
        editMode = true;
        animPlayer.sliderScaler.transform.localScale = new Vector3(0,1,1);
    }

    public void resetAll(){
        foreach(GameObject anims in animatables){
            keyFG = anims.GetComponent<keyFrameGenerator>();
            animPlayer = anims.GetComponent<animationPlayer>();
            keyFG.resetAnim();
            editMode = true;
            animPlayer.sliderScaler.transform.localScale = new Vector3(0,1,1);
        }
    }

    public void animToggleVoid(){
        keyFG.animToggleVoid();
    }

    public void addKeyFrame(){
        keyFG.addKeyFrame();
    }

    public void showTimeline(){
        timelineVis = !timelineVis;
        if(timelineVis == true)
        {
            timelineMenu.transform.localScale = timelineStartPos;
            timelineMenu.SetActive(true);
        }
        else
        {
            timelineMenu.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            timelineMenu.SetActive(false);
        }
    }

    public void updateDurationNumber(){
        durationText.text = $"{System.Math.Round(duration,2)} s";
    }

    public void updateDelayNumber(){
        delaySec.text = $"{System.Math.Round(waitTime,2)} s";
    }

    public void setMasterTimer(){
        masterTimer.value = 0f;
        masterTimer.maxValue = duration;
        foreach(GameObject anims in animatables){
            animPlayer = anims.GetComponent<animationPlayer>();
            animPlayer.objectSlider.maxValue = duration;
        }
    }

    public void dropdownChange(){
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Animatable");
        changeActive(objs[dropdown.value]);
    }

    public void startAnim(){
        editMode = false;
        foreach(GameObject anims in animatables){
            keyFG = anims.GetComponent<keyFrameGenerator>();
            animPlayer = anims.GetComponent<animationPlayer>();
            if(keyFG.keyFrameList.Count > 1){
                anims.transform.position = keyFG.keyFrameList[0];
                animPlayer.nextFrame = 1;
            }
            animPlayer.animDone = false;
            animPlayer.pause = false;
        }
        animPlayer.pause = false;
        setMasterTimer();
    }

    /*public void checkDone(){
        bool allDone = true;
        foreach(GameObject anims in animatables){
            if(anims.GetComponent<animationPlayer>().animDone == false && anims.GetComponent<keyFrameGenerator>().keyFrameList.Count > 1)
            {
                allDone = false;
            }
        }
        if(allDone == true){
            isWaiting = true;
        }
    }*/

    public void pauseAll(){
        if(editMode == true){
            startAnim();
        }
        else{
            foreach(GameObject anims in animatables){
                anims.GetComponent<animationPlayer>().pauseToggleVoid();
            }      
        }
    }

    public void ValueChangeCheck()
	{
		animPlayer.animSpeed = speedSlider.value;
	}

    // Change Active Object

    public void changeActive(GameObject newActive){
        foreach(GameObject anims in animatables)
        {
            if(newActive == anims)
            {
                activeAnimatable = newActive;
                keyFG = anims.GetComponent<keyFrameGenerator>();
                animPlayer = anims.GetComponent<animationPlayer>();
                keyFG.isActive = true;
                objText.text = anims.name;
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
            sniperMode = false;
            speedSlider.value = animPlayer.animSpeed;

            for(int i = 0; i < animatables.Length; i++)
            {
                if(animatables[i] == activeAnimatable){
                    dropdown.value = i;
                }
            }

            inGameVisToggle.isOn = true;
            keyFG.toggleVis = true;
            keyFG.changeVisibility();


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

                // Animate
                if (keyFG.animToggle == false && inGameAnimToggle.isOn == true){
                    inGameAnimToggle.isOn = false;
                    keyFG.animToggle = false;
                } 
                else if(keyFG.animToggle == true && inGameAnimToggle.isOn == false){
                    inGameAnimToggle.isOn = true;
                    keyFG.animToggle = true;
                }
    }
             
    void Update(){
        if(isWaiting == true){
            countUp += Time.deltaTime;
            if(countUp < waitTime){
                //Debug.Log($"The waiting timer is at: {countUp}");
            }
            else if(countUp >= waitTime) {
                isWaiting = false;
                countUp = 0;
                startAnim();
            } 
        }
        if(editMode == false){
            if(animPlayer.pause == false){
            masterTimer.value += 1f* Time.deltaTime;
            }
        }
        else{
            masterTimer.value = 0;
        }

        if(masterTimer.value == masterTimer.maxValue)
        {
            isWaiting = true;
        }
    }
}
