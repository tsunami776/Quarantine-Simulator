using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EatChicken : MonoBehaviour
{
    [SerializeField]
    TMP_Text m_TextComponent;
    
    public void EatMyChicken()
    {
        if (GameObject.Find("SMGP_LOD_Chicken") != null)
        {
            GameObject hunger = GameObject.Find("hungerValue");
            HungerUpdate hungerScript = hunger.GetComponent<HungerUpdate>();
            hungerScript.health += 20.0f;
        }
    } 
}
