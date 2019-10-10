using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dematerialize : MonoBehaviour
{
    public Slider demateralizeSlider;
    public Color dematerlizeColor;
    public Color playerColor;
    public bool dematerlized;
    public bool runOutOfTime;
    public float dematerlizeTime;
    public bool pressed;

    // Start is called before the first frame update
    void Start()
    {
        dematerlizeColor.a = 0.5f;
        dematerlizeTime = 3.0f;
        dematerlized = false;
        runOutOfTime = false;
        playerColor = gameObject.GetComponent<Renderer>().material.color;
        pressed = false;
        demateralizeSlider.gameObject.SetActive(true);
        demateralizeSlider.maxValue = 3.0f;
        demateralizeSlider.value = dematerlizeTime;
    }

    // Update is called once per frame
    void Update()
    {
        demateralizeSlider.value = dematerlizeTime;
        if (dematerlized)
        {
            dematerlizeTime -= Time.deltaTime;
        }
        else if (dematerlizeTime < 3.0f)
        {
            dematerlizeTime += Time.deltaTime;
            if (dematerlizeTime > 3.0f)
            {
                dematerlizeTime = 3.0f;
            }
        }
        if (dematerlizeTime <= 0f)
        {
            runOutOfTime = true;
            dematerlized = false;
            gameObject.GetComponent<Renderer>().material.color = playerColor; 
        }
        else if (dematerlizeTime >= 3.0f)
        {
            runOutOfTime = false;
        }

        if (Input.GetKey(KeyCode.P) && !runOutOfTime && pressed == false)
        {
            pressed = true;
            if (!dematerlized)
            {
                gameObject.GetComponent<Renderer>().material.color = dematerlizeColor;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = playerColor;
            }
            dematerlized = !dematerlized;
        }
        if (!Input.GetKey(KeyCode.P) && pressed == true)
        {
            pressed = false;
        }
    }
}
