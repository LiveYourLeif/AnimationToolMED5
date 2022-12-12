using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.SceneManagement;

    // GameManager runs all the settings of the animation, including all UI elements, and transfers it to the animatable objects

public class gameManager : MonoBehaviour
{
    // The current active object
    public GameObject activeAnimatable;

    // Values on the UI that are updated throughout the runtime
    public TextMeshProUGUI objText;
    public TextMeshProUGUI delaySec;
    public TextMeshProUGUI durationText;

    // Arrays of all animatable objects and all keyframes
    GameObject[] animatables;
    GameObject[] keyFrames;

    // Scripts to access from animitable objects
    public animationPlayer animPlayer;
    public keyFrameGenerator keyFG;

    // "Animating" and "Visibility" toggles
    public Toggle inGameAnimToggle;
    public Toggle inGameVisToggle;

    // Slider for the animation speed
    public Slider speedSlider;

    // The master timer in the timeline
    public Slider masterTimer;

    // Dropdown menu (Unused in the final version)
    public TMP_Dropdown dropdown;

    // The entire timeline menu
    public GameObject timelineMenu;

    // Various variables for UI elements
    public bool sniperMode = false;        // Eraser Mode
    public bool timelineVis = false;       // Show/Hide Timeline
    public bool editMode = true;           // Edit Mode
    public bool isWaiting = false;         // Waiting after animation finishes
    public float waitTime {get; set;}      // Time to wait before the animation loops
    public float duration {get; set;}      // Duration of the animation
    float countUp = 0;                     // Keeps track of the time while waiting after animation finishes

    // Scale of the timeline UI
    public Vector3 timelineStartScale;

    
    
    void Start(){
        // Get scripts from the current active object
        animatables = GameObject.FindGameObjectsWithTag ("Animatable");
        activeAnimatable = animatables[0];
        keyFG = animatables[0].GetComponent<keyFrameGenerator>();
        animPlayer = animatables[0].GetComponent<animationPlayer>();
        changeActive(animatables[0]);
        keyFG.isActive = true;

        // Set values for variables
        speedSlider.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
        speedSlider.value = 5f;
        waitTime = 2f;
        duration = 5f;

        // Keep track of the original scale of the timeline, then hide it by shrinking it down
        timelineStartScale = timelineMenu.transform.localScale;
        timelineMenu.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        timelineMenu.SetActive(false);
    }


    // UI ELEMENTS
    
    // Toggle "Eraser Mode" On/Off
    public void sniperToggle(){
        sniperMode = !sniperMode;
    }

    // Toggle Visibility On/Off
    public void toggleVisibility(){
        keyFG.toggleVisibility();
    }

    // Delete all keyframes for one object
    public void resetAnim(){

        // Run resetAnim(), enable editmode and reset the blue timeline indicator
        keyFG = activeAnimatable.GetComponent<keyFrameGenerator>();
        animPlayer = activeAnimatable.GetComponent<animationPlayer>();
        keyFG.resetAnim();
        editMode = true;
        animPlayer.sliderScaler.transform.localScale = new Vector3(0,1,1);
    }
    
    // Delete all keyframes for all objects
    public void resetAll(){

        // The same as resetAnim(), but using a foreach loop to go through each animatable object
        foreach(GameObject anims in animatables){
            keyFG = anims.GetComponent<keyFrameGenerator>();
            animPlayer = anims.GetComponent<animationPlayer>();
            keyFG.resetAnim();
            editMode = true;
            animPlayer.sliderScaler.transform.localScale = new Vector3(0,1,1);
        }
    }

    // Toggles "Animating" On/Off
    public void animToggleVoid(){
        keyFG.animToggleVoid();
    }

    // Manually adds one keyframe
    public void addKeyFrame(){
        keyFG.addKeyFrame();
    }

    // Show timeline
    public void showTimeline(){
        timelineVis = !timelineVis;

        // Set the scale of the timeline to normal if toggled, else, shrink it down
        if(timelineVis == true)
        {
            timelineMenu.transform.localScale = timelineStartScale;
            timelineMenu.SetActive(true);
        }
        else
        {
            timelineMenu.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            timelineMenu.SetActive(false);
        }
    }

    // Update the "Animation Duration" number on the UI, rounded to two decimals
    public void updateDurationNumber(){
        durationText.text = $"{System.Math.Round(duration,2)} s";
    }

    // Update the "Loop Delay" number on the UI, rounded to two decimals
    public void updateDelayNumber(){
        delaySec.text = $"{System.Math.Round(waitTime,2)} s";
    }

