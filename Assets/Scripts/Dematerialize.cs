using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dematerialize : MonoBehaviour
{
    public Slider demateralizeSlider;
    public Image sliderImageFill;
    private Color sliderImageFillColor;
    public Color dematerlizeColor;
    public Color playerColor;
    public bool dematerlized;
    public bool runOutOfTime;
    public float dematerlizeTime;

    public Light playerPointLight;
    private float lightIntensity;
    [SerializeField] [Range(0f, 1f)] float dimVal;

    // Start is called before the first frame update
    void Start()
    {
        dematerlizeColor.a = 0.5f;
        dematerlizeTime = 3.0f;
        dematerlized = false;
        SetIsDematerialized();
        runOutOfTime = false;
        playerColor = gameObject.GetComponent<Renderer>().material.color;
        demateralizeSlider.gameObject.SetActive(true);
        demateralizeSlider.maxValue = 3.0f;
        demateralizeSlider.value = dematerlizeTime;
        sliderImageFillColor = sliderImageFill.color;

        lightIntensity = playerPointLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        // Set the transparency of the fill color of the slider
        sliderImageFillColor.a = demateralizeSlider.value / demateralizeSlider.maxValue;
        sliderImageFill.color = sliderImageFillColor;

        // Set point light intensity
        if (dematerlized)
        {
            playerPointLight.intensity = lightIntensity * dimVal;
        }
        else
        {
            playerPointLight.intensity = lightIntensity;
        }

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
            SetIsDematerialized();
            gameObject.GetComponent<Renderer>().material.color = playerColor; 
        }
        else if (dematerlizeTime >= 3.0f)
        {
            runOutOfTime = false;
        }
        demateralizeSlider.value = dematerlizeTime;

        if (Input.GetKey(KeyCode.P) && !runOutOfTime)
        {
            if (!dematerlized)
            {
                dematerlized = !dematerlized;
                SetIsDematerialized();
            }
            gameObject.GetComponent<Renderer>().material.color = dematerlizeColor;
        }
        else
        {
            if (dematerlized)
            {
                dematerlized = !dematerlized;
                SetIsDematerialized();
            }
            gameObject.GetComponent<Renderer>().material.color = playerColor;
        }
    }

    // Set global boolean isDematerialized
    private void SetIsDematerialized()
    {
        GameStats.isDematerialized = dematerlized;
    }
}
