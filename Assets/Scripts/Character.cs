﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Character : MonoBehaviour
{
    [SerializeField] [Range(1f, 5f)]
    private float horizontalSpeed = 2.0f;
    [SerializeField] [Range(1f, 5f)]
    private float slowerSpeed = 1.5f;
    [SerializeField] [Range(1f, 5f)]
    private float slowestSpeed = 1.0f;

    [SerializeField] private float horizontalVal = 0f;

    [SerializeField] private float jumpVelocity = 10.0f;

    // Collision Variables
    [SerializeField] private LayerMask groundLayerMask = 0;

    private new Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

    public GameObject door;
    public GameObject openDoorHint;

    public bool hasMagnet = false;
    [SerializeField] GameObject magnetObject;
    public List<Item> inventory;

    void Awake()
    {
        rigidbody2D = transform.GetComponent<Rigidbody2D>();
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        inventory = new List<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveHorizontally();
        Jump();
        ExitLevel();
    }

    // Control character's horizontal movement
    void MoveHorizontally()
    {
        horizontalVal = Input.GetAxis("Horizontal");

        // Flip the character sprite when facing left
        if (horizontalVal < 0) FlipCharacter(true);
        else if (horizontalVal > 0) FlipCharacter(false);

        // Character moves horizontally
        MovementSlowDown();
        Vector3 horizontal = new Vector3(horizontalVal, 0.0f, 0.0f);
        transform.position += horizontal * horizontalSpeed * Time.deltaTime;

        transform.GetComponent<Animator>().SetFloat("WalkSpeed", Mathf.Abs(horizontalVal));
        ToggleMagnet();
    }

    // Control if character needs to jump
    void Jump()
    {
        if (isGrounded() && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rigidbody2D.velocity = Vector2.up * jumpVelocity;
        }
    }

    void ToggleMagnet()
    {
        if (Input.GetKeyDown(KeyCode.O) && hasMagnet)
        {
            transform.GetComponent<Animator>().SetBool("HoldMagnet", !transform.GetComponent<Animator>().GetBool("HoldMagnet"));
            magnetObject.SetActive(!magnetObject.activeSelf);
        }
    }

    // Slow down the player's movement based on the inventory item number
    void MovementSlowDown()
    {
        int treasureNum = GameStats.NumOfItems;
        switch (treasureNum)
        {
            case 3: horizontalSpeed = slowerSpeed; jumpVelocity = 9.5f;  break;
            case 4: horizontalSpeed = slowerSpeed; jumpVelocity = 8.0f;  break;
            case 5: horizontalSpeed = slowestSpeed; jumpVelocity = 7.5f; break;
        }
    }

    // When the player decides to leave the museum level
    private void ExitLevel()
    {
        // Key to open the door
        if (Mathf.Abs(transform.position.x - door.GetComponent<Collider2D>().bounds.center.x) < door.GetComponent<Collider2D>().bounds.extents.x)
        {
            openDoorHint.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
                GameManager.currentGameState = GameState.ExitingLevel;
        }
        else
        {
            openDoorHint.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore the collision with goal items
        if (collision.gameObject.layer == 9 || collision.gameObject.layer == 12)
        {
            Debug.Log("Player collides with goal items");
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        // Ignore the collision with door
        if (collision.gameObject.layer == 13)
        {
            Debug.Log("Player enters collision with door");
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }


    #region Helper Methods
    // Flip the character when facing/walking left
    void FlipCharacter(bool ifFlipped)
    {
        GetComponent<SpriteRenderer>().flipX = ifFlipped ? true : false;
        magnetObject.GetComponent<SpriteRenderer>().flipX = ifFlipped ? true : false;
    }

    // Check if the character has hit the ground
    bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.01f, groundLayerMask);
        return raycastHit2D.collider != null;
    }
    #endregion
}
