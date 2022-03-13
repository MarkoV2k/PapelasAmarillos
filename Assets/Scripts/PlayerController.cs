using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 direction;
    private Vector3 sizeOfPlayer;
    public float speed = 8;
    public float jumpForce = 10;
    private float gravity = -15;
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
        // Lock rotation of rb 
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

        // Tether indicator 
        gos = GameObject.FindGameObjectsWithTag("Tether Points");
        anchor = GameObject.Find("Anchor");
        tetherTransform = FindClosestTetherPoint(gos).transform; // Get the closest tether transform
        indicatorGameObject = GameObject.Find("Indicator"); // Find Game Object "Indicator"
        indicatorSphere = GameObject.Find("IndicatorSphere"); // Find Game Object "Indicator"
        indicatorGameObject.transform.position = new Vector3(tetherTransform.position.x, tetherTransform.position.y + indicatorOffset, 0);

        // Sound
        connectSound = GetComponent<AudioSource>();
        soundplayed = false;

        // Misc
        lr = GetComponent<LineRenderer>();
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

        // Hold
        if (Input.GetMouseButton(0))
        {
            DoSwingAction();
        }
        // Release
        else
        {
        {
            DoFallingAction();
        }
        // Update rope positions
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, anchor.transform.position);
    }

    void DoSwingAction()
    {
        // Calculate angle between player and tether point and rotate the player around it
        var dir = (tetherTransform.position - transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        // Fire Rope (Extra check, every thing here runs only once per tethering action) 
        if (Input.GetMouseButtonDown(0))
        {
            // Get the vector to the closest Tether point as long as there are points

            closestTether = FindClosestTetherPoint(gos).transform.position - transform.position;

            // Move pressed indicator position to tetherTransform pos
            indicatorGameObject.transform.position = new Vector3(tetherTransform.position.x, tetherTransform.position.y + indicatorPressedOffset, 0);
            indicatorSphere.transform.position = new Vector3(tetherTransform.position.x, tetherTransform.position.y, 0);

            // Shoot a ray out towards that position       
            LayerMask ignorePlayer = ~(1 << LayerMask.NameToLayer("Player"));
            Physics.Raycast(transform.position, closestTether, out RaycastHit hit, maxTetherDistance, ignorePlayer);
            if (hit.collider)
            {
                if (hit.collider.tag == "Tether Points")
                {
                    // Move the anchor to the correct position
                    anchor.transform.position = new Vector3(hit.point.x, hit.point.y, 0);
                    // Zero out any rotation of anchor
                    anchor.transform.rotation = Quaternion.identity;

                    // Create HingeJoints
                    joint = gameObject.AddComponent<HingeJoint>();
                    joint.axis = Vector3.forward;
                    joint.anchor = Vector3.zero;
                    joint.connectedBody = anchor.GetComponent<Rigidbody>();

                    // Create anchor HingeJoint
                    anchorJoint = anchor.AddComponent<HingeJoint>();
                    anchorJoint.axis = Vector3.forward;
                    anchorJoint.anchor = Vector3.zero;
                    lr.enabled = true; // show rope

                    // Play connect sound
                    if (!soundplayed)
                    {
                        connectSound.Play(0);
                        soundplayed = true;
                    }
                }
            }
        }
    }

    void DoFallingAction()
    {
        // Keep updating position of closest while flying as long as we find tether points
        if (FindClosestTetherPoint(gos) != null)
        {
            tetherTransform = FindClosestTetherPoint(gos).transform;
        }

        // Move indicator to the closest tether point
        indicatorGameObject.transform.position = new Vector3(tetherTransform.position.x, tetherTransform.position.y + indicatorOffset, 0f);
        // Move sphere away from screen
        indicatorSphere.transform.position = new Vector3(0f, -1.0f, 0f);

        // Update player rotation while flying
        Vector3 direction = rb.velocity.normalized;
        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ - (360.0f));

        // Called only once
        if (Input.GetMouseButtonUp(0))
        {
            releaseSound.Play(0);
            // Disable sound
            soundplayed = false;
            // Destroy HingeJoints
            Destroy(joint);
            Destroy(anchorJoint);
            // Hide rope
            lr.enabled = false;
        }
    }

    GameObject FindClosestTetherPoint(GameObject[] gos)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;

        //Player position
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            // Get vector from Tether point to Player
            Vector3 diff = go.transform.position - position;
            // Get distance value of this vector
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}