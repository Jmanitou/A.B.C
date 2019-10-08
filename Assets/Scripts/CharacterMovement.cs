using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] [Range(1f, 5f)]
    private float horizontalSpeed = 2.0f;
    [SerializeField] [Range(1f, 5f)]
    private float slowerSpeed = 1.5f;
    [SerializeField] [Range(1f, 5f)]
    private float slowestSpeed = 1.0f;

    [SerializeField] private float jumpVelocity = 10.0f;

    // Collision Variables
    [SerializeField] private LayerMask groundLayerMask = 0;

    private new Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

    void Awake()
    {
        rigidbody2D = transform.GetComponent<Rigidbody2D>();
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MoveHorizontally();
        Jump();
    }

    // Control character's horizontal movement
    void MoveHorizontally()
    {
        float horizontalVal = Input.GetAxis("Horizontal");

        // Flip the character sprite when facing left
        if (horizontalVal < 0) FlipCharacter(true);
        else if (horizontalVal > 0) FlipCharacter(false);

        // Character moves horizontally
        MovementSlowDown();
        Vector3 horizontal = new Vector3(horizontalVal, 0.0f, 0.0f);
        transform.position += horizontal * horizontalSpeed * Time.deltaTime;

        transform.GetComponent<Animator>().SetFloat("WalkSpeed", Mathf.Abs(horizontalVal));
    }

    // Control if character needs to jump
    void Jump()
    {
        if (isGrounded() && Input.GetKeyDown(KeyCode.W))
        {
            rigidbody2D.velocity = Vector2.up * jumpVelocity;
        }
    }

    // Slow down the player's movement based on the inventory item number
    void MovementSlowDown()
    {
        int treasureNum = GameStats.NumOfItems;
        switch (treasureNum)
        {
            case 3: horizontalSpeed = slowerSpeed; break;
            case 4: horizontalSpeed = slowerSpeed; break;
            case 5: horizontalSpeed = slowestSpeed; break;
        }
    }

    #region Helper Methods
    // Flip the character when facing/walking left
    void FlipCharacter(bool ifFlipped) { GetComponent<SpriteRenderer>().flipX = ifFlipped ? true : false; }

    // Check if the character has hit the ground
    bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.01f, groundLayerMask);
        return raycastHit2D.collider != null;
    }
    #endregion
}