    // Set the master timer's max value equal to the duration of the animation
    public void setMasterTimer(){
        masterTimer.value = 0f;
        masterTimer.maxValue = duration;

        // For each object, set their timeline slider's max value to the same as the master timer
        foreach(GameObject anims in animatables){
            animPlayer = anims.GetComponent<animationPlayer>();
            animPlayer.objectSlider.maxValue = duration;
        }
    }

    // Dropdown menu for change active object (Not in the final version)
    public void dropdownChange(){
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Animatable");
        changeActive(objs[dropdown.value]);
    }

    // Starts the animation for each object
    public void startAnim(){
        editMode = false;

        // For each object, set their position to the first keyframe, and their target to the second keyframe
        foreach(GameObject anims in animatables){
            keyFG = anims.GetComponent<keyFrameGenerator>();
            animPlayer = anims.GetComponent<animationPlayer>();
            if(keyFG.keyFrameList.Count > 1){
                anims.transform.position = keyFG.keyFrameList[0];
                animPlayer.nextFrame = 1;
            }

            // Set animDone and pause to false
            animPlayer.animDone = false;
            animPlayer.pause = false;
        }

        // Setting pause to false again, as a failsafe against OnValueChanged
        animPlayer.pause = false;

        // Set the master timer
        setMasterTimer();
    }

    // Play/Pause button, originally just a pause button, hence the name
    public void pauseAll(){

        // If editMode is current true, start the animation
        if(editMode == true){
            startAnim();
        }
        
        // Else, pause the animation of all objects
        else
        {
            foreach(GameObject anims in animatables){
                anims.GetComponent<animationPlayer>().pauseToggleVoid();
            }      
        }
    }

    // Set the animation speed to match the value of the slider
    public void ValueChangeCheck()
	{
		animPlayer.animSpeed = speedSlider.value;
	}

    // Change the current active object
    public void changeActive(GameObject newActive){

        // First, run through every animatable object in the scene
        foreach(GameObject anims in animatables)
        {

            // This is the object turning active
            if(newActive == anims)
            {
                // Set it as active
                activeAnimatable = newActive;
                keyFG = anims.GetComponent<keyFrameGenerator>();
                animPlayer = anims.GetComponent<animationPlayer>();
                keyFG.isActive = true;

                // Change the active object listed on the UI
                objText.text = anims.name;

                // Turn it red (Used in testScene)
                anims.GetComponent<Renderer>().material.color = new Color(255f, 0f, 0f);
            }

            // This is NOT the object turning active
            else
            {
                // Set it as not active
                keyFG = anims.GetComponent<keyFrameGenerator>();
                animPlayer = anims.GetComponent<animationPlayer>();
                keyFG.isActive = false;

                // Turn it grey (Used in testScene)
                anims.GetComponent<Renderer>().material.color = Color.grey;
            }
        }
            // After the loop ends, set the scripts of the new active object
            keyFG = newActive.GetComponent<keyFrameGenerator>();
            animPlayer = newActive.GetComponent<animationPlayer>();
            speedSlider.value = animPlayer.animSpeed;

            // Toggle the visibility on
            inGameVisToggle.isOn = true;
            keyFG.toggleVis = true;
            keyFG.changeVisibility();

            // Set Eraser Mode to false
            sniperMode = false;

            // Turn all the active objects keyframes red, and all non-active objects keyframes grey
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

                // Make sure the "Animate" stays consistent between objects
                // (E.g. if you turn Animate off for an object, and then make the active
                // object one with Animate turned on, the UI should update accordingly)
                if (keyFG.animToggle == false && inGameAnimToggle.isOn == true){
                    inGameAnimToggle.isOn = false;
                    keyFG.animToggle = false;
                } 
                else if(keyFG.animToggle == true && inGameAnimToggle.isOn == false){
                    inGameAnimToggle.isOn = true;
                    keyFG.animToggle = true;
                }
    }

    // Update is mostly used to keep track of time when the animation is waiting to loop   
    void Update(){

        // Animation has been played, and program is waiting to loop
        if(isWaiting == true){

            // Increase countUp by 1 every second
            countUp += Time.deltaTime;

            // If countUp is greater than or equal to waitTime, start the animation again
            if(countUp >= waitTime) {
                isWaiting = false;
                countUp = 0;
                startAnim();
            } 
        }

        // If editMode is false, and the animation is not paused, increase the master timer by 1 every second
        if(editMode == false){
            if(animPlayer.pause == false){
            masterTimer.value += 1f* Time.deltaTime;
            }
        }
        
        // Else, set it at 0
        else{
            masterTimer.value = 0;
        }

        // If the master timer has reached its max value, the animation is done playing, and the delay until the animation loop begins
        if(masterTimer.value == masterTimer.maxValue)
        {
            isWaiting = true;
        }
    }
}
