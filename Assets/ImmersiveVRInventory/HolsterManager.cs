using System.Collections;
using System.Linq;
using Assets.Backpack.GeneralUtilities;
using ReliableSolutions.Unity.Common.Utilities;
using UnityEditor;
using UnityEngine;

public class HolsterManager : MonoBehaviour
{
    void Start()
    {
        ExecuteStoreAutoHolsterableItems();
    }

    [ContextMenu("Store Auto Holsterable Items")]
    private void ExecuteStoreAutoHolsterableItems()
    {
        StartCoroutine(StoreAutoHolsterableItems());
    }

    private IEnumerator StoreAutoHolsterableItems()
    {
        yield return new WaitForSeconds(1);

        foreach (var autoHolsterableItem in FindObjectsOfType<HolsterableItem>()
            .Where(h => h.AutoHolsterSlotOnUngrab != null))
        {
            StoreInSlot(autoHolsterableItem.AutoHolsterSlotOnUngrab, autoHolsterableItem);
        }
    }

    public void HandleGrab(TrackableController controller)
    {
        var collidingWithHandle = controller.CollidingWithHandle;
        if (collidingWithHandle != null)
        {
            controller.StartHoldingHandle(collidingWithHandle);
        }
        else if (controller.CollidingWithSlot != null && controller.CollidingWithSlot.CurrentlyStoredItem != null)
        {
            var slot = controller.CollidingWithSlot;
            var storedItem = slot.CurrentlyStoredItem;

            if (storedItem != null)
            {
                slot.TakeOut(controller);
                SetItemColliders(storedItem.gameObject, true);
                controller.CollidingWithItem = storedItem;
                storedItem.MoveRoutine?.ForceStop();

                var holsterHandle = storedItem.GetComponentInChildren<HolsterGrabHandle>();
                if (holsterHandle != null)
                {
                    controller.StartHoldingHandle(holsterHandle);
                    SetItemColliders(holsterHandle.HolsterItem.gameObject, true);
                }

                if (slot.ScaleItemsToFit)
                {
                    storedItem.ScalingRoutine?.ForceStop();

                    var scaleCoroutine = ScaleSmoothMutator.Scale(storedItem.transform, storedItem.ScaleBeforeStoring, slot.HolsterSlotContainer.MoveItemToStoreOverSeconds);
                    scaleCoroutine.Finished += (s, e) => { storedItem.ScalingRoutine = null; };
                    scaleCoroutine.BeforeYieldReturn += (s, e) =>
                    {
                        if (holsterHandle != null) controller.UpdateFollowerOffset(holsterHandle);
                    };
                    storedItem.ScalingRoutine = scaleCoroutine;
                    scaleCoroutine.Start(StartCoroutine);
                }
            }
        }
    }

    public void HandleUngrab(TrackableController controller)
    {
        var slot = controller.CollidingWithSlot;
        var collidingWithItem = controller.CollidingWithItem;
        var holdingHandle = controller.HolidingHandle;
        if (holdingHandle != null)
        {
            controller.StopHoldingHandle(holdingHandle);
            var holsterItem = holdingHandle.HolsterItem;

            if (holsterItem != null)
            {
                if (holsterItem.AutoHolsterSlotOnUngrab != null)
                {
                    StoreHolster(controller, holsterItem.AutoHolsterSlotOnUngrab, holsterItem, holdingHandle);
                }
                else if (slot != null && slot.CurrentlyStoredItem == null)
                {
                    StoreHolster(controller, slot, holsterItem, holdingHandle);
                }
            }

        }
        else if (collidingWithItem != null)
        {
            if (slot != null && slot.CurrentlyStoredItem == null)
            {
                StoreInSlot(slot, collidingWithItem);
                controller.CollidingWithItem = null;
            }
            else if (collidingWithItem.AutoHolsterSlotOnUngrab != null)
            {
                StoreInSlot(collidingWithItem.AutoHolsterSlotOnUngrab, collidingWithItem);
            }
        }
    }

    private void StoreHolster(TrackableController controller, HolsterSlot slot, HolsterableItem holsterItem,
        HolsterGrabHandle holdingHandle)
    {
        StoreInSlot(slot, holsterItem);
        SetItemColliders(holdingHandle.HolsterItem.gameObject, false);
        controller.CollidingWithHandle = null; //need to set manually as it'll not fire once colliders are turned off
    }

