using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationPlayer : MonoBehaviour
{

    public keyFrameGenerator keyFG;
    public XRCustomGrabInteractable XRCustom;
    private int nextFrame = 1;
    private bool firstTime = true;
    public float animSpeed {get; set;}
    public bool rotationTrack = false;
    public bool isLooping = true;

    // Start is called before the first frame update
    void Start(){
        XRCustom = gameObject.GetComponent<XRCustomGrabInteractable>();
        keyFG = gameObject.GetComponent<keyFrameGenerator>();
        animSpeed = 5f;
    }

    void Update()
    {
        if(XRCustom.isGrabbed == false && keyFG.keyFrameList.Count > 1){
            if(firstTime == true){
                gameObject.transform.position = keyFG.keyFrameList[0];
                if (rotationTrack){
                gameObject.transform.rotation = keyFG.keyFrameRotations[0];
                }
                firstTime = false;
            }
            playAnimation(keyFG.keyFrameList, keyFG.keyFrameRotations);
        }
    }
    

    void playAnimation(List <Vector3> frames, List <Quaternion> rotations){

    gameObject.transform.position = Vector3.MoveTowards(transform.position, frames[nextFrame], Time.deltaTime * animSpeed);
    if(rotationTrack){
    gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, rotations[nextFrame], Time.deltaTime * animSpeed * 100);
    }

    if(gameObject.transform.position == frames[nextFrame]){
        if(nextFrame + 1 >= frames.Count){
            if(isLooping == true){
            gameObject.transform.position = frames[0];
                if(rotationTrack){
                gameObject.transform.rotation = rotations[0];
                }
                nextFrame = 1;
            }
        }
        else {
            nextFrame++;
        }
    }
    //Debug.Log(gameObject.transform.position);
    }

    public void rotationToggleVoid(){
        rotationTrack = !rotationTrack;
    }

    public void loopingToggle(){
        isLooping = !isLooping;
    }
}
//hey virker det her push??