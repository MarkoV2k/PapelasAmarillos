using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxis("Horizontal");

        direction.x = hInput * speed;

        animator.SetFloat("speed", Mathf.Abs(hInput));

        bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
        animator.SetBool("isGrounded", isGrounded);


        if (isGrounded)
        {
            direction.y = 0;

            doubleJump = true;

            if (Input.GetButtonDown("Jump"))
            {
                direction.y = jumpForce;
            }
        }
        else
        {
            direction.y += gravity * Time.deltaTime;

            if (doubleJump & Input.GetButtonDown("Jump"))
            {
                //animator.SetTrigger("doubleJump");
                direction.y = jumpForce;
                doubleJump = false;
            }
        }

        if(hInput != 0)
        {
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(hInput, 0, 0));
            model.rotation = newRotation;
        }

        controller.Move(direction * Time.deltaTime);
    }
}