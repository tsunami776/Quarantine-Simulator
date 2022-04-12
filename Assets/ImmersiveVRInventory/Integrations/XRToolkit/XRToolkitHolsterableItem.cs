#if INTEGRATIONS_XRTOOLKIT

using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class XRToolkitHolsterableItem : HolsterableItem
{
    private static MethodInfo _deregisterInteractableFn;
//at version 0.10 - unregister / register methods became public - leaving as reflection for backward compatibility with older version (private) methods
    private static MethodInfo DeregisterInteractableFn => _deregisterInteractableFn ?? (_deregisterInteractableFn = typeof(XRInteractionManager)
        .GetMethod("UnregisterInteractable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

    private static MethodInfo _registerInteractableFn;
    private static MethodInfo RegisterInteractableFn => _registerInteractableFn ?? (_registerInteractableFn = typeof(XRInteractionManager)
        .GetMethod("RegisterInteractable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

    private static MethodInfo _forceSelectFn;
    private static MethodInfo ForceSelectFn => _forceSelectFn ?? (_forceSelectFn =
        typeof(XRInteractionManager)
            .GetMethod("ForceSelect", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
    
    private XRGrabInteractable XRGrabInteractable;

    protected override void Awake()
    {
        base.Awake();

        XRGrabInteractable = GetComponent<XRGrabInteractable>();
        if (XRGrabInteractable == null)
            Debug.LogError($"{nameof(XRGrabInteractable)} needs to be on same game object.");
    }

    public override void Store(HolsterSlot slot)
    {
        base.Store(slot);

        DeregisterInteractableFn.Invoke(XRGrabInteractable.interactionManager, new object[] { XRGrabInteractable });
    }


    public override void TakeOut(TrackableController controller)
    {
        base.TakeOut(controller);

        RegisterInteractableFn.Invoke(XRGrabInteractable.interactionManager, new object[] { XRGrabInteractable });

        var interactor = controller.GetComponent<XRBaseControllerInteractor>(); 
        ForceSelectFn.Invoke(XRGrabInteractable.interactionManager, new object[] {interactor, XRGrabInteractable});
    }

}

#endif