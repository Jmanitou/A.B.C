using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMode : MonoBehaviour
{
    public Material darkMat;
    public Material shinyMat;   // Material for museum object

    public Light playerPointLight;  // Reference to the point light from the player
    public LayerMask goalItemLayerMask = 0;

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
        for (int i = 0; i < goalItems.Length; ++i)
        {
            GameObject goalItem = goalItems[i];
        }

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

    // Light up the goal item when the player's point light hits
    bool LightingUpGoalItem()
    {
        GameObject closestItem = GameStats.goalItems[GameStats.closestItemIndex].gameObject;
        RaycastHit2D circleCast2D = Physics2D.CircleCast(playerPointLight.transform.position, playerPointLight.range, closestItem.transform.position - playerPointLight.transform.position, 0.01f, goalItemLayerMask);
        return circleCast2D.collider != null;
    }
}
