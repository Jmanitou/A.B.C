using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMode : MonoBehaviour
{
    public Material darkMat;

    [SerializeField] private List<GameObject> darkenObjects = null;

    void Awake()
    {
        darkenObjects = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Directional Light").GetComponent<Light>().enabled = false;

        // Fill the list of game objects need to be darkened
        GameObject[] goalItems = GameObject.FindGameObjectsWithTag("GoalItem");
        foreach (GameObject goalItem in goalItems)
        {
            darkenObjects.Add(goalItem.transform.GetChild(0).gameObject);
            darkenObjects.Add(goalItem.transform.GetChild(1).gameObject);
        }

        GameObject[] otherItems = GameObject.FindGameObjectsWithTag("Dark");
        foreach (GameObject otherItem in otherItems)
        {
            darkenObjects.Add(otherItem.gameObject);
        }

        for (int i = 0; i < darkenObjects.Count; ++i)
        {
            darkenObjects[i].GetComponent<Renderer>().material = darkMat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
