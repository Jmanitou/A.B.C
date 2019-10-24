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
    [SerializeField] [Range(0f, 10f)] private float softCatchRadius = 8f;   // radius for guard soft catches player
    [SerializeField] [Range(0f, 10f)] private float hardCatchRadius = 3f;   // radius for guard hard catches player
    [SerializeField] [Range(0f, 10f)] private float arrestRadius = 2f;      // radius for guard arrests player

    [Header("Sprite/Animation Info")]
    [SerializeField] private bool isGoingLeft = false;              // boolean that stores if the sprite needs to be flipped
    [SerializeField] private bool isIdling = true;                  // boolean that stores if the guard is idling
    [SerializeField] private bool isWalking = false;                // boolean that stores if the guard is walking

    // General Movement Variables
    [Header("Path Following Attributes")]
    [SerializeField] private Transform[] waypoints;
    public int currentWPIndex = 0;
    public int numOfSearchCycle = 0;

    [Header("Movement Speed")]
    [SerializeField] [Range(0f, 10f)] private float guardPatrolSpeed = 5f;          // the patrol speed of the guard
    [SerializeField] [Range(0f, 10f)] private float guardInvestigateSpeed = 5f;     // the investigate speed of the guard
    [SerializeField] [Range(0f, 10f)] private float guardAlertSpeed = 5f;
    [SerializeField] [Range(0f, 10f)] private float guardPursueSpeed = 5f;

    // Investigate State Variables
    [Header("INVESTIGATE Attributes")]
    [SerializeField] private Vector2 areaOfInterest_center;                         // the center position of the guard's area of interest
    [SerializeField] [Range(0f, 10f)] private float areaOfInterest_radius_i;        // the radius of the guard's area of interest for investigation
    [SerializeField] private bool isAOIInitialized = false;
    public Vector2[] pointsToInvestigate;                                           // stores the points to investigate
    [SerializeField] private int timesOfSearch_i;                                   // stores the max number of investigating the AOI

    [Header("PURSUE Attributes")]
    [SerializeField] private Vector2 posToPursue;                                   // the position for the guard to pursue
    [SerializeField] private bool ifHitTheEnd;                                      // if the guard has pursued to the end or blockage

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
        ControlGuardUI(false, "");

        // Initialize player transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Start the Finite State Machine
        StartCoroutine(GuardFSM());
    }

    #region States Definitions

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

    // Patrol State
    // Guard follows a set path navigating from point to point in an order, OR stays in one place.
    // Occasionally the guard stops at a point and stays still for a few seconds.
    private IEnumerator Patrol()
    {
        // Debugging
        Debug.Log("Entering PATROL state");
        yield return null;

        // ==== EXECUTE PART OF PATROL STATE ====

        SetSpeedMuliplier(1f);
        ControlGuardUI(false, "");
        FollowPath(ConvertTransformArray(waypoints), guardPatrolSpeed);

        // ==== STATE TRANSITION ====

        // Patrol -> Investigate: Soft catch
        if (IsSoftCaught())
            currentState = StateType.Investigate;

        // Patrol -> Alert: Hard catch
        if (IsHardCaught())
            currentState = StateType.Alert;

        if (IsArrested())
            GameManager.currentGameState = GameState.GameOver;

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

        SetSpeedMuliplier(guardInvestigateSpeed / guardPatrolSpeed);
        ControlGuardUI(true, "?");
        InitializeAOI();

        if (numOfSearchCycle < timesOfSearch_i)
            SearchAreaOfInterest();

        // ==== STATE TRANSITION ====

        else
        {
            isAOIInitialized = false;
            timesOfSearch_i = 0;

            // Investigate -> Patrol: Searched around AOI and did not hard catch player
            currentState = StateType.Patrol;
        }

        // Investigate -> Investigate: Soft catch again
        if (IsSoftCaught())
        {
            isAOIInitialized = false;
            timesOfSearch_i = 0;
            currentState = StateType.Investigate;
        }

        // Investigate -> Alert: Hard catch/museum is alerted
        if (IsHardCaught())
        {
            isAOIInitialized = false;
            timesOfSearch_i = 0;
            currentState = StateType.Alert;
        }

        if (IsArrested())
            GameManager.currentGameState = GameState.GameOver;

    }

    // Alert State
    private IEnumerator Alert()
    {
        // Debugging
        Debug.Log("Entering ALERT state");
        yield return null;

        // ==== EXECUTE PART OF INVESTIGATE STATE ====

        SetSpeedMuliplier(guardAlertSpeed / guardPatrolSpeed);
        ControlGuardUI(true, "!");
        UIManager.isMuseumOnAlert = true;
        FollowPath(ConvertTransformArray(waypoints), guardAlertSpeed);

        // ==== STATE TRANSITION ====

        //if (IsSoftCaught() || IsHardCaught())
        //{
        //    currentState = StateType.Pursue;
        //}

        if (IsArrested())
            GameManager.currentGameState = GameState.GameOver;

    }

    // Pursue State
    private IEnumerator Pursue()
    {
        // Debugging
        Debug.Log("Entering PURSUE state");
        yield return null;

        // ==== EXECUTE PART OF INVESTIGATE STATE ====

        ControlGuardUI(true, "!!");
        Pursue(posToPursue);

        // ==== STATE TRANSITION ====

        if (ifHitTheEnd)
            currentState = StateType.Alert;

        if (IsArrested())
            GameManager.currentGameState = GameState.GameOver;
    }

    //// Incapacitated State
    //private IEnumerator Incapacitated()
    //{
    //    // Debugging
    //    Debug.Log("Entering INCAPACITATED state");
    //    yield return null;

    //}

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
    private void FollowPath(Vector2[] _waypoints, float maxSpeed)
    {
        // Move to the next target if close enough to current target
        if (Mathf.Abs(_waypoints[currentWPIndex].x - transform.position.x) < 0.1f)
        {
            UpdateWaypointIndex(_waypoints);
        }

        transform.Translate(Seek(_waypoints[currentWPIndex], maxSpeed) * Time.deltaTime);
    }

    // Helper method to update current waypoint index
    private void UpdateWaypointIndex(Vector2[] _waypoints)
    {
        if (currentWPIndex != _waypoints.Length - 1)
        {
            currentWPIndex++;
        }
        else
        {
            currentWPIndex = 0;
            numOfSearchCycle++;
        }
    }

    // AI Seeking behavior
    public void AISeek(Vector2 targetPos, float maxSpeed)
    {
        transform.Translate(Seek(targetPos, maxSpeed) * Time.deltaTime);
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

    // Pursue behavior of the guard AI
    private void Pursue(Vector2 targetPos)
    {
        bool pursueLeft = (transform.position.x - targetPos.x > 0) ? true : false;
        if (pursueLeft != isGoingLeft)
            SwitchDirection();

        if (pursueLeft)
            transform.Translate(new Vector2(-guardPursueSpeed * Time.deltaTime, 0));
        else
            transform.Translate(new Vector2(guardPursueSpeed * Time.deltaTime, 0));
    }

    // Animator controller
    private void ControlAnimation()
    {
        guardAnimator.SetBool("IsWalking", isWalking);
    }

    // Set walking animation speed multiplier
    private void SetSpeedMuliplier(float _multiplier)
    {
        if (Mathf.Abs(_multiplier - guardAnimator.GetFloat("SpeedMultiplier")) > Mathf.Epsilon) // floats comparison, essentially doing "if these are not equal"
            guardAnimator.SetFloat("SpeedMultiplier", _multiplier);
    }

    // Helper method to convert transform array to vector2 array (positions)
    private Vector2[] ConvertTransformArray(Transform[] _transforms)
    {
        Vector2[] output = new Vector2[_transforms.Length];
        for (int i = 0; i < output.Length; ++i)
        {
            output[i] = _transforms[i].position;
        }
        return output;
    }

    // Check if the player is soft caught by guard
    // Soft Catch - Guard perceives something of interest and is motivated to investigate it 
    // but does not know there is an actual burglar.
    private bool IsSoftCaught()
    {
        bool isPlayerOnLeft = playerTransform.position.x - transform.position.x < 0 ? true : false;
        if (isGoingLeft == isPlayerOnLeft && 
            Vector2.Distance(playerTransform.position, flashLight.transform.position) <= softCatchRadius &&
            !IsPlayerDematerialized())
        {
            Debug.Log("Player soft caught!");

            // Set the area of interest for investigate state
            if (currentState == StateType.Patrol || currentState == StateType.Investigate)
                areaOfInterest_center = isGoingLeft ? new Vector2(playerTransform.position.x - areaOfInterest_radius_i, playerTransform.position.y) :
                                                      new Vector2(playerTransform.position.x + areaOfInterest_radius_i, playerTransform.position.y);

            // Set the position to pursue
            if (currentState == StateType.Alert)
                posToPursue = playerTransform.position;
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
            Vector2.Distance(playerTransform.position, flashLight.transform.position) <= hardCatchRadius &&
            !IsPlayerDematerialized())
        {
            Debug.Log("Player hard caught!");

            // Set the position to pursue
            if (currentState == StateType.Alert)
                posToPursue = playerTransform.position;

            return true;
        }
        return false;
    }

    // Checks if the player is within the arrest range
    private bool IsArrested()
    {
        bool isPlayerOnLeft = playerTransform.position.x - transform.position.x < 0 ? true : false;
        if (isGoingLeft == isPlayerOnLeft &&
            Vector2.Distance(playerTransform.position, flashLight.transform.position) <= arrestRadius &&
            !IsPlayerDematerialized())
        {
            Debug.Log("Player arrested! Game Over!");

            return true;
        }
        return false;
    }

    private bool IsPlayerDematerialized()
    {
        return GameStats.isDematerialized;
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
            speechBubbleImg.GetComponent<Image>().enabled = true;
            exclamationText.GetComponent<Text>().enabled = true;
            isBubbleOn = isTextOn = true;
            exclamationText.text = exclamations;
        }
        else if (!isEnabled && isBubbleOn && isTextOn)
        {
            speechBubbleImg.GetComponent<Image>().enabled = false;
            exclamationText.GetComponent<Text>().enabled = false;
            isBubbleOn = isTextOn = false;
        }
        exclamationText.text = exclamations;
    }

    // Initialize the area of interest for investigate state
    private void InitializeAOI()
    {
        if (!isAOIInitialized)
        {
            // Reset the number of search times
            numOfSearchCycle = 0;

            pointsToInvestigate = new Vector2[2];

            // store left and right points for investigation
            pointsToInvestigate[0] = new Vector2(areaOfInterest_center.x - areaOfInterest_radius_i, areaOfInterest_center.y);
            pointsToInvestigate[1] = new Vector2(areaOfInterest_center.x + areaOfInterest_radius_i, areaOfInterest_center.y);

            timesOfSearch_i = Random.Range(2, 4); // Randomize a number for how many times the guard will search within the area

            isAOIInitialized = true;
        }
    }

    // When in investigate/pursue mode, search around the area of interest a couple of times
    private void SearchAreaOfInterest()
    {
        FollowPath(pointsToInvestigate, guardInvestigateSpeed);
    }

    #endregion

    #region MonoBehaviour Messages

    // Ignore the collision between the guard and the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            if (currentState == StateType.Investigate && isAOIInitialized)
            {
                UpdateWaypointIndex(pointsToInvestigate);
            }

            else if (currentState == StateType.Pursue)
            {
                ifHitTheEnd = true;
            }
        }

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Guard collides with player");
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        // Ignore collision with other guards
        if (collision.gameObject.layer == 11)
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        // Ignore the collision with goal items
        if (collision.gameObject.layer == 9 || collision.gameObject.layer == 12)
        {
            Debug.Log("Guard collides with goal items");
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        // Ignore the collision with door
        if (collision.gameObject.layer == 13)
        {
            Debug.Log("Guard collides with door");
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

        // Arrest Range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, arrestRadius);

        // Area of Interest, if there is any
        if (currentState == StateType.Investigate || currentState == StateType.Pursue)
        {
            Gizmos.color = new Color(0, 1, 1, 0.4f);    // Cyan
            Gizmos.DrawSphere(areaOfInterest_center, areaOfInterest_radius_i);
        }
    }

    #endregion
}
