using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class gameManager : MonoBehaviour
{

    // A GameManager that runs all the settings of the animation and transfers it to the animatable objects

    public bool sniperMode = false;
    public GameObject activeAnimatable;
    public TextMeshProUGUI objText;
    public TextMeshProUGUI delaySec;
    GameObject[] animatables;
    GameObject[] keyFrames;
    public animationPlayer animPlayer;
    public keyFrameGenerator keyFG;

    public Toggle inGameLoopToggle;
    public Toggle inGameAnimToggle;
    public Toggle inGamePauseToggle;
    public Toggle inGameVisToggle;

    public Slider speedSlider;
    public Slider masterTimer;

    public bool editMode = true;
    bool isWaiting = false;
    public float waitTime {get; set;}
    float countUp = 0;

    
    
    void Start(){
        animatables = GameObject.FindGameObjectsWithTag ("Animatable");
        activeAnimatable = animatables[0];
        keyFG = animatables[0].GetComponent<keyFrameGenerator>();
        animPlayer = animatables[0].GetComponent<animationPlayer>();
        keyFG.isActive = true;
        speedSlider.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
        waitTime = 2f;
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
        editMode = true;
    }

    public void animToggleVoid(){
        keyFG.animToggleVoid();
    }

    public void addKeyFrame(){
        keyFG.addKeyFrame();
    }

    public void pauseAnimation(){
        animPlayer.pauseToggleVoid();
    }

    public void updateDelayNumber(){
        delaySec.text = $"{System.Math.Round(waitTime,2)} s";
    }

    public void setMasterTimer(float totalAnimationTime){
        masterTimer.value = 0f;
        masterTimer.maxValue = totalAnimationTime;
    }

    public void resetAll(){
        foreach(GameObject anims in animatables){
            keyFG = anims.GetComponent<keyFrameGenerator>();
            keyFG.resetAnim();
        }
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
        inGamePauseToggle.isOn = false;
        animPlayer.pause = false;
        float longestDistance = 0;
        foreach(GameObject anims in animatables){
        keyFG = anims.GetComponent<keyFrameGenerator>();
        float totalDistance = 0;
            for(int i = 0; i < keyFG.keyFrameList.Count - 1; i++)
            {
                totalDistance += Vector3.Distance(keyFG.keyFrameList[i], keyFG.keyFrameList[i+1]);
            }
        if (totalDistance > longestDistance){
            longestDistance = totalDistance;
        }
        }
        float totalAnimationTime;
        totalAnimationTime = longestDistance / animPlayer.animSpeed + waitTime;
        setMasterTimer(totalAnimationTime);
    }

    public void checkDone(){
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
    }

    public void pauseAll(){
        foreach(GameObject anims in animatables){
            anims.GetComponent<animationPlayer>().pause = true;
        }
        inGamePauseToggle.isOn = true;
        animPlayer.pause = true;
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
                
                // Looping
                if (animPlayer.isLooping == false && inGameLoopToggle.isOn == true){
                    inGameLoopToggle.isOn = false;
                    animPlayer.isLooping = false;
                } 
                else if(animPlayer.isLooping == true && inGameLoopToggle.isOn == false){
                    inGameLoopToggle.isOn = true;
                    animPlayer.isLooping = true;
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

                // Pause
                if (animPlayer.pause == false && inGamePauseToggle.isOn == true){
                    inGamePauseToggle.isOn= false;
                    animPlayer.pause = false;
                } 
                else if(animPlayer.pause == true && inGamePauseToggle.isOn== false){
                    inGamePauseToggle.isOn = true;
                    animPlayer.pause = true;
                }
    }
             
    void Update(){
        if(isWaiting == true){
            countUp += Time.deltaTime;
            if(countUp < waitTime){
                Debug.Log($"The waiting timer is at: {countUp * Time.deltaTime}");
            }
            else if(countUp >= waitTime) {
                isWaiting = false;
                countUp = 0;
                startAnim();
            } 
        }
        masterTimer.value += 1f* Time.deltaTime;
    }
}
