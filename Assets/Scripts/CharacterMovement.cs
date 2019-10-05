using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float horizontalSpeed = 2.0f;
    [SerializeField] private float jumpVelocity = 10.0f;

    // Collision Variables
    [SerializeField] private LayerMask groundLayerMask = 0;

    private Rigidbody2D rigidbody2D;
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
        if (horizontalVal < 0) FlipCharacter(false);
        else if (horizontalVal > 0) FlipCharacter(true);

        // Character moves horizontally
        Vector3 horizontal = new Vector3(horizontalVal, 0.0f, 0.0f);
        transform.position += horizontal * horizontalSpeed * Time.deltaTime;
    }

    // Control if character needs to jump
    void Jump()
    {
        if (isGrounded() && Input.GetKeyDown(KeyCode.W))
        {
            rigidbody2D.velocity = Vector2.up * jumpVelocity;
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
