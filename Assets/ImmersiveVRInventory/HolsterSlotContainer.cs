using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Backpack.Extensions;
using Assets.ImmersiveVRInventory.Common.Utilities;
using UnityEngine;

public class HolsterSlotContainer : MonoBehaviour
{
    [SerializeField] public float MoveItemToStoreOverSeconds = 0.5f;

    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private BoxCollider HolsterSurfaceArea;
    private int TotalSlots => SlotColumns * SlotRows;
    [SerializeField] private int SlotColumns = 4;
    [SerializeField] private int SlotRows = 3;

    [SerializeField] public float SlotRadiusAdjustment = 1f;

    private List<HolsterSlot> holsterSlots = new List<HolsterSlot>();

    [SerializeField] private SlotContainerData SlotContainerData;
    [SerializeField] public CubemapFace CreateSlotsOnSurfaceAreaFace = CubemapFace.PositiveX;
    [SerializeField] private bool AdjustSlots;

    void Awake()
    {
        AdjustSlots = false;
        holsterSlots = GetComponentsInChildren<HolsterSlot>().ToList();
        if (!holsterSlots.Any()) CreateHolsterSlots();

        TryApplySlotContainerData();
    }

    [ContextMenu("Print Holster Slot items")]
    public void PrintHolsterSlotItems()
    {
        Debug.Log(string.Join(Environment.NewLine, 
            GetComponentsInChildren<HolsterSlot>().Select((s, i) => $"Slot {i} - {(s.CurrentlyStoredItem != null ? s.CurrentlyStoredItem.gameObject.name : "No Item")}")
        ));
    }

    [ContextMenu("Recreate Holster Slots")]
    public void RecreateHolsterSlots()
    {
        if (!Application.isEditor) return;

        var slotGos = GetComponentsInChildren<HolsterSlot>();
        foreach (var holsterSlot in slotGos)
        {
            GameObject.DestroyImmediate(holsterSlot.gameObject);
        }
        CreateHolsterSlots();
    }

    public void CreateHolsterSlots()
    {
        var faceCorners = HolsterSurfaceArea.bounds.GetCubemapEdgePoints(CreateSlotsOnSurfaceAreaFace);
        var slotPositions = GetSlotPositions(faceCorners);

        var slotRotation = GetSlotRotation();

        for (var index = 0; index < slotPositions.Count; index++)
        {
            var slotPosition = slotPositions[index];
            var slotGo = Instantiate(SlotPrefab, slotPosition, Quaternion.identity, this.transform);
            var slotScript = slotGo.GetComponent<HolsterSlot>();
            slotGo.name = $"{slotGo.name} - {index / SlotColumns} - {index % SlotColumns}";
            slotGo.transform.localRotation = Quaternion.Euler(slotRotation);
            slotGo.transform.localScale = new Vector3(SlotRadiusAdjustment, SlotRadiusAdjustment, SlotRadiusAdjustment);

            holsterSlots.Add(slotScript);
        }
    }

    private Vector3 GetSlotRotation()
    {
        switch (CreateSlotsOnSurfaceAreaFace)
        {
            case CubemapFace.PositiveX: return new Vector3(0, -90, 0);
            case CubemapFace.NegativeX: return new Vector3(0, 90, 0);
            case CubemapFace.PositiveY: return new Vector3(0, 0, 0);
            case CubemapFace.NegativeY: return new Vector3(0, -180, 0);
            case CubemapFace.PositiveZ: return new Vector3(180, 0, 0);
            case CubemapFace.NegativeZ: return new Vector3(0, 0, 0);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private List<Vector3> GetSlotPositions(CubemapEdgePoints cubemapEdgePoints)
    {
        var slotPositions = new List<Vector3>();
        var startPoint = cubemapEdgePoints.TopLeft;
        for (var rowIndex = 0; rowIndex < SlotRows; rowIndex++)
            for (var columnIndex = 0; columnIndex < SlotColumns; columnIndex++)
            {
                var slotColumnStart = (cubemapEdgePoints.TopRight - cubemapEdgePoints.TopLeft) / SlotColumns;
                var slotRowStart = (cubemapEdgePoints.BottomLeft - cubemapEdgePoints.TopLeft) / SlotRows;
                var slotCenter = startPoint 
                                 + (slotColumnStart * columnIndex + slotColumnStart /2)
                    + slotRowStart * rowIndex + slotRowStart / 2;
                slotPositions.Add(slotCenter);
            }

        return slotPositions;
    }

    public void TryApplySlotContainerData()
    {
        if (SlotContainerData != null && SlotContainerData.isSet)
        {
            transform.localPosition = SlotContainerData.positionRelativeToParent;
            transform.rotation = SlotContainerData.rotation;
        }
    }

    [ContextMenu("Persist setup")]
    public void TryCaptureSlotContainerData()
    {
        if (SlotContainerData != null)
        {
            SlotContainerData.positionRelativeToParent = transform.localPosition;
            SlotContainerData.rotation = transform.rotation;
            SlotContainerData.isSet = true;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} doesn't have {nameof(SlotContainerData)} assigned, either create entry from menu and assign or Reset 'HolsterSlotContainer' " +
                             $"via Editor which will automatically create ScriptableObject");
        }
    }

    [ContextMenu("Apply Holster Adjustments As Base")]
    public void ApplyHolsterAdjustmentsAsBase()
    {
        TryApplySlotContainerData();
    }

    private static string DefaultDataPath = "Assets/Backpack/ScriptableObjects/SlotContainerData";
    void Reset()
    {
        if (SlotContainerData == null)
        {
#if UNITY_EDITOR
            var data = ScriptableObjectUtility.CreateAsset<SlotContainerData>(DefaultDataPath, gameObject.name);
            SlotContainerData = data;
            TryCaptureSlotContainerData();
#endif
        }
    }

    void OnDrawGizmos()
    {
        if (AdjustSlots)
        {
            if (transform.rotation != Quaternion.identity)
            {
                Debug.LogWarning($"Due to the way position slots position is calculated, please set object rotation to 0. That could be normally rotated after adjustment. Once set enable {nameof(AdjustSlots)} or create.");
                AdjustSlots = false;
                return;
            }

            var cubemapEdgePoints = HolsterSurfaceArea.bounds.GetCubemapEdgePoints(CreateSlotsOnSurfaceAreaFace);

            const float SphereRadius = 0.03f;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(cubemapEdgePoints.TopLeft, SphereRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(cubemapEdgePoints.TopRight, SphereRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(cubemapEdgePoints.BottomLeft, SphereRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(cubemapEdgePoints.BottomRight, SphereRadius);

            if (_recreateSlotsForPreviewWaitTime > .5f)
            {
                RecreateHolsterSlots();
                _recreateSlotsForPreviewWaitTime = 0;
            }
            _recreateSlotsForPreviewWaitTime += Time.deltaTime;
        }
    }

    private float _recreateSlotsForPreviewWaitTime;
    void OnValidate()
    {

    }

}
