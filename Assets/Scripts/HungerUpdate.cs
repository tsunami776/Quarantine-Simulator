using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HungerUpdate : MonoBehaviour
 {
     public TMP_Text m_TextComponent;
     public float hunger = 50.0f;
     private const float coef = 0.2f;
     
     void Update ()
     {
         m_TextComponent.text = hunger.ToString("F1");
         hunger -= coef * Time.deltaTime;
     }        
 }
