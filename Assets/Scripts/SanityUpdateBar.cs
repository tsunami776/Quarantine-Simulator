using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityUpdateBar : MonoBehaviour
{
     public ProgressBarPro progressBar;
     public float sanity = 50.0f;
     private const float coef = 0.2f;
     
     void Update ()
     {
         if (sanity > 99.5f) {
             sanity = 99.0f;
         }
         progressBar.SetValue(sanity, 100.0f, false);
         sanity -= coef * Time.deltaTime;
     } 
}
