using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Displayer : MonoBehaviour
{   
    public TextMeshProUGUI livesText; 
    public TextMeshProUGUI collectText; 
    public TextMeshProUGUI timeText; 

    public Tabulator tabulator;


    // update each section of text. 
    // in the case of pick ups reaching a certain number, change the color of display 
    void Update()
    {
        livesText.text = $"Lives: {tabulator.Lives}";
        timeText.text = $"{tabulator.RemainingTime.Minutes:D2}:{tabulator.RemainingTime.Seconds:D2}";
        collectText.text = $"{tabulator.Collect}"; 

        if(tabulator.Collect == 12)
        {
            collectText.color = new Color32(255, 255, 15, 255);
        }
    }
}
