using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static int NumOfItems;
    [SerializeField]
    private int startNumOfItems = 0;

    public static float TimeLimit;
    [SerializeField]
    private float timeLimit = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        NumOfItems = startNumOfItems;
        TimeLimit = timeLimit;
    }


}
