using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 2.0f;

    public Animator movementAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveHorizontally();
    }

    // Control character's horizontal movement
    void MoveHorizontally()
    {
        float horizontalVal = Input.GetAxis("Horizontal");

        // Animation control
        movementAnimator.SetFloat("Horizontal", horizontalVal);

        // 

        // Move character position
        Vector3 horizontal = new Vector3(horizontalVal, 0.0f, 0.0f);
        transform.position += horizontal * speed * Time.deltaTime;
    }

    // Flip the character when facing/walking left
    void FlipCharacter(bool ifFlipped)
    {
        GetComponent<SpriteRenderer>().flipX = ifFlipped ? true : false;
    }
}
