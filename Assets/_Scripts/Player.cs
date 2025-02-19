﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    //Movement
    public float MoveSpeed = 10;
    public float TurnRate = 2f;

    //For animation
    Animator animator;
    int status = 0;
    int IDLE = 0;
    int RUNNING = 1;
    int RUNNING_BACKWARD = 2;

    //Bullet asset
    public GameObject Bullet;
    public float BulletForce = 1f;

    //Attached camera
    private Camera playerCam;

    //Scoring
    public Text ScoreBoard;
    private int GOAL_SCORE = 200;

    //Jumping
    public float jumpSpeed = 10f;
    public bool isGrounded;
    Rigidbody rb;

    // better jump
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCam = GetComponentInChildren<Camera>();
        animator = GetComponent<Animator>();
        status = IDLE;
        ScoreManager.SetScore(0);
        rb = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        //Animations
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (vertical == 1)
        {
            if (status != RUNNING)
            {
                animator.SetTrigger("Run");
                status = RUNNING;
            }
        }
        else if (vertical == -1)
        {
            if (status != RUNNING_BACKWARD)
            {
                animator.SetTrigger("RunBack");
                status = RUNNING_BACKWARD;
            }
        }
        else if (horizontal != 0)
        {
            if (status != RUNNING)
            {
                animator.SetTrigger("Run");
                status = RUNNING;
            }
        }
        else
        {
            if (status != IDLE)
            {
                animator.SetTrigger("Idle");
                status = IDLE;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        //FPS Controller movement
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * MoveSpeed * Time.deltaTime, Space.Self);

        //Mouse movement
        Vector2 md = new Vector2(Input.GetAxisRaw("Mouse X") * TurnRate, Input.GetAxisRaw("Mouse Y") * TurnRate);
        transform.Rotate(new Vector3(0f, md.x, 0f));
        playerCam.transform.Rotate(new Vector3(-md.y, 0f, 0f));
        playerCam.transform.rotation.eulerAngles.Set(0f, Mathf.Clamp(transform.rotation.eulerAngles.y, -90f, 90f), 0f);


        // jump improve
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        //Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, 1, 0) * jumpSpeed, ForceMode.Impulse);
            isGrounded = false;
        }

        //Shooty bits
        if (Input.GetMouseButtonDown(0)) //button 0 is left click and 1 is right click
        {
            Vector3 spanwnBullet = transform.position + (transform.forward * 1.3f);
            spanwnBullet.y += 0.5f;
            GameObject temp = Instantiate(Bullet, spanwnBullet, transform.rotation);
            temp.GetComponent<Rigidbody>().velocity = transform.forward * BulletForce;
        }

    }

    public void AddKillScore(int SCORE_PER_KILL)
    {
        ScoreManager.AddScore(SCORE_PER_KILL);
        ScoreBoard.text = "Score: " + ScoreManager.GetScore();
        if(ScoreManager.GetScore() >= GOAL_SCORE)
        {
            SceneManager.LoadScene(3);
        }
    }

    public void ResetScore()
    {
        ScoreManager.SetScore(0);
        ScoreBoard.text = "Score: " + ScoreManager.GetScore();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Hit something");
        if (collision.gameObject.tag.Equals("Zombie"))
        {
            Debug.Log("Died");
            SceneManager.LoadScene(2);
        }

        if (collision.gameObject.tag.Equals("Ground") && isGrounded == false)
        {
            isGrounded = true;
        }
    }

}
