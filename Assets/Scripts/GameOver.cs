using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject NameOfGameObjectThatYouWantToActivate;
    public GameObject NameOfGameObjectThatYouWantToActivate1;
    public GameObject NameOfGameObjectThatYouWantToActivate2;
    public GameObject NameOfGameObjectThatYouWantToActivate3;
    // Update is called once per frame
    void Update()
    {
        GameObject hunger2 = GameObject.Find("HungerBar");
        //GameObject GameOverScreen = GameObject.Find("GameOverScreen");
        GameObject Lighting = GameObject.Find("Lighting");
        GameObject left = GameObject.Find("LeftHand Base Controller (1)");
        GameObject right = GameObject.Find("RightHand Parent");
        GameObject infoPanel = GameObject.Find("Info Panel");
        HungerUpdateBar hungerScript2 = hunger2.GetComponent<HungerUpdateBar>();
        GameOver gv = hunger2.GetComponent<GameOver>();
        if(hungerScript2.hunger <= 1.0f && GameObject.Find("LeftHand Base Controller (1)") != null) 
        {
            left.SetActive(false);
            right.SetActive(false);
            NameOfGameObjectThatYouWantToActivate.SetActive(true);
            Lighting.SetActive(false);
            NameOfGameObjectThatYouWantToActivate1.SetActive(false);
            NameOfGameObjectThatYouWantToActivate2.SetActive(false);
            NameOfGameObjectThatYouWantToActivate3.SetActive(false);
            gv.enabled = false;
        }
    }
}
