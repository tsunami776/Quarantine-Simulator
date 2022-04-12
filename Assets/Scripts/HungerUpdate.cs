using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HungerUpdate : MonoBehaviour
 {
     public TMP_Text m_TextComponent;
     public float health = 100.0f;
     private const float coef = 0.2f;
     
     void Update ()
     {
         m_TextComponent.text = health.ToString("F1");
         health -= coef * Time.deltaTime;
     }        
 }
