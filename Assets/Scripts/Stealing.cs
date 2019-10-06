using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stealing : MonoBehaviour
{
    [SerializeField] private List<GameObject> goalItems;
    public Transform playerTransform;

    [SerializeField] private float timeToSteal = 3.0f;
    [SerializeField] private float stealTimer = 0f;
    public Slider playerStealSlider;

    [SerializeField] 
    [Range(0f, 3f)]
    private float range = 0;   // Range for the player to interact (steal) with the museum collection

    void Awake()
    {
        goalItems = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] temps = GameObject.FindGameObjectsWithTag("GoalItem");
        foreach (GameObject temp in temps)
        {
            goalItems.Add(temp);
        }

        // Set slider
        playerStealSlider.maxValue = timeToSteal;
        playerStealSlider.value = stealTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.U))
        {
            stealTimer += Time.deltaTime; // Time accumulating
            playerStealSlider.value = stealTimer;

            if (stealTimer >= timeToSteal)
            {
                Steal(FindClosestGoalItem());
                stealTimer = 0f;
            }
        }
    }

    // Steal a goal item given the index in the goalItems array
    void Steal(int index)
    {
        // If the player is within range
        if (Vector2.Distance(playerTransform.position, goalItems[index].transform.position) <= range)
        {
            // Disable the child (museum collection) of a goal item
            goalItems[index].transform.GetChild(0).gameObject.SetActive(false);

            // Remove the item from the list since it has been stolen
            goalItems.RemoveAt(index);
        }
    }

    int FindClosestGoalItem()
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

    private void OnDrawGizmosSelected()
    {
        // Draw the range of the object
        Gizmos.color = Color.yellow;
        foreach (GameObject go in goalItems)
        {
            Gizmos.DrawWireSphere(go.transform.position, range);
        }
    }
}
