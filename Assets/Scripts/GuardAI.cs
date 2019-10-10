using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum StateType
{
    Patrol,
    Alert,
    Chase
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
        // Initialize guard's state
        currentState = StateType.Patrol;

        spawnPosition = transform.position;
        isGoingLeft = true;
        FlipSprite();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState(currentState);
    }

    // Implementation of a state FSM
    void UpdateState(StateType currentState)
    {
        switch (currentState)
        {
            case StateType.Patrol:
                Patrol();
                break;
            case StateType.Chase:
                break;
            case StateType.Alert:
                break;
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

    #region Helper Methods

    void Patrol()
    {
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
    }

    // Switch the movement direction
    void SwitchDirection()
    {
        isGoingLeft = !isGoingLeft;
        FlipSprite();
        FlipLight();
    }

    void FlipSprite() { GetComponent<SpriteRenderer>().flipX = !isGoingLeft ? true : false; }
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
