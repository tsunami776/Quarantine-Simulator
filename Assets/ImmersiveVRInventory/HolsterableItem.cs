using Assets.Backpack.GeneralUtilities;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

public class HolsterableItem : MonoBehaviour
{

    [Header("Options")]
    [SerializeField] public bool RegisterCollisionsWithTrackedController = true;
    [SerializeField] public HolsterSlot AutoHolsterSlotOnUngrab;
    [SerializeField] public Rigidbody Rigidbody;
    [SerializeField] public MeshRenderer MeshRenderer;
    [SerializeField] public Transform AttachmentOrigin;

    [Header("Debug")]
    [SerializeField] private bool IsDebug;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private Vector3 _scaleBeforeStoring;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private Quaternion _rotationBeforeStoring;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private HolsterSlot _storedInSlot;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private Transform _itemParentBeforeStoring;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private bool _isRigidbodyKinematicInitially;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private bool _isRigidbodyUsingGravityInitially;

    public TrackableCoroutine MoveRoutine { get; set; }
    public TrackableCoroutine ScalingRoutine { get; set; }
    public TrackableCoroutine RotateRoutine { get; set; }
    
    public Vector3 ScaleBeforeStoring { get { return _scaleBeforeStoring; } private set { _scaleBeforeStoring = value; } }
    public Quaternion RotationBeforeStoring { get { return _rotationBeforeStoring; } private set { _rotationBeforeStoring = value; } }
    private Transform ItemParentBeforeStoring { get { return _itemParentBeforeStoring; } set { _itemParentBeforeStoring = value; } }
    private bool IsRigidbodyKinematicInitially { get { return _isRigidbodyKinematicInitially; } set { _isRigidbodyKinematicInitially = value; } }
    private bool IsRigidbodyUsingGravityInitially { get { return _isRigidbodyUsingGravityInitially; } set { _isRigidbodyUsingGravityInitially = value; } }
    public bool IsStored => _storedInSlot != null;

    protected virtual void Awake()
    {
        Rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
        if (MeshRenderer == null) MeshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

        if (Rigidbody != null)
        {
            IsRigidbodyKinematicInitially = Rigidbody.isKinematic;
            IsRigidbodyUsingGravityInitially = Rigidbody.useGravity;
        }

        AssignExistingOrCreateAttachmentOrigin();
    }

    protected virtual void Update()
    {
        if (IsStored)
        {
            ForcePropertiesAsStored();
        }
    }


    public virtual void Store(HolsterSlot slot)
    {
        _storedInSlot = slot;
        ScaleBeforeStoring = transform.localScale;
        RotationBeforeStoring = transform.rotation;
        ItemParentBeforeStoring = this.transform.parent;

        ForcePropertiesAsStored();
    }

    private void ForcePropertiesAsStored()
    {
        if (Rigidbody != null)
        {
            Rigidbody.isKinematic = true;
            Rigidbody.useGravity = false;
        }

        if (_storedInSlot != null)
        {
            this.transform.parent = _storedInSlot.transform;
        }
    }

    public virtual void TakeOut(TrackableController controller)
    {
        _storedInSlot = null;
        this.transform.parent = ItemParentBeforeStoring;

        if (Rigidbody != null)
        {
            Rigidbody.isKinematic = IsRigidbodyKinematicInitially;
            Rigidbody.useGravity = IsRigidbodyUsingGravityInitially;
        }
    }

    public void OnDrawGizmos()
    {
        if (IsDebug)
        {
            if (AttachmentOrigin != null)
            {
                Debug.DrawRay(AttachmentOrigin.position, AttachmentOrigin.up, Color.green);
                Debug.DrawRay(AttachmentOrigin.position, AttachmentOrigin.right, Color.red);
                Debug.DrawRay(AttachmentOrigin.position, AttachmentOrigin.forward, Color.blue);
            }
        }
    }

    protected void AssignExistingOrCreateAttachmentOrigin()
    {
        if (AttachmentOrigin == null)
        {
            var attachmentOriginName = $"_{nameof(AttachmentOrigin)}";
            var existingAttachmentOrigin = transform.Find(attachmentOriginName);
            if (existingAttachmentOrigin != null)
            {
                AttachmentOrigin = existingAttachmentOrigin.transform;
            }
            else
            {
                var directionPointGo = new GameObject(attachmentOriginName);
                directionPointGo.transform.parent = transform;
                directionPointGo.transform.position = transform.position;
                AttachmentOrigin = directionPointGo.transform;
            }
        }
    }
    void Reset()
    {
        AssignExistingOrCreateAttachmentOrigin();
    }
}