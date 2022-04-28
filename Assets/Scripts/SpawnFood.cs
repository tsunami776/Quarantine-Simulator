using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnFood : MonoBehaviour
{
     public TMP_Text textComponent;
     public GameObject package;
     public Vector3 foodPos;
     public ProgressBarPro progressBar;
     public AudioSource ring;
     public float orderProgress = 0.0f;
     private const float orderCoef = 15.0f;
     public bool orderDone = true;
     public string mode;

     public void startFoodOrder(GameObject myfood) 
     {
         package = myfood;
         mode = "ordering";
         orderDone = false;
         GameObject jobprogressbar = GameObject.Find("JobProgressBar");
         JobUpdateBar jobupdatescript = jobprogressbar.GetComponent<JobUpdateBar>();
         jobupdatescript.money -= 20;
         textComponent.text = jobupdatescript.money.ToString("F1");
     }

     private void spawnFood()
    {
        Instantiate(package, foodPos, Quaternion.identity);
    }

     void Update ()
     {
         if (orderProgress > 99.9f)
         {
             orderDone = true;
             orderProgress = 0.0f;
             progressBar.SetValue(orderProgress, 100.0f, false);
             mode = "";
             spawnFood();
             //ring = GetComponent<AudioSource>();
             ring.Play();
         }
         if (!orderDone)
         {
             if (mode == "ordering") 
             {
                progressBar.SetValue(orderProgress, 100.0f, false);
                orderProgress += orderCoef * Time.deltaTime;
             }
         } 
     }
}
