using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// This is the same script as a "XR Grab Interactable", but it overrides certain methods, so the object does not snap to ray

public class XRCustomGrabInteractable : XRGrabInteractable
{
    private Vector3 interactorPosition = Vector3.zero;
    private Quaternion interactorRotation = Quaternion.identity;
    public bool isGrabbed = false;
    
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
            isGrabbed = true;
        }
    }
    
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        
        if(args.interactor is XRRayInteractor)
        {
            interactorPosition = Vector3.zero;
            interactorRotation = Quaternion.identity;
            isGrabbed = false;
            this.gameObject.GetComponent<keyFrameGenerator>().keyFrameSpawner();
        }
    } 
}