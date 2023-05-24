using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerController : MonoBehaviour
{
    private float speed = 6;
    public float jumpForce;
    public bool doubleJump;

    private Rigidbody2D rb;
    private SpriteRenderer rbSprite;
    private bool playerInFloor = false;
    private Animator ani;
    private BoxCollider2D playerCollider;
    private int contabilizaDano = 0;
    private int contabilizaColeta = 0;

    public Transform detectFloor;
    public LayerMask isFloor;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();
        rbSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (FindObjectOfType<GameController>().GetSavePoint() != Vector3.zero)
        {
            transform.position = FindObjectOfType<GameController>().GetSavePoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump")){
            if (playerInFloor) {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                GameObject.Find("AudioController").GetComponent<AudioController>().Jump();
                playerInFloor = false;
                doubleJump = true;
            } else if (!playerInFloor && doubleJump == true){
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                GameObject.Find("AudioController").GetComponent<AudioController>().Jump();
                playerInFloor = false;
                doubleJump = false;
            }
            ani.SetBool("isJumping", true);
        } else {
            //ani.SetBool("isJumping", false);
        }

        if (Input.GetButtonDown("Ataque1"))
        {
            ani.SetBool("isAtacking", true);
            GameObject.Find("AudioController").GetComponent<AudioController>().Attack();
        }
        else
        {
            ani.SetBool("isAtacking", false);
        }

        if (playerInFloor)
        {
            ani.SetBool("isJumping", false);
        }
    }

    public bool GetStatusInFloor()
    {
        return playerInFloor;
    }

    private void Flip(bool isFliped){
        float scaleX = Mathf.Abs(transform.localScale.x);
        if (isFliped){
            scaleX *= -1;
        }
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    private void FixedUpdate()
    {
        // Movimentação
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector3(speed * horizontal, rb.velocity.y, 0f);
        //bool flip = false;
        if(horizontal > 0){
            ani.SetBool("isRunning", true);
            //GameObject.Find("AudioController").GetComponent<AudioController>().RunningPlay();
            Flip(false);
            //rbSprite.flipX = false;
        }
        else if(horizontal < 0){
            ani.SetBool("isRunning", true);
            //GameObject.Find("AudioController").GetComponent<AudioController>().RunningPlay();
            Flip(true);
            //rbSprite.flipX = true;
        }
        else{
            ani.SetBool("isRunning", false);
            //GameObject.Find("AudioController").GetComponent<AudioController>().RunningBreak();

        }

        if (vertical > 0){
            ani.SetBool("isJumping", true);
        }

        //AtaquePLayer();
        //Pulo Simples
        playerInFloor = Physics2D.OverlapBox(detectFloor.position, new Vector2(0.16739f, 0.16739f), 0f, isFloor);
        /*playerInFloor = Physics2D.OverlapCircle(detectFloor.position, 0.0887f, isFloor);*/
    }

    private void TimeTransitionHurt()
    {
        ani.SetBool("isHurting", false);
        return;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3) // floor
        {
            playerInFloor = true;
            doubleJump = false;
        }

        if (collision.gameObject.tag.Equals("Finish")) {
            FindObjectOfType<GameController>().LevelEnd();
        }

    }
    public void AnimationDeadPlayer()
    {
        ani.SetBool("isDead", true);
    }

    public void AnimationHurtPlayer()
    {
        ani.SetBool("isHurting", true);
        Invoke("TimeTransitionHurt", 0.4f);
    }

    private void Sleep()
    {
        contabilizaDano = 0;
        return;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed()
    {
        speed++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (contabilizaDano == 0)
            {
                Debug.Log("acertou");
                FindObjectOfType<GameController>().HurtEnemy(collision.gameObject);
                contabilizaDano = 1;
                Invoke("Sleep", 1.5f);
            }
        }

        if (collision.gameObject.CompareTag("water"))
        {
            ani.SetBool("isDead", true);
            FindObjectOfType<GameController>().DeadPlayer();
        }

        if (collision.gameObject.CompareTag("savepoint"))
        {
            Debug.Log("Saveee");
            FindObjectOfType<GameController>().SetSafePoint(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("treasureChest"))
        {
            if (Input.GetButtonDown("Open"))
            {
                collision.GetComponent<TreasureChestController>().AnimationOpen();
            }
        }

        if (collision.gameObject.CompareTag("collectibles"))
        {
            if (Input.GetButtonDown("Coleta"))
            {
                Destroy(collision.gameObject);
                if (contabilizaColeta == 0)
                {
                    FindObjectOfType<GameController>().GetCollectibles(collision.gameObject);
                    contabilizaColeta = 1;
                }
                    
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("collectibles"))
        {
            contabilizaColeta = 0;
        }
    }
}
