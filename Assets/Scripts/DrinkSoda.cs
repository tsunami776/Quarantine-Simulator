using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrinkSoda : MonoBehaviour
{
    [SerializeField]
    TMP_Text m_TextComponent;
    
    public void DrinkMySoda()
    {
        if (GameObject.Find("SMGP_LOD_Soda_can") != null)
        {
            GameObject hunger = GameObject.Find("hungerValue");
            HungerUpdate hungerScript = hunger.GetComponent<HungerUpdate>();
            hungerScript.health += 10.0f;
        }
    } 
}
