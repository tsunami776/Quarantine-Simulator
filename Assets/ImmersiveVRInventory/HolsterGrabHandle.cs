using ReliableSolutions.Unity.Common;
using UnityEngine;

public class HolsterGrabHandle : MonoBehaviour
{
    [SerializeField] public FollowObjectTarget FollowObjectTarget;
    [SerializeField] public HolsterableItem HolsterItem;
    [SerializeField] public bool FollowRotationDifferenceFromInitialGrab;

}
