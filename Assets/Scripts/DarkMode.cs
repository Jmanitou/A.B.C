using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMode : MonoBehaviour
{
    public Material darkMat;
    public Material shinyMat;   // Material for museum object

    public Light playerPointLight;  // Reference to the point light from the player
    [SerializeField] 
    [Range(0f, 2f)]
    private float lightRange = 5f;

    [SerializeField] private List<GameObject> darkenObjects = null;

    [SerializeField] private GameObject closestTreasure;
    private bool ifTreasureIsShiny = false;

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
            darkenObjects.Add(goalItem.transform.GetChild(0).gameObject); // Treasure
            darkenObjects.Add(goalItem.transform.GetChild(1).gameObject); // Pedestal
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

        // Initialize closest treasure to the player
        closestTreasure = GameStats.goalItems.Count > 0 ?
            GameStats.goalItems[GameStats.closestItemIndex].gameObject.transform.GetChild(0).gameObject : null;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStats.goalItems.Count > 0)
        {
            // Update the reference to the closest treasure
            closestTreasure = GameStats.goalItems[GameStats.closestItemIndex].gameObject.transform.GetChild(0).gameObject;

            // Update the treasure's material
            // If light hit a treasure, and the treasure is not shiny
            if (LightHitsGoalItem() && !ifTreasureIsShiny)
            {
                closestTreasure.GetComponent<Renderer>().material = shinyMat;
                ifTreasureIsShiny = true;
            }
            // If light did not hit any treasure, and the treasure is shiny
            else if (!LightHitsGoalItem() && ifTreasureIsShiny)
            {
                closestTreasure.GetComponent<Renderer>().material = darkMat;
                ifTreasureIsShiny = false;
            }
        }
    }

    // Light up the goal item when the player's point light hits
    bool LightHitsGoalItem()
    {
        if (Vector2.Distance(closestTreasure.transform.position, playerPointLight.transform.position)
            <= lightRange + closestTreasure.GetComponent<SpriteRenderer>().bounds.extents.magnitude)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPointLight.transform.position, lightRange);
    }
}
