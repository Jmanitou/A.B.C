using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMode : MonoBehaviour
{
    public Material darkMat;

    [SerializeField] private GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < gameObjects.Length; ++i)
        {
            gameObjects[i].GetComponent<Renderer>().material = darkMat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
