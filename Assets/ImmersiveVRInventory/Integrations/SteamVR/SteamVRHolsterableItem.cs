#if INTEGRATIONS_STEAMVR

using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Throwable))]
public class SteamVRHolsterableItem : HolsterableItem
{
    private Interactable Interactable;

    protected override void Awake()
    {
        base.Awake();

        Interactable = GetComponent<Interactable>();
        if (Interactable == null)
            Debug.LogError($"{nameof(Interactable)} needs to be on same game object.");
    }

    public override void Store(HolsterSlot slot)
    {
        base.Store(slot);

        Interactable.hoveringHand?.DetachObject(Interactable.gameObject);
    }


    public override void TakeOut(TrackableController controller)
    {
        base.TakeOut(controller);

        var steamVrTrackableController = controller as SteamVRTrackableController;
        if (steamVrTrackableController == null)
        {
            throw  new Exception("Controller needs to be of SteamVR type");
        }

        steamVrTrackableController.Controller.AttachObject(Interactable.gameObject, GrabTypes.Grip);
    }
}
#endif