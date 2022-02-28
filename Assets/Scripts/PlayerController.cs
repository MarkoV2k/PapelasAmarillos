using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 direction;
    private Vector3 sizeOfPlayer;
    public float speed = 8;
    public float jumpForce = 10;
    private float gravity = -20;
    private float gravityOnWall = -5;

    public Transform wallCheck;
    public Transform wallCheckBack;
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

        bool onWall = Physics.CheckSphere(wallCheck.position, 0.05f, groundLayer);
        animator.SetBool("onWall", onWall);

        bool onWallBack = Physics.CheckSphere(wallCheckBack.position, 0.05f, groundLayer);
        animator.SetBool("onWall", onWallBack);


        // STARA WERSJA BEZ SKAKANIA PO ŒCIANACH ALE DZIA£A 100%
        /*if (isGrounded)
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
                
                direction.y = jumpForce;
                doubleJump = false;
                animator.SetTrigger("doubleJump");
            }
        }*/

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
            if (onWall || onWallBack)
            {
                doubleJump = true;
                direction.y += gravityOnWall * Time.deltaTime;
                animator.SetBool("onWall", onWall);
                animator.SetBool("onWall", onWallBack);

                if (doubleJump & Input.GetButtonDown("Jump"))
                {
                    animator.SetTrigger("doubleJump");
                    animator.SetTrigger("climb");
                    direction.y = 5;
                    doubleJump = false;
                }

            }
            else
            {
                direction.y += gravity * Time.deltaTime;

                if (doubleJump & Input.GetButtonDown("Jump"))
                {
                    animator.SetTrigger("doubleJump");
                    //animator.SetTrigger("jumpFromWall");
                    direction.y = jumpForce;
                    doubleJump = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            sizeOfPlayer = transform.localScale;
            if (sizeOfPlayer.x < 9.5f)
            {
                if (gravity > -29)
                    gravity += -1;
                if (speed < 12.5f)
                    speed += 0.5f;
                if (jumpForce > 8.2f)
                    jumpForce += -0.2f;

                sizeOfPlayer.y += 1f;
                sizeOfPlayer.x += 1f;
                sizeOfPlayer.z += 1f;
                transform.localScale = sizeOfPlayer;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            sizeOfPlayer = transform.localScale;
            if (sizeOfPlayer.x > 1f)
            {
                if (gravity < -20)
                    gravity += 1;
                if (speed < 8)
                    speed += -0.5f;
                if (jumpForce < 10)
                    jumpForce += 0.2f;

                sizeOfPlayer.y += -1f;
                sizeOfPlayer.x += -1f;
                sizeOfPlayer.z += -1f;

                transform.localScale = sizeOfPlayer;
            }
        }

        if (hInput != 0)
        {
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(hInput, 0, 0));
            model.rotation = newRotation;
        }

        controller.Move(direction * Time.deltaTime);
    }
}