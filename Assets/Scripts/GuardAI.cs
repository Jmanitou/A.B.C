using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public enum StateType
{
    Patrol,
    Investigate,
    Pursue,
    Alert,
    Incapacitated
}

public class GuardAI : MonoBehaviour
{
    // General Variables
    [SerializeField] private StateType currentState = 0;
    private Vector2 spawnPosition;
    public GameObject flashLight;

    // Patrol Movement Variables
    [SerializeField] [Range(0f, 10f)] private float guardPatrolSpeed = 5f;
    [SerializeField] bool isGoingLeft = false;
    [SerializeField] [Range(0f, 15f)] float patrolDist = 30.0f;     // Max distance for patrolling

    // Start is called before the first frame update
    void Start()
    {
        // Initialize guard's movement
        spawnPosition = transform.position;
        isGoingLeft = true;
        FlipSprite();

        // Initialize guard's state
        currentState = StateType.Patrol;

        // Start the Finite State Machine
        StartCoroutine(GuardFSM());
    }

    // FSM Control
    IEnumerator GuardFSM()
    {
        // Execute the current coroutine (state)
        while (true)
        {
            yield return StartCoroutine(currentState.ToString());
        }
    }

    // Ignore the collision between the guard and the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Guard collides with player");
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    // Patrol State
    private IEnumerator Patrol()
    {
        // Debugging
        Debug.Log("Entering PATROL state");
        yield return null;

        // ==== EXECUTE PART OF PATROL STATE ====

        float distFromSpawn = transform.position.x - spawnPosition.x;

        // Gone too far, switch patrol direction
        if (distFromSpawn > patrolDist || distFromSpawn < -patrolDist)
            SwitchDirection();

        if (isGoingLeft)
        {
            // Move leftward
            transform.Translate(-guardPatrolSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            // Move rightward
            transform.Translate(guardPatrolSpeed * Time.deltaTime, 0, 0);
        }

        // ==== STATE TRANSITION ====

        if (transform.position.x < 5f)
        {
            currentState = StateType.Investigate;
        }
    }

    // Investigate State
    private IEnumerator Investigate()
    {
        // Debugging
        Debug.Log("Entering INVESTIGATE state");
        yield return null;
    }

    #region Helper Methods

    // Switch the movement direction
    void SwitchDirection()
    {
        isGoingLeft = !isGoingLeft;
        FlipSprite();
        FlipLight();
    }

    // Flip the guard sprite based on the direction he's facing
    void FlipSprite() { GetComponent<SpriteRenderer>().flipX = !isGoingLeft ? true : false; }

    // Flip flashlight direction based on the guard's facing direction
    void FlipLight()
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

    #endregion
}
