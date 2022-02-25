using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZmianaRozmiaruV2 : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 direction;
    public float speed = 8;
    public float jumpForce = 10;
    public float gravity = -20;

    public Transform groundCheck;
    public LayerMask groundLayer;

    public bool doubleJump = true;

    public Animator animator;
    public Transform model;

    public GameObject player;
    private Vector3 scaleChange, scaleChange2;

    void Awake()
    {
        scaleChange = new Vector3(2f, 2f, 2f);
        scaleChange2 = new Vector3(-2f, -2f, -2f);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.transform.localScale += scaleChange;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.transform.localScale += scaleChange2;
        }

    }
}
