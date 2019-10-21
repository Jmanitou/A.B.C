using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 public enum StateType
{
    Patrol,
    Investigate,
    Alert,
    Pursue
    //Incapacitated
}

public class GuardAI : MonoBehaviour
{
    #region Fields

    [Header("General Attributes of AI")]
    [SerializeField] private StateType currentState = 0;    // stores the current state of the AI
    private Vector2 spawnPosition;                          // stores the original spawning location of the guard
    public GameObject flashLight;                           // a reference to the guard's flashlight
    public Animator guardAnimator;                          // a reference to the guard's movement animator
    public Transform playerTransform;                       // a reference to the player's Transform info

    [Header("Sprite/Animation Info")]
    [SerializeField] bool isGoingLeft = false;              // boolean that stores if the sprite needs to be flipped
    [SerializeField] bool isIdling = true;                  // boolean that stores if the guard is idling
    [SerializeField] bool isWalking = false;                // boolean that stores if the guard is walking

    // General Movement Variables
    [Header("Movement/Path Following Attributes")]
    [SerializeField] private Transform[] waypoints;
    public int currentWPIndex = 0;

    // States Variables
    [Header("States Attributes")]
    [SerializeField] [Range(0f, 10f)] private float softCatchRadius = 8f;   // radius for guard soft catches player
    [SerializeField] [Range(0f, 10f)] private float hardCatchRadius = 3f;   // radius for guard hard catches player
    [SerializeField] [Range(0f, 10f)] private float guardPatrolSpeed = 5f;  // the patrol speed of the guard

    // UI
    [Header("UI")]
    [SerializeField] private Image speechBubbleImg;
    [SerializeField] private Text exclamationText;
    [SerializeField] private bool isBubbleOn = false;
    [SerializeField] private bool isTextOn = false;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize guard's movement
        spawnPosition = transform.position;
        isGoingLeft = false;

        // Initialize guard's state
        currentState = StateType.Patrol;

        // Initialize UI
        isBubbleOn = isTextOn = false;

