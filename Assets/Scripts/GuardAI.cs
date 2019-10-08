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
    [SerializeField] private StateType currentState = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize guard's state
        currentState = StateType.Patrol;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Implementation of a state FSM
    void UpdateState(StateType currentState)
    {
        switch (currentState)
        {
            case StateType.Patrol:
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

    #endregion
}
