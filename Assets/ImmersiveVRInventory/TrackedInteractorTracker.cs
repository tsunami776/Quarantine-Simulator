using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TrackedInteractorTracker : MonoBehaviour
{
    [Serializable]
    public class UnityEvent : UnityEvent<List<TrackableController>>
    {

    }

    [SerializeField] private List<TrackableController> AvailableControllers;
    private List<TrackableController> CurrentlyCollidingControllers = new List<TrackableController>();

    public UnityEvent AllControllersCollisionStarts = new UnityEvent();
    public UnityEvent AllControllersCollisionEnds = new UnityEvent();

    void OnCollisionEnter(Collision collision)
    {
        var controller = collision.gameObject.GetComponent<TrackableController>();
        HandleCollisionEnter(controller);
    }

    void OnTriggerEnter(Collider collider)
    {
        var controller = collider.gameObject.GetComponent<TrackableController>();
        HandleCollisionEnter(controller);
    }

    private void HandleCollisionEnter(TrackableController controller)
    {
        if (!CurrentlyCollidingControllers.Contains(controller))
        {
            CurrentlyCollidingControllers.Add(controller);
            if (AvailableControllers.Any() && AvailableControllers.All(availableController =>
                CurrentlyCollidingControllers.Any(c => c == availableController)))
            {
                AllControllersCollisionStarts?.Invoke(CurrentlyCollidingControllers.ToList());
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        var controller = collision.gameObject.GetComponent<TrackableController>();
        HandleCollisionExit(controller);
    }
    
    void OnTriggerExit(Collider other)
    {
        var controller = other.gameObject.GetComponent<TrackableController>();
        HandleCollisionExit(controller);
    }

    private void HandleCollisionExit(TrackableController controller)
    {
        if (CurrentlyCollidingControllers.Contains(controller))
        {
            CurrentlyCollidingControllers.Remove(controller);
            AllControllersCollisionEnds?.Invoke(CurrentlyCollidingControllers.ToList());
        }
    }
}
