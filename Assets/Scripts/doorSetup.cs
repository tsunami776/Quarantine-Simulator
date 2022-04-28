using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class doorSetup : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private XRBaseInteractable door = null;

    public Animator openandclose;
	// public bool open;

    // IEnumerator opening()
    // {
    //     //print("you are opening the door");
    //     openandclose.Play("Opening");
    //     open = true;
    //     yield return new WaitForSeconds(5.0f);
    // }

    // IEnumerator closing()
    // {
    //     //print("you are closing the door");
    //     openandclose.Play("Closing");
    //     open = false;
    //     yield return new WaitForSeconds(5.0f);
    // }

    // Start is called before the first frame update
    // void Start()
    // {
    //     open = false;
    // }

    // Update is called once per frame
    void Update()
    {
        if(door.isSelected) {
            openandclose.SetBool("Opened", !openandclose.GetBool("Opened"));
            //StartCoroutine(opening());
        }

        // if(!door.isSelected) {
        //     openandclose.SetBool("Opened", false);
        //     //StartCoroutine(closing());
        // }
    }
}
