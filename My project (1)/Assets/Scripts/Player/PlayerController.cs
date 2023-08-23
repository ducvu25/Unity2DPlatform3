using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum action
{
    idle,
    walk,
    run,
    jump,
    fell,
    touchWall
}
public class PlayerController : MonoBehaviour
{
    [Header("-------Information-------")]
    [SerializeField] PlayerSO playerInformation;
    [SerializeField][Range(1, 3)] int numberJump;

    float speed;
    float hp;
    float mp;
    float dame;
    float jumpFoce;
    float[] time_spawn;

    [Header("-------Check collider-------\n")]


    [Header("------Ground-----")]
    [SerializeField] LayerMask lmGround;
    [SerializeField] Transform pointDown;
    [SerializeField] float pointDownRadius = 0.5f;
    bool isGround;
    bool isWater;
    bool canJump;
    int m_numberJump;

    [Header("------Wall-----")]
    [SerializeField] LayerMask lmWall;
    [SerializeField] Transform pointWall;
    [SerializeField] float pointWallDistance = 0.1f;
    [SerializeField] float wallSpeed;
    bool isTochingWall;

    bool facingRight;

    [SerializeField] float forceAir = 10f;

    float autoValue;
    Rigidbody2D rg;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        hp = playerInformation.Hp;
        mp = playerInformation.Mp;
        dame = playerInformation.Dame;
        jumpFoce = playerInformation.JumpFoce;
        speed = 0;
        time_spawn = new float[playerInformation.TimeSpawn.Length];
        for (int i = 0; i < playerInformation.TimeSpawn.Length; i++)
            time_spawn[i] = 0;

        facingRight = true;

        rg = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        this.CheckCollider();
        this.Run();
        this.Jump();
        this.Attack();
        this.UpdateAnimation();
    }
    void CheckCollider()
    {
        isTochingWall = Physics2D.Raycast(pointWall.position, transform.right* (facingRight ? 1 : -1) , pointWallDistance, lmWall);

        if (Physics2D.OverlapCircle(pointDown.position, pointDownRadius, lmGround))
            isGround = true;
        else
            isGround = false;

        if (isGround || isTochingWall)
        {
            canJump = true;
            m_numberJump = numberJump;
        }
        else
        {
            if (m_numberJump > 0)
                canJump = true;
            else
                canJump = false;
        }
    }
    void Run()
    {
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (facingRight)
                Filip();
            this.AddMovement(-1);
            
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (!facingRight)
                Filip();
            this.AddMovement(1);
        }
        else
        {
            rg.velocity = new Vector2(0, rg.velocity.y);
            speed = 0;
        }
        if(isTochingWall && rg.velocity.y < -wallSpeed)
        {
            rg.velocity = new Vector2(rg.velocity.x, -wallSpeed);
        }
    }
    void AddMovement(int value)
    {
        if (isGround)
        {
            rg.velocity = new Vector2(value*speed, rg.velocity.y);
            if (speed < playerInformation.Speed * 1.3f)
                speed += playerInformation.Speed / 8;
        }else if(!isGround && !isTochingWall)
        {
            Vector2 forceToAdd = new Vector2(value * forceAir, 0);
            if(Mathf.Abs(rg.velocity.x) < forceAir * 1.2f)
                rg.AddForce(forceToAdd);
        }
    }
    void Jump()
    {
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow)) && canJump)
        {
            if (!isTochingWall)
            {
                rg.velocity = new Vector2(rg.velocity.x, jumpFoce);
                m_numberJump--;
            }else 
            {
                autoValue = jumpFoce*5;
                rg.velocity = new Vector2(rg.velocity.x, jumpFoce*0.75f);
                Filip();
            }
        }
        if(autoValue > 1)
        {
            rg.AddForce(new Vector2(autoValue * (facingRight ? 1 : -1), 0));
            autoValue *= 0.9f;
            Debug.Log(autoValue);
        }
        
       /* if((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow)) && canJump)
        {
            rg.velocity = new Vector2(rg.velocity.x, rg.velocity.y *1.5f);
        }*/
    }
    void Attack()
    {
        KeyCode[] input = { KeyCode.Q, KeyCode.W };
        int[] attack = { -1, 1 };
        for(int i=0; i<time_spawn.Length; i++)
        {
            if (time_spawn[i] > 0)
                time_spawn[i] -= Time.deltaTime;
            else if (Input.GetKeyDown(input[i]))
            {
                animator.SetTrigger("Attack");
                animator.SetFloat("Attack Type", attack[i]);
                time_spawn[i] = playerInformation.TimeSpawn[i];
            }
        }
    }
    void Filip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    void UpdateAnimation()
    {
        action state = action.idle;
        if(speed > 0.1f)
        {
            if (speed > playerInformation.Speed * 0.75f)
                state = action.run;
            else
                state = action.walk;
        }
        if (rg.velocity.y > 0.1f)
        {
            state = action.jump;
            if (isTochingWall)
                state = action.touchWall;
        }
        else if (rg.velocity.y < -0.1f)
        {
            state = action.fell;
            if (isTochingWall)
                state = action.touchWall;
        }
        animator.SetInteger("State", (int)state);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointDown.position, pointDownRadius);
        Gizmos.DrawLine(pointWall.position, new Vector3(pointWall.position.x + pointWallDistance, pointWall.position.y, pointWall.position.z));
    }
}
