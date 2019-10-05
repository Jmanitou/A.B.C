using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealing : MonoBehaviour
{
    [SerializeField] private GameObject[] goalItems;
    public Transform playerTransform;

    [SerializeField] private float range;   // Range for the player to interact (steal) with the museum collection

    // Start is called before the first frame update
    void Start()
    {
        goalItems = GameObject.FindGameObjectsWithTag("GoalItem");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Steal(FindClosestGoalItem());
        }
    }

    // Steal a goal item given the index in the goalItems array
    void Steal(int index)
    {
        // Disable the child (museum collection) of a goal item
        goalItems[index].transform.GetChild(0).gameObject.SetActive(false);
    }

    int FindClosestGoalItem()
    {
        // Get only x and y values of player's position
        Vector2 playerPosition = playerTransform.position;
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < goalItems.Length; ++i)
        {
            GameObject go = goalItems[i];

            // Calculate the distance between the player and the current museum collection
            float distance = Vector2.Distance(playerPosition, go.transform.position);

            if (distance < closestDistance)
            {
                closestIndex = i;           // Update the closest index
                closestDistance = distance; // Update the closest distance
            }
        }

        return closestIndex;
    }
}
