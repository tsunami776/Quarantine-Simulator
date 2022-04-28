using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFood : MonoBehaviour
{
    //public LayerMask l;
    public AudioSource eat;

    private void OnCollisionEnter(Collision other) {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("Interactable")) {
            Debug.Log("eat");
            Destroy(other.gameObject);
            GameObject hunger2 = GameObject.Find("HungerBar");
            HungerUpdateBar hungerScript2 = hunger2.GetComponent<HungerUpdateBar>();
            hungerScript2.hunger += 10.0f;
            eat.Play();  
        }
    }
}
