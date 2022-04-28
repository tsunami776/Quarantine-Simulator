using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JobUpdateBar : MonoBehaviour
{
     public ProgressBarPro progressBar;
     public TMP_Text textComponent;
     public float jobProgress = 0.0f;
     public float money = 100.0f;
     private const float modelingCoef = 2.0f;
     private const float editingCoef = 1.5f;
     public bool jobDone = true;
     public string mode;
     
     public void startModelingJob() 
     {
         mode = "modeling";
         jobDone = false;
     }

     public void startEditingJob() 
     {
         mode = "editing";
         jobDone = false;
     }

     void Update ()
     {
         if (jobProgress > 99.9f)
         {
             jobDone = true;
             jobProgress = 0.0f;
             progressBar.SetValue(jobProgress, 100.0f, false);
             mode = "";
         }
         if (!jobDone)
         {
             if (mode == "modeling") 
             {
                progressBar.SetValue(jobProgress, 100.0f, false);
                jobProgress += modelingCoef * Time.deltaTime;
                money += modelingCoef * Time.deltaTime;
                textComponent.text = money.ToString("F1");
                GameObject sanityBar = GameObject.Find("SanityBar");
                SanityUpdateBar sanityUpdateScript = sanityBar.GetComponent<SanityUpdateBar>();
                sanityUpdateScript.sanity += modelingCoef * Time.deltaTime;
             }
             if (mode == "editing") 
             {
                progressBar.SetValue(jobProgress, 100.0f, false);
                jobProgress += editingCoef * Time.deltaTime;
                money += editingCoef * Time.deltaTime;
                textComponent.text = money.ToString("F1");
                GameObject sanityBar = GameObject.Find("SanityBar");
                SanityUpdateBar sanityUpdateScript = sanityBar.GetComponent<SanityUpdateBar>();
                sanityUpdateScript.sanity += editingCoef * Time.deltaTime;
             }
         } else {
             textComponent.text = money.ToString("F1");
         }
         
     }   
}
