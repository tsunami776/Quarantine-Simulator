using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotContainerData", menuName = "ScriptableObjects/SlotContainerData", order = 1)]
public class SlotContainerData : ScriptableObject
{
    public bool isSet = false;
    public Vector3 positionRelativeToParent;
    public Quaternion rotation;
}