        // Start the Finite State Machine
        StartCoroutine(GuardFSM());
    }

    // FSM Control
    IEnumerator GuardFSM()
    {
        // Execute the current coroutine (state)
        while (true)
        {
            ControlAnimation();
            yield return StartCoroutine(currentState.ToString());
        }
    }

    #region States Definitions

    // Patrol State
    // Guard follows a set path navigating from point to point in an order, OR stays in one place.
    // Occasionally the guard stops at a point and stays still for a few seconds.
    private IEnumerator Patrol()
    {
        // Debugging
        Debug.Log("Entering PATROL state");
        yield return null;

        // ==== EXECUTE PART OF PATROL STATE ====

        ControlGuardUI(false, "");
        FollowPath(guardPatrolSpeed);

        // ==== STATE TRANSITION ====

        // Patrol -> Investigate: Soft catch
        if (IsSoftCaught())
            currentState = StateType.Investigate;

        // Patrol -> Alert: Hard catch
        if (IsHardCaught())
            currentState = StateType.Alert;

        //// Patrol -> Incapacitated: Knocked out
        //if (IsKnockedOut())
        //    currentState = StateType.Incapacitated;
    }

    // Investigate State
    // Guard stops and turns towards the area of interest. He waits for a moment, 
    // then moves to the point where he (or other guards) saw/heard the player. 
    // He turns around a couple times before returning to their path.
    private IEnumerator Investigate()
    {
        // Debugging
        Debug.Log("Entering INVESTIGATE state");
        yield return null;

        // ==== EXECUTE PART OF INVESTIGATE STATE ====

        ControlGuardUI(true, "!");

        // ==== STATE TRANSITION ====

    }

    // Alert State
    private IEnumerator Alert()
    {
        // Debugging
        Debug.Log("Entering ALERT state");
        yield return null;

    }

    // Pursue State
    private IEnumerator Pursue()
    {
        // Debugging
        Debug.Log("Entering PURSUE state");
        yield return null;

    }

    // Incapacitated State
    private IEnumerator Incapacitated()
    {
        // Debugging
        Debug.Log("Entering INCAPACITATED state");
        yield return null;

    }

    #endregion

    #region Helper Methods

    // Switch the movement direction
    private void SwitchDirection()
    {
        isGoingLeft = !isGoingLeft;
        FlipSprite();
        FlipLight();
    }

    // Flip the guard sprite based on the direction he's facing
    private void FlipSprite() { GetComponent<SpriteRenderer>().flipX = isGoingLeft ? true : false; }

    // Flip flashlight direction based on the guard's facing direction
    private void FlipLight()
    {
        // Flip relative position
        float lightPosX = flashLight.transform.localPosition.x;
        float lightPosY = flashLight.transform.localPosition.y;
        float lightPosZ = flashLight.transform.localPosition.z;
        flashLight.transform.localPosition = new Vector3(-lightPosX, lightPosY, lightPosZ);

        // Flip relative rotation
        float lightRotX = flashLight.transform.localRotation.eulerAngles.x;
        float lightRotY = flashLight.transform.localRotation.eulerAngles.y;
        float lightRotZ = flashLight.transform.localRotation.eulerAngles.z;
        flashLight.transform.localRotation = Quaternion.Euler(new Vector3(180 - lightRotX, lightRotY, lightRotZ));
    }

    // Moves the agent along a series of waypoints forming a path
    private void FollowPath(float maxSpeed)
    {
        // Move to the next target if close enough to current target
        if (Mathf.Abs(waypoints[currentWPIndex].position.x - transform.position.x) < 0.1f)
        {
            currentWPIndex = (currentWPIndex == waypoints.Length - 1) ? 0 : currentWPIndex + 1;
            Debug.Log("Current waypoint index is: " + currentWPIndex);
        }

        transform.Translate(Seek(waypoints[currentWPIndex].position, maxSpeed) * Time.deltaTime);
    }

    // Returns a force that directs an agent toward a target position
    private Vector2 Seek(Vector2 targetPos, float maxSpeed)
    {
        Vector2 desiredVelocity = targetPos - (Vector2)transform.position;
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;
        desiredVelocity.y = 0;      // Doesn't move on the y-axis

        // Set idle/walk state
        if (isIdling) isIdling = false;
        if (isWalking == false) isWalking = true;
        
        // Check facing direction
        bool facingLeft = false; ;
        if (desiredVelocity.x > 0) facingLeft = false;
        else if (desiredVelocity.x < 0) facingLeft = true;

        if (facingLeft != isGoingLeft && isWalking) SwitchDirection();

        return desiredVelocity;
    }
    
    // Animator controller
    private void ControlAnimation()
    {
        guardAnimator.SetBool("IsWalking", isWalking);
    }

    // Check if the player is soft caught by guard
    // Soft Catch - Guard perceives something of interest and is motivated to investigate it 
    // but does not know there is an actual burglar.
    private bool IsSoftCaught()
    {
        bool isPlayerOnLeft = playerTransform.position.x - transform.position.x < 0 ? true : false;
        if (isGoingLeft == isPlayerOnLeft && 
            Vector2.Distance(playerTransform.position, flashLight.transform.position) <= softCatchRadius)
        {
            Debug.Log("Player soft caught!");
            return true;
        }
        return false;
    }

    // Check if the player is hard caught by guard
    // Hard Catch - A hard catch means that the guard has seen the player to a point 
    // where they know they saw someone who’s not supposed to be there and will put the museum on alert.
    private bool IsHardCaught()
    {
        bool isPlayerOnLeft = playerTransform.position.x - transform.position.x < 0 ? true : false;
        if (isGoingLeft == isPlayerOnLeft &&
            Vector2.Distance(playerTransform.position, flashLight.transform.position) <= hardCatchRadius)
        {
            Debug.Log("Player hard caught!");
            return true;
        }
        return false;
    }

    // Check if the guard has been knocked out by the player
    private bool IsKnockedOut()
    {
        return false;
    }

    // Control the guard's speech bubble and exclamation marks UI
    private void ControlGuardUI(bool isEnabled, string exclamations)
    {
        if (isEnabled && !isBubbleOn && !isTextOn)
        {
            Debug.Log("Enable Guard UI!!");
            speechBubbleImg.GetComponent<Image>().enabled = true;
            exclamationText.GetComponent<Text>().enabled = true;
            isBubbleOn = isTextOn = true;
            exclamationText.text = exclamations;
        }
    }

    #endregion

    #region MonoBehaviour Messages

    // Ignore the collision between the guard and the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Guard collides with player");
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        // Patrol Path Range
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; ++i)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        // Soft Catch Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(flashLight.transform.position, softCatchRadius);


        // Hard Catch Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(flashLight.transform.position, hardCatchRadius);

    }

    #endregion
}
