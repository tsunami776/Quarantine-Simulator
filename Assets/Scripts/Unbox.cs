using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unbox : MonoBehaviour
{
    public GameObject objectToDestroy1;
    public GameObject objectToDestroy2;
    // public GameObject newFood;
    // public Vector3 foodPos;
    // Start is called before the first frame update
    public void UnboxFood()
    {
        // GameObject b = Instantiate(newFood) as GameObject;
        // b.transform.position = foodPos;
        // b.transform.localScale = new Vector3(1, 1, 1);
        Destroy (objectToDestroy1);
        Destroy (objectToDestroy2);
    }
}
