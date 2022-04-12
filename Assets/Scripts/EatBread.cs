using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EatBread : MonoBehaviour
 {
    [SerializeField]
    TMP_Text m_TextComponent;
    
    public void EatMyBread()
    {
        if (GameObject.Find("SMGP_LOD_Bread") != null)
        {
            GameObject hunger = GameObject.Find("hungerValue");
            HungerUpdate hungerScript = hunger.GetComponent<HungerUpdate>();
            hungerScript.health += 10.0f;
        }
    }        
 }
