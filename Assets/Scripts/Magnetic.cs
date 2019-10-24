using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 5f)]
    private float forceMagnitude = .5f;
    [SerializeField] GameObject magnet;

    void FixedUpdate()
    {
        if (magnet.activeSelf && gameObject == GameStats.goalItems[GameStats.closestItemIndex])
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce((new Vector2(magnet.transform.position.x - transform.position.x, magnet.transform.position.y - transform.position.y))*forceMagnitude);
        }
    }
}
