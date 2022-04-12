#if INTEGRATIONS_XRTOOLKIT

using System;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class XRToolkitTrackableController : TrackableController
{
    [Serializable] public class UnityEvent : UnityEvent<XRToolkitTrackableController>
    {

    }

    public UnityEvent OnGrabStarted = new UnityEvent();
    public UnityEvent OnGrabStopped = new UnityEvent();

    public XRBaseController Controller;
    
    private bool _isGrabInProgress;

    void Update()
    {
        if (Controller.selectInteractionState.active)
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