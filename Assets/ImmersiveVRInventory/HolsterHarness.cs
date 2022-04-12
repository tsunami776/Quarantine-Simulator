using System.Collections.Generic;
using System.Linq;
using ReliableSolutions.Unity.Common;
using UnityEngine;

public class HolsterHarness : MonoBehaviour
{
    [SerializeField] private FollowObjectTarget FollowObjectTarget;
    [SerializeField] private bool IsAdjustmentButtonVisible;
    [SerializeField] private bool IsObjectFollowerDisabledWhenAdjusting = false;

    [SerializeField] private TrackedInteractorTracker AdjustmentModeOn;
    [SerializeField] private TrackedInteractorTracker AdjustmentModeOff;

    private bool _isAdjustmentInProgress;

    private List<HolsterGrabHandle> _allAdjustableHandles;

    void Start()
    {
        FollowObjectTarget.enabled = true;
        if (IsAdjustmentButtonVisible)
        {
            AdjustmentModeOn.gameObject.SetActive(true);
        }

        _allAdjustableHandles = GetComponentsInChildren<HolsterGrabHandle>(includeInactive: true).ToList();
    }

    public void StartRuntimeAdjusting()
    {
        _allAdjustableHandles.ForEach(h => h.gameObject.SetActive(true));
        if(IsObjectFollowerDisabledWhenAdjusting) FollowObjectTarget.enabled = false;

        AdjustmentModeOn.gameObject.SetActive(false);
        AdjustmentModeOff.gameObject.SetActive(true);

        _isAdjustmentInProgress = true;
    }

    public void StopRuntimeAdjusting()
    {
        _allAdjustableHandles.ForEach(h => h.gameObject.SetActive(false));
        if (IsObjectFollowerDisabledWhenAdjusting) FollowObjectTarget.enabled = true;

        AdjustmentModeOn.gameObject.SetActive(true);
        AdjustmentModeOff.gameObject.SetActive(false);

        foreach (var grabHandle in _allAdjustableHandles)
        {
            grabHandle.gameObject.GetComponentInParent<HolsterSlotContainer>().TryCaptureSlotContainerData();
        }

        _isAdjustmentInProgress = true;
    }
}
