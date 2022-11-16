using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class animationPlayer : MonoBehaviour
{

    public keyFrameGenerator keyFG;
    public XRCustomGrabInteractable XRCustom;
    public GameObject manager;
    public gameManager mng;
    public int nextFrame = 1;
    private bool firstTime = true;
    public float animSpeed;
    public bool animDone = false;
    public bool pause = false;
    Vector3 startScale;

    public Slider objectSlider;
    public GameObject sliderScaler;

    // Start is called before the first frame update
    void Start(){
        XRCustom = gameObject.GetComponent<XRCustomGrabInteractable>();
        keyFG = gameObject.GetComponent<keyFrameGenerator>();
        manager = GameObject.Find("GameManager");
        mng = manager.GetComponent<gameManager>();
        animSpeed = 5f;
        startScale = gameObject.transform.localScale;
    }

    void Update()
    {
        if(XRCustom.isGrabbed)
        {
           mng.editMode = true;
        }
        if(mng.sniperMode){
            if(keyFG.keyFrameList.Count > 1){
                gameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                gameObject.transform.position = keyFG.keyFrameList[0];
                nextFrame = 1;
            }
        } 
        else
        {
            gameObject.transform.localScale = startScale;
        }
           if(XRCustom.isGrabbed == false && keyFG.keyFrameList.Count > 1){
               // if(firstTime == true){
                  //  gameObject.transform.position = keyFG.keyFrameList[0];
                    //if (rotationTrack){
                    //gameObject.transform.rotation = keyFG.keyFrameRotations[0];
                    //}
                    //firstTime = false;
                //}
                if(animDone == false && mng.editMode == false && objectSlider.value <= mng.masterTimer.value && pause == false){
                playAnimation(keyFG.keyFrameList, keyFG.keyFrameRotations);
                }
                sliderScaler.transform.localScale = new Vector3(calcDistance(),1,1);

            }
    } 

    float calcDistance(){
        float totalDistance = 0f;
        float scaleFactor;
                for(int i = 0; i < keyFG.keyFrameList.Count - 1; i++)
                {
                totalDistance += Vector3.Distance(keyFG.keyFrameList[i], keyFG.keyFrameList[i+1]);
                }
        scaleFactor = (totalDistance / animSpeed) / 10f * 100f;
        Debug.Log($"animation time: {mng.totalAnimationTime}");
        Debug.Log($"distance: {totalDistance}");
        Debug.Log($"speed: {animSpeed}");
        Debug.Log($"scale factor:{scaleFactor}");
        return scaleFactor;
    }

    void playAnimation(List <Vector3> frames, List <Quaternion> rotations){
    gameObject.transform.position = Vector3.MoveTowards(transform.position, frames[nextFrame], Time.deltaTime * animSpeed);
    //if(rotationTrack){
    //gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, rotations[nextFrame], Time.deltaTime * animSpeed * 100);
    //}

    if(gameObject.transform.position == frames[nextFrame]){
        if(nextFrame + 1 >= frames.Count){
            animDone = true;
            Debug.Log($"{gameObject.name} IS DONE");
            mng.checkDone();
        }
        else {
            nextFrame++;
        }
    }
    //Debug.Log(gameObject.transform.position);
    }

    public void pauseToggleVoid(){
    pause = !pause;
    }
}
//hey virker det her push??