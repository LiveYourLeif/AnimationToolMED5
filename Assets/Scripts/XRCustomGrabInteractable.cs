using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// This is the same script as a "XR Grab Interactable", but it overrides certain methods, so the object does not snap to ray
// Only custom code is commentated here

public class XRCustomGrabInteractable : XRGrabInteractable
{
    private Vector3 interactorPosition = Vector3.zero;
    private Quaternion interactorRotation = Quaternion.identity;
    public bool isGrabbed = false; // Checks if the object is currently being grabbed or not
    
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        
        if (args.interactor is XRRayInteractor) 
        {
            interactorPosition = args.interactor.attachTransform.localPosition;
            interactorRotation = args.interactor.attachTransform.localRotation;

            bool hasAttach = attachTransform != null;
            args.interactor.attachTransform.position = hasAttach ? attachTransform.position : transform.position;
            args.interactor.attachTransform.rotation = hasAttach ? attachTransform.rotation : transform.rotation;
            isGrabbed = true; // If grabbed, set isGrabbed to true
        }
    }
    
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        
        if(args.interactor is XRRayInteractor)
        {
            interactorPosition = Vector3.zero;
            interactorRotation = Quaternion.identity;
            isGrabbed = false; // If released, set isGrabbed to false

            // If the object is released, spawn a keyframe at the position where the keyframe was let go
            if(this.gameObject.GetComponent<keyFrameGenerator>() != null && this.gameObject.GetComponent<keyFrameGenerator>().animToggle == true){
                this.gameObject.GetComponent<keyFrameGenerator>().keyFrameSpawner();
            }
        }
    } 
}