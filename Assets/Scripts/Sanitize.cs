using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sanitize : MonoBehaviour
{
    public Button unboxButton;
    public AudioSource san;

    void Start()
    {
        unboxButton.interactable = false; 
    }

    public void SanitizeButton() 
    { 
        san.Play();
        unboxButton.interactable = true; 
    }
}
