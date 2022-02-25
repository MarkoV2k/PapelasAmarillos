using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZmianaRozmiaru : MonoBehaviour
{

    public GameObject playerSmall;
    public GameObject playerBig;

    public bool smallActive = true;
    public bool bigActive = false;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
                playerBig.SetActive(true);
                playerSmall.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerBig.SetActive(false);
            playerSmall.SetActive(true);
        }
    }
}
