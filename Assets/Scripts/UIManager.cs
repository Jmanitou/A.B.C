using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Reference to UI elements
    [SerializeField] private Text goalItemsCollected = null;
    [SerializeField] private Image alarm;

    // Miscellaneous
    [Header("Miscellaneous")]
    public static bool isMuseumOnAlert = false;
    [SerializeField] [Range(0f, 0.5f)] private float alarmAlphaMax = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float alarmFlashTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        goalItemsCollected.text = "Goal Items: " + GameStats.NumOfItems + "/5";
        StartCoroutine(StartAlarm());
    }

    // Update is called once per frame
    void Update()
    {
        goalItemsCollected.text = "Goal Items: " + GameStats.NumOfItems + "/5";

        // Debug the flash
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isMuseumOnAlert = !isMuseumOnAlert;
        }
    }

    private IEnumerator StartAlarm()
    {
        while (true)
        {
            yield return StartCoroutine(MuseumAlert(alarmAlphaMax, alarmFlashTime));
        }
    }

    // Screen flashes red when the museum is on alert
    private IEnumerator MuseumAlert(float aValue, float aTime)
    {
        if (isMuseumOnAlert)
        {
            float alpha = alarm.color.a;            // get the current alpha value of the image
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor = new Color(1, 0, 0, Mathf.Lerp(alpha, aValue, t));
                alarm.color = newColor;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime / aTime)
            {
                Color newColor = new Color(1, 0, 0, Mathf.Lerp(alpha, aValue, t));
                alarm.color = newColor;
                yield return null;
            }
            alarm.color = new Color(0, 0, 0, 0);    // reset the color of the alarm
            yield return new WaitForSeconds(2);
        }
    }
}
