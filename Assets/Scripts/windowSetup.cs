using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class windowSetup : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private XRBaseInteractable window = null;

    public Animator openandclose;
	public bool open;

    IEnumerator opening()
    {
        //print("you are opening the door");
        openandclose.Play("Openingwindow");
        open = true;
        yield return new WaitForSeconds(5.0f);
    }

    IEnumerator closing()
    {
        //print("you are closing the door");
        openandclose.Play("Closingwindow");
        open = false;
        yield return new WaitForSeconds(5.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        open = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(window.isSelected) {
            //openandclose.SetBool("Opened", !openandclose.GetBool("Opened"));
            StartCoroutine(opening());
        }

        if(!window.isSelected) {
            //openandclose.SetBool("Opened", false);
            StartCoroutine(closing());
        }
    }
}