    public void StoreInSlot(HolsterSlot slot, HolsterableItem item)
    {
        if (slot.Store(item))
        {
            SetItemColliders(item.gameObject, false);

            var meshBounds = item.MeshRenderer.bounds;
            if (meshBounds != default(Bounds))
            {
                var startScale = item.transform.localScale;
                var targetScale = item.transform.localScale;
                if (slot.ScaleItemsToFit)
                {
                    item.ScalingRoutine?.ForceStop();
                    var trackableCoroutine = ScaleSmoothMutator.ScaleToFit(
                        new ScaleToFitTarget(meshBounds, item.transform), 
                        slot.GetItemHoldingSphereBounds(), 
                        slot.HolsterSlotContainer.MoveItemToStoreOverSeconds,
                        out var newScale
                    );
                    targetScale = newScale;
                    
                    trackableCoroutine.Finished += (s, e) => { item.ScalingRoutine = null; };
                    item.ScalingRoutine = trackableCoroutine;
                    trackableCoroutine.Start(StartCoroutine);

                }

                var startRotation = item.transform.rotation;
                var targetRotation = RotationHelper.GetQuaternionRotationChildRelativeParentApplicable(
                      slot.AttachmentPoint.rotation,
                      item.transform.rotation,
                      item.AttachmentOrigin.rotation
                );
                StartRotateCoroutine(targetRotation, item, slot);

                //instantly set rotation / scale to target in order to get correct position adjustment
                item.transform.rotation = targetRotation;
                item.transform.localScale = targetScale;

                var slotAttachmentPositionAdjustedForItemOffset = GetSlotAttachmentPositionAdjustedForItemOffset(slot, item);

                item.transform.rotation = startRotation;
                item.transform.localScale = startScale;

                StartMoveCoroutine(slotAttachmentPositionAdjustedForItemOffset, slot, item);
            }
        }
    }

    private static Vector3 GetSlotAttachmentPositionAdjustedForItemOffset(HolsterSlot slot, HolsterableItem item)
    {
        return slot.AttachmentPoint.position + (item.transform.position - item.AttachmentOrigin.position);
    }

    private void StartMoveCoroutine(Vector3 target, HolsterSlot slot, HolsterableItem item)
    {
        item.MoveRoutine?.ForceStop();
        var moveCoroutine = TransformSmoothMover.MoveOverSeconds(item.transform, target, slot.HolsterSlotContainer.MoveItemToStoreOverSeconds);
        moveCoroutine.Finished += (s, e) =>
        {
            item.MoveRoutine = null;
            item.transform.position = GetSlotAttachmentPositionAdjustedForItemOffset(slot, item);
        };
        item.MoveRoutine = moveCoroutine;
        moveCoroutine.Start(StartCoroutine);
    }


    private void StartRotateCoroutine(Quaternion target, HolsterableItem storedItem, HolsterSlot slot)
    {
        storedItem.RotateRoutine?.ForceStop();
        var rotateCoroutine = TransformSmoothRotator.RotateOverSeconds(storedItem.transform, target, slot.HolsterSlotContainer.MoveItemToStoreOverSeconds);
        rotateCoroutine.Finished += (s, e) => { storedItem.RotateRoutine = null; };
        storedItem.RotateRoutine = rotateCoroutine;
        rotateCoroutine.Start(StartCoroutine);
    }

    private static void SetItemColliders(GameObject parent, bool isEnabled)
    {
        //TODO: that should set back to previous state, not to true (they might have been changed by user code and that should not be adjusted)
        var allHolsterColliders = parent.GetComponentsInChildren<Collider>();
        foreach (var holsterCollider in allHolsterColliders)
        {
            holsterCollider.enabled = isEnabled;
        }
    }


    private void DrawPointWithPosition(Vector3 point, string text)
    {
        var previousColor = Gizmos.color;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(point, .01f);

        Gizmos.color = Color.white;
#if UNITY_EDITOR
        Handles.Label(point, $"{point} {text}");

#endif
        Gizmos.color = previousColor;
    }
}


