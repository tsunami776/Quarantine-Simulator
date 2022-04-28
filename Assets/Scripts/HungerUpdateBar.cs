using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerUpdateBar : MonoBehaviour
 {
     public ProgressBarPro progressBar;
     public float hunger = 50.0f;
     private const float coef = 0.2f;
     
     void Update ()
     {
         if (hunger > 99.5f) {
             hunger = 99.0f;
         }
         progressBar.SetValue(hunger, 100.0f, false);
         hunger -= coef * Time.deltaTime;
     }        
 }
