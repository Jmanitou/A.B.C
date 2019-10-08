using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text goalItemsCollected = null;

    // Start is called before the first frame update
    void Start()
    {
        goalItemsCollected.text = "Goal Items: " + GameStats.NumOfItems + "/5";
    }

    // Update is called once per frame
    void Update()
    {
        goalItemsCollected.text = "Goal Items: " + GameStats.NumOfItems + "/5";
    }
}
