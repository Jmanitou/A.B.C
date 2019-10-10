using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stealing : MonoBehaviour
{
    public Transform playerTransform;

    [SerializeField] private float timeToSteal = 3.0f;
    [SerializeField] private float stealTimer = 0f;
    public Slider playerStealSlider;
    public Text StealHintText;

    [SerializeField] 
    [Range(0f, 3f)]
    private float range = 0;   // Range for the player to interact (steal) with the museum collection


    // Start is called before the first frame update
    void Start()
    {
        GameObject[] temps = GameObject.FindGameObjectsWithTag("GoalItem");
        foreach (GameObject temp in temps)
        {
            GameStats.goalItems.Add(temp);
        }

        // Slider starts with inactive
        playerStealSlider.gameObject.SetActive(false);
        
        // Set slider
        playerStealSlider.maxValue = timeToSteal;
        playerStealSlider.value = stealTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player still needs to steal anything
        if (GameStats.goalItems.Count > 0)
        {
            // If the player is within range of the closest object
            if (isWithinValidRange(GameStats.closestItemIndex))
            {
                // Hide the hint
                StealHintText.enabled = true;

                if (Input.GetKey(KeyCode.U))
                {
                    // Show the slider to the player to indicate stealing progress
                    playerStealSlider.gameObject.SetActive(true);

                    stealTimer += Time.deltaTime;           // Time accumulating
                    playerStealSlider.value = stealTimer;   // Update the slider value

                    // When the stealing progress reaches 100%
                    if (stealTimer >= timeToSteal)
                    {
                        Steal(GameStats.closestItemIndex);                   // Steal the item
                        stealTimer = 0f;                                // Reset the timer
                        playerStealSlider.gameObject.SetActive(false);  // Hide the slider

                        // Remove the item from the list since it has been stolen
                        GameStats.goalItems.RemoveAt(GameStats.closestItemIndex);
                    }
                }

                else
                {
                    // If not holding stealing key, hide the slider
                    playerStealSlider.gameObject.SetActive(false);
                }
            }

            // If the player is not within the range of any objects
            else
            {
                playerStealSlider.gameObject.SetActive(false);  // Hide the slider
                stealTimer = 0;
                playerStealSlider.value = 0;

                // Hide the hint
                StealHintText.enabled = false;
            }

        }
    }

    // Check if the player is within a valid range to steal the item
    bool isWithinValidRange(int index)
    {
        if (Vector2.Distance(playerTransform.position, GameStats.goalItems[index].transform.position) <= range)
        {
            return true;
        }
        return false;
    }

    // Steal a goal item given the index in the goalItems array
    void Steal(int index)
    {
        // If the player is within range
        if (Vector2.Distance(playerTransform.position, GameStats.goalItems[index].transform.position) <= range)
        {
            GameStats.NumOfItems++;
            // Disable the child (museum treasure) of a goal item
            GameStats.goalItems[index].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
