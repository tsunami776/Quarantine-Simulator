using System.Collections;
using System.Linq;
using Assets.Backpack;
using Assets.Backpack.GeneralUtilities;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

public class HolsterSlot : MonoBehaviour
{
    private static string AttachmentPointGoName = $"_{nameof(AttachmentPoint)}";
    public static readonly string RingRadiusShaderPropertyName = "_RingRadius";
    public static readonly string ThicknessShaderPropertyName = "_Thickness";

    private MaterialPropertyBlock _ringRadiusMaterialPropertyBlock;
    private ReversableAnimationTrackableCoroutine _animationRoutine;
    private Renderer _renderer;
    private HolsterManager _holsterManager;

    [Header("Options")]
    [SerializeField]
    private AnimationCurve RingRadiusAnimation = AnimationCurve.Linear(
        0, 0.8f,
        1, 1f
    );
    [SerializeField] public bool ScaleItemsToFit = true;
    [SerializeField] public Transform AttachmentPoint;
    [SerializeField] private SphereCollider FitStoredItemsToBounds;
    [SerializeField] [TagSelector] private string[] LimitToItemsWithTag = new string[] { };
    [SerializeField] private HolsterableItem InitiallyStoredItem;
    
    [Header("Debug")]
    [SerializeField] private bool IsDebug = false;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private float _currentRingRadius;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private HolsterableItem _currentlyStoredItem;
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private HolsterSlotContainer _holsterSlotContainer;
    

    public HolsterSlotContainer HolsterSlotContainer
    {
        get { return _holsterSlotContainer; }
        private set { _holsterSlotContainer = value; }
    }

    public HolsterableItem CurrentlyStoredItem
    {
        get { return _currentlyStoredItem; }
        private set { _currentlyStoredItem = value; }
    }

    void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        HolsterSlotContainer = GetComponentInParent<HolsterSlotContainer>();
    }

    void Start()
    {
        _currentRingRadius = RingRadiusAnimation.Evaluate(0f);
        _ringRadiusMaterialPropertyBlock = new MaterialPropertyBlock();
        _holsterManager = FindObjectOfType<HolsterManager>();

        UpdateRingRadius();

        if (InitiallyStoredItem != null)
        {
            _holsterManager.StoreInSlot(this, InitiallyStoredItem);
        }
    }

    public Bounds GetItemHoldingSphereBounds()
    {
        return FitStoredItemsToBounds.bounds;
    }

    public bool Store(HolsterableItem item)
    {
        if (!CanStore(item)) return false;

        if (CurrentlyStoredItem != null)
            return false;

        CurrentlyStoredItem = item;
        item.Store(this);

        return true;
    }

    public bool CanStore(HolsterableItem item)
    {
        if (LimitToItemsWithTag.Any() && LimitToItemsWithTag.All(t => item.tag != t))
        {
            return false;
        }

        return true;
    }

    public HolsterableItem TakeOut(TrackableController controller)
    {
        var storedItem = CurrentlyStoredItem;
        storedItem.TakeOut(controller);
        CurrentlyStoredItem = null;
        return storedItem;
    }

    public bool TryEnter(HolsterableItem item)
    {
        if (item != null && !CanStore(item)) return false;

        StartAnimationRoutine(FloatAnimation.Direction.Forward);

        return true;
    }

    private void StartAnimationRoutine(FloatAnimation.Direction direction)
    {
        var currentElapsedTimeDirectionAdjusted = _animationRoutine?.CurrentElapsedTimeDirectionAdjusted ?? 0f;
        _animationRoutine?.ForceStop();

        _animationRoutine = FloatAnimation.OverAnimationCurve(RingRadiusAnimation,
            (value) =>
            {
                _currentRingRadius = value;
                UpdateRingRadius();
            }, currentElapsedTimeDirectionAdjusted,
            direction
        );
        _animationRoutine.Start(StartCoroutine);
    }

    private void UpdateRingRadius()
    {
        _renderer.GetPropertyBlock(_ringRadiusMaterialPropertyBlock);
        _ringRadiusMaterialPropertyBlock.SetFloat(RingRadiusShaderPropertyName, _currentRingRadius);
        _renderer.SetPropertyBlock(_ringRadiusMaterialPropertyBlock);
    }

    public bool Exit()
    {
        StartAnimationRoutine(FloatAnimation.Direction.Backward);

        return true;
    }

    public void OnDrawGizmos()
    {
        if (IsDebug)
        {
            if (AttachmentPoint != null)
            {
                Debug.DrawRay(AttachmentPoint.position, AttachmentPoint.up, Color.green);
                Debug.DrawRay(AttachmentPoint.position, AttachmentPoint.right, Color.red);
                Debug.DrawRay(AttachmentPoint.position, AttachmentPoint.forward, Color.blue);
            }

            var sphereBounds = FitStoredItemsToBounds.bounds;
            Gizmos.DrawWireCube(sphereBounds.center, sphereBounds.size);

            if (CurrentlyStoredItem != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(CurrentlyStoredItem.MeshRenderer.bounds.center, CurrentlyStoredItem.MeshRenderer.bounds.size);
            }
        }
    }

    void Reset()
    {
        if (AttachmentPoint == null)
        {
            var existing = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == AttachmentPointGoName);
            if (existing != null)
            {
                AttachmentPoint = existing;
            }
            else
            {
                var attachmentPointGo = new GameObject(AttachmentPointGoName);
                attachmentPointGo.transform.parent = transform;
                attachmentPointGo.transform.position = transform.position;
                AttachmentPoint = attachmentPointGo.transform;
            }
        }
    }



    [ContextMenu("Start Animation forward")]
    public void TestStartAnimationForward() => StartAnimationRoutine(FloatAnimation.Direction.Forward);

    [ContextMenu("Start Animation backward")]
    public void TestStartAnimationBackward() => StartAnimationRoutine(FloatAnimation.Direction.Backward);

    [ContextMenu("Start Animation toggle")]
    public void TestStartAnimationToggleCoroutine()
    {
        StartCoroutine(TestStartAnimationToggle());
    }

    private IEnumerator TestStartAnimationToggle()
    {
        StartAnimationRoutine(FloatAnimation.Direction.Forward);
        yield return new WaitForSeconds(0.4f);

        StartAnimationRoutine(FloatAnimation.Direction.Backward);
        yield return new WaitForSeconds(0.2f);

        StartAnimationRoutine(FloatAnimation.Direction.Forward);
        yield return new WaitForSeconds(0.8f);

        StartAnimationRoutine(FloatAnimation.Direction.Backward);
        yield return new WaitForSeconds(0.3f);

        StartAnimationRoutine(FloatAnimation.Direction.Forward);
        yield return new WaitForSeconds(0.3f);

        StartAnimationRoutine(FloatAnimation.Direction.Forward);
        yield return new WaitForSeconds(2f);

        StartAnimationRoutine(FloatAnimation.Direction.Backward);
    }
}
