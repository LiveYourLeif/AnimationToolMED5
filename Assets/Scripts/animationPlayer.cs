using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script attached to all animatable objects. Used to play / modify the animation.

public class animationPlayer : MonoBehaviour
{

    // Scripts from elsewhere in the scene
    public keyFrameGenerator keyFG;
    public XRCustomGrabInteractable XRCustom;
    public GameObject manager;
    public gameManager mng;

    // Sets the next frame in the animation
    public int nextFrame = 1;

    // Speed of the animation
    public float animSpeed;

    // Bool to check if the animation has finished playing
    public bool animDone = false;

    // Bool to check if the animation has been paused
    public bool pause = false;

    // The original scale of the object
    Vector3 startScale;

    // Related to the timeline
    public Slider objectSlider;     // The object's timeline slider, used to specify when the animation starts playing
    public GameObject sliderScaler; // Used to scale the blue indicator depending on the length of the animation
    public GameObject timelineCube; // The blue indicator - visually shows how long the objects animation is compared to the length of the entire animation


    void Start(){

        // Get scripts from other obecjts
        XRCustom = gameObject.GetComponent<XRCustomGrabInteractable>();
        keyFG = gameObject.GetComponent<keyFrameGenerator>();
        manager = GameObject.Find("GameManager");
        mng = manager.GetComponent<gameManager>();

        // Set animation speed and the scale of the object at the start
        animSpeed = 5f;
        startScale = gameObject.transform.localScale;
    }

    void Update()
    {

        // If an object is grabbed, enable "editMode" in gameManager
        if(XRCustom.isGrabbed)
        {
           mng.editMode = true;
        }

        // If "Eraser Mode" is active, hide the object by scaling it down so much that it can not be seen
        // This ensures that the animatable object does not obscure any keyframes from view
        if(mng.sniperMode){
            if(keyFG.keyFrameList.Count > 1){
                gameObject.transform.localScale = new Vector3(0.00001f, 0.00001f, 0.00001f);
                gameObject.transform.position = keyFG.keyFrameList[0];
                nextFrame = 1;
            }
        } 
        // Otherwise, set the object back to its normal scale
        else
        {
            gameObject.transform.localScale = startScale;
        }

        // If the object is not grabbed, and there is more than one keyframe, check if the animation should be playing
        if(XRCustom.isGrabbed == false && keyFG.keyFrameList.Count > 1){

            // The animation will only play if all conditions are met:
            // 1) Animation has not finished playing
            // 2) The player is not in edit mode (e.g. moving around objects and keyframes)
            // 3) The animation's starting time is less than or equal to the current timestamp of the master timer
            // 4) The animation is not currently paused
            // 5) The animation is not currently waiting to loop back to the start
            if(animDone == false && mng.editMode == false && objectSlider.value <= mng.masterTimer.value && pause == false && mng.isWaiting == false){
                // Play the animation
                playAnimation(keyFG.keyFrameList, keyFG.keyFrameRotations);
            }
            // Set the blue indicator in the timeline to be equal to the length of the animation
            sliderScaler.transform.localScale = new Vector3(calcDistance(),1,1);
        }
    } 

//calcDistance: Calculates the distance of the blue indicator in the timeline, compared to the length of the animation
    float calcDistance(){

        // Length of the entire animation
        float totalDistance = 0f;

        // The percentage of the full animation that the objects animation takes up
        // E.g. if the full animation is 4 seconds, and the objects animation is 2 seconds, scaleFactor would be 50
        // If scaleFactor is above 100, the objects animation is longer than the full animation, and will not fully play out
        float scaleFactor;

                // Find the length of the animation, by taking the distance between each keyframe 
                for(int i = 0; i < keyFG.keyFrameList.Count - 1; i++)
                {
                    totalDistance += Vector3.Distance(keyFG.keyFrameList[i], keyFG.keyFrameList[i+1]);
                }
        
        // Calculate the scaleFactor by comparing the duration of the objects animation with the duration of the full animation
        // Since time = distance / speed, we can use totalDistance/animSpeed to find the duration
        scaleFactor = (totalDistance / animSpeed) / mng.duration * 100f;

        // Check if the entirety of the objects animation can play out, or if it ends prematurely because the full animation is not long enough
        // objectSlide.value / objectSlider.maxValue * 100 will return the percentage of the full animation that has played before the objects animation begins
        // If this value plus scaleFactor is less than or equal 100, that means the entire animation will play out
        if(scaleFactor + objectSlider.value / objectSlider.maxValue * 100 <= 100)
        {
            // Set the color of the indicator to blue
            timelineCube.GetComponent<Renderer>().material.color = new Color32 (95,118,255,255);
            return scaleFactor;
        }

        // If the value is over 100, that means the entire animation is too long, and can not play out in full!
        else
        {
            // Set the color to red
            timelineCube.GetComponent<Renderer>().material.color = new Color (255,0,0);

            // Scale the size of the indicator to match the size of the animation
            return 100 - objectSlider.value / objectSlider.maxValue * 100;
        }
    }


    // playAnimation: Plays the animation
    void playAnimation(List <Vector3> frames, List <Quaternion> rotations){

        // Move the object towards the next keyframe
        gameObject.transform.position = Vector3.MoveTowards(transform.position, frames[nextFrame], Time.deltaTime * animSpeed);

        // The object has reached the position of the next keyframe
        if(gameObject.transform.position == frames[nextFrame]){

            // If that was the last keyframe, the animation is done playing
            if(nextFrame + 1 >= frames.Count){
                animDone = true;
                Debug.Log($"{gameObject.name} IS DONE");
            }

            // Else, move towards the next keyframe in the list
            else {
                nextFrame++;
            }
        }
    }

    //pauseToggleVoid: Toggles "Pause" On/Off
    public void pauseToggleVoid(){
    pause = !pause;
    }
}
