#if INTEGRATIONS_STEAMVR

using System;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class SteamVRTrackableController : TrackableController
{
    [Serializable] public class UnityEvent : UnityEvent<SteamVRTrackableController>
    {

    }

    public UnityEvent OnGrabStarted = new UnityEvent();
    public UnityEvent OnGrabStopped = new UnityEvent();

    public Hand Controller;
    
    private bool _isGrabInProgress;

    void Update()
    {
        var isPressed = Controller.grabGripAction.GetState(Controller.handType);

        if (isPressed)
        {
            if (!_isGrabInProgress)
            {
                _isGrabInProgress = true;
                OnGrabStarted?.Invoke(this);
            }
        }
        else
        {
            if (_isGrabInProgress)
            {
                _isGrabInProgress = false;
                OnGrabStopped?.Invoke(this);
            }
        }
    }
}

#endif