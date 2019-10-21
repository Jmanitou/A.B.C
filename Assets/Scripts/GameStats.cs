using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    // Collectibles
    public static int NumOfItems;
    [SerializeField]
    private int startNumOfItems = 0;

    // Museum
    public static bool isMuseumAlert = false;

    // Time Limit
    public static float TimeLimit;
    [SerializeField]
    private float timeLimit = 60.0f;

    // Reference to goal items
    public static List<GameObject> goalItems;
    public static int closestItemIndex = 0;     // The closest goal item to the player

    // Reference to the player
    public Transform playerTransform;
    public static bool isDematerialized = false;

    void Awake()
    {
        goalItems = new List<GameObject>();
    }


    // Start is called before the first frame update
    void Start()
    {
        NumOfItems = startNumOfItems;
        TimeLimit = timeLimit;
        isDematerialized = false;
    }

    void Update()
    {
        // Find the closest goal item to the player each frame
        closestItemIndex = FindClosestGoalItem();
    }

    private int FindClosestGoalItem()
    {
        // Get only x and y values of player's position
        Vector2 playerPosition = playerTransform.position;
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < goalItems.Count; ++i)
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
