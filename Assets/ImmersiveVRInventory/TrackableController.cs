using System;
using System.Collections.Generic;
using System.Linq;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TrackableController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool IsDebug;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] public HolsterSlot CollidingWithSlot;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] public HolsterGrabHandle CollidingWithHandle;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] public HolsterGrabHandle HolidingHandle;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] public HolsterableItem CollidingWithItem;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private SphereCollider _collider;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private HashSet<HolsterSlot> _allAlreadyCollidedSlots = new HashSet<HolsterSlot>();

    void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    void FixedUpdate()
    {
        var collisions = Physics.OverlapSphere(_collider.transform.TransformPoint(_collider.center), _collider.radius);
        var closestHolsterHandle = GetClosestCollidingComponent<HolsterGrabHandle>(collisions);
        if (closestHolsterHandle != null)
        {
            CollidingWithHandle = closestHolsterHandle;
        }
        else
        {
            CollidingWithHandle = null;
        }

        var closestSlot = GetClosestCollidingComponent<HolsterSlot>(collisions);
        if (closestSlot != null)
        {
            CollidingWithSlot?.Exit();
            CollidingWithSlot = closestSlot;
            closestSlot.TryEnter(HolidingHandle != null ? HolidingHandle.HolsterItem : CollidingWithItem);
        }
        else
        {
            CollidingWithSlot?.Exit();
            CollidingWithSlot = null;
        }

        var closestItem = GetClosestCollidingComponent<HolsterableItem>(collisions, 
            (holsterableItem) => holsterableItem.RegisterCollisionsWithTrackedController,
        lookInParentObjects: true);
        if (closestItem != null)
        {
            CollidingWithItem = closestItem;
        }
        else
        {
            CollidingWithItem = null;
        }

    }

    private TComponent GetClosestCollidingComponent<TComponent>(Collider[] collisions, Func<TComponent, bool> filterComponentsPredicate = null, bool lookInParentObjects = false) where TComponent: MonoBehaviour
    {
        if (lookInParentObjects)
        {
            return GetClosestCollidingComponent(collisions.ToList()
                .Select(c =>
                {
                    var found = c.GetComponentInParent<TComponent>();
                    return found ?? null;
                }).Where(c => c != null && (filterComponentsPredicate == null || filterComponentsPredicate(c)))
            );
        }

        return GetClosestCollidingComponent(collisions.ToList()
               .Select(c => c.TryGetComponent<TComponent>(out var ic) ? ic : null)
               .Where(c => c != null && (filterComponentsPredicate == null || filterComponentsPredicate(c)))
        );
    }

    private TComponent GetClosestCollidingComponent<TComponent>(IEnumerable<TComponent> components) where TComponent: MonoBehaviour
    {
        return components.Where(c => c != null)
            .Select(hs =>
            {
                var colliderCenter = transform.TransformPoint(_collider.center);
                return new {hs, distance = Vector3.Distance(colliderCenter, hs.transform.position)};
            })
            .OrderBy(d => d.distance)
            .Select(d => d.hs)
            .FirstOrDefault();
    }

    public void StartHoldingHandle(HolsterGrabHandle handle)
    {
        HolidingHandle = handle;
        handle.FollowObjectTarget.SetSource(transform, () => GetFollowerOffset(handle), handle.FollowRotationDifferenceFromInitialGrab);
    }

    private static Vector3 GetFollowerOffset(HolsterGrabHandle handle)
    {
        return handle.transform.position - handle.FollowObjectTarget.transform.position;
    }

    public void UpdateFollowerOffset(HolsterGrabHandle handle)
    {
        handle.FollowObjectTarget.GetPositionOffset = () => GetFollowerOffset(handle);
    }

    public void StopHoldingHandle(HolsterGrabHandle handle)
    {
        handle.FollowObjectTarget.SetSource(null, () => Vector3.zero);
        HolidingHandle = null;
    }

    private void HandleCollisionEnter(GameObject collisionGameObject)
    {
        SetActiveCollisions(collisionGameObject, (holsterSlot) =>
        {
            if (CollidingWithSlot != null)
            {
                CollidingWithSlot.Exit();
            }

            var collisions = Physics.OverlapSphere(_collider.transform.TransformPoint(_collider.center), _collider.radius);
            var closestHolsterSlotQuery = collisions.ToList()
                .Select(c => c.TryGetComponent<HolsterSlot>(out var hs) ? hs : null);

            var closestHolsterSlot = closestHolsterSlotQuery.Where(s => s != null)
                .Select(hs => new { hs, distance = Vector3.Distance(transform.position, hs.transform.position) })
                .OrderBy(d => d.distance)
                .Select(d => d.hs)
                .FirstOrDefault();


            foreach (var slot in _allAlreadyCollidedSlots)
                if(slot != closestHolsterSlot) slot.Exit();

            CollidingWithSlot = closestHolsterSlot;
            closestHolsterSlot.TryEnter(HolidingHandle != null ? HolidingHandle.HolsterItem : CollidingWithItem);
        });
    }

    private void HandleCollisionExit(GameObject collisionGameObject)
    {
        var holsterSlot = collisionGameObject.GetComponent<HolsterSlot>();
        if (holsterSlot != null && CollidingWithSlot == holsterSlot)
        {
            CollidingWithSlot.Exit();
            CollidingWithSlot = null;
        }

        var holsterableItem = collisionGameObject.GetComponentInParent<HolsterableItem>();
        if (holsterableItem != null && holsterableItem == CollidingWithItem)
        {
            CollidingWithItem = null;
        }

        var holsterHandle = collisionGameObject.GetComponent<HolsterGrabHandle>();
        if (holsterHandle != null && holsterHandle == CollidingWithHandle)
        {
            CollidingWithHandle = null;
        }
    }


    private void HandleCollisionStay(GameObject collisionGameObject)
    {
        SetActiveCollisions(collisionGameObject, null);
    }

    private void SetActiveCollisions(GameObject collisionGameObject, Action<HolsterSlot> executeForFoundHolsterSlot)
    {
        var holsterSlot = collisionGameObject.GetComponent<HolsterSlot>();
        if (holsterSlot != null)
        {
            _allAlreadyCollidedSlots.Add(holsterSlot);
            executeForFoundHolsterSlot?.Invoke(holsterSlot);
        }

        var holsterableItem = collisionGameObject.GetComponentInParent<HolsterableItem>();
        if (holsterableItem != null && holsterableItem.RegisterCollisionsWithTrackedController)
        {
            CollidingWithItem = holsterableItem;
        }

        var holsterHandle = collisionGameObject.GetComponent<HolsterGrabHandle>();
        if (holsterHandle != null)
        {
            CollidingWithHandle = holsterHandle;
        }
    }


}