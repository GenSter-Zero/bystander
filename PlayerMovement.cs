using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩家移动控制
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Collider Collider;

    [Header("移动设置")]
    public Vector3 moveDirction;//设置移动方向向量
    public Vector3 jumpDirction;//设置跳跃方向向量
    public float playerSpeed;//人物速度
    public float runSpeed = 25f;//奔跑速度
    public float walkSpeed = 15f;//移动速度
    public float jumpSpeed = 5f;//跳跃中的移动速度
    public float jumpHeight = 10f;//跳跃高度
    public float gravity = -40f;//重力

    [Header("判断条件")]
    public bool isRun;//shift判断
    public bool isW;//前进判断
    public bool isJump;//跳跃判断
    public bool isRunning;//奔跑判断
    public bool isWalk;//移动判断
    public bool isGrounded;//地面检测判断

    [Header("环境检测")]
    public Transform groundCheck;//地面检测点
    public LayerMask groundMask;//地面检测层
    private float groundDistance = 0.1f;//地面检测半径
    public float slopeForce = 6.0f;//斜坡时力度
    public float slopeForceLength = 2.0f;//斜坡时射线长度

    [Header("声音设置")]
    [SerializeField]
    private AudioSource audioSource;
    public AudioClip walkingSound;
    public AudioClip runingSound;

    [Header("键位设置")]
    [SerializeField]
    [Tooltip("奔跑按键")] public KeyCode runInputName = KeyCode.LeftShift;

    private void Start()
    {
        //获取player身上的characterController组件
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();     
    }

    private void Update()
    {
        CheckGround();
        Movement();
    }

    public void Movement()//移动
    {
        float h = Input.GetAxisRaw("Horizontal");//以前说过，记得+raw
        float v = Input.GetAxisRaw("Vertical");
        isRun = Input.GetKey(runInputName);//按键绑定
        isW = Input.GetKey(KeyCode.W);
        isRunning = isRun && isW;//奔跑
        isWalk = (Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0) ? true : false;//走路

        playerSpeed = (isRunning) ? runSpeed : walkSpeed;//三元运算符，可以用ifelse，但应该只有w前进时加速
        moveDirction = (transform.right * h + transform.forward * v).normalized;//给予物体移动方向并标准化为1
        Jump();

        if (isGrounded && jumpDirction.y<0)//坡上给予重力
        {
            jumpDirction.y = -40f;    
        }
 
        jumpDirction.y += gravity * Time.deltaTime;//不在地面就给予重力
        characterController.Move((moveDirction * playerSpeed + jumpDirction) * Time.deltaTime);//将移动速度与方向结合进行移动
        PlayerSound();

        //如果在斜坡运动
        //if (OnSlpe()){characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);}

        //moveDirction = (transform.right * h + transform.forward * v).normalized;//给予物体移动方向并标准化为1
        //characterController.Move(moveDirction * playerSpeed * Time.deltaTime);//将移动速度与方向结合进行移动
        //Jump();
        //characterController.Move(jumpDirction * Time.deltaTime);
    }

    public void Jump()//跳跃
    {
        isJump = Input.GetButtonDown("Jump");//按键绑定
        if (isJump && isGrounded)
        {
            jumpDirction.y = Mathf.Sqrt(jumpHeight * -2f * gravity);//纳米物理！小子！高度与速度的关系！
        }
    }

    public void PlayerSound()
    {
        if(isGrounded && moveDirction.sqrMagnitude > 0.9f)
        {
            audioSource.clip = isRunning ? runingSound : walkingSound;
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
    public void CheckGround()//地面检测
    {
        //在此点创造一个半径指定的球面，与之前的射线检测不同，如果与mask层的任何东西发生碰撞，都为true
        isGrounded = Physics.CheckSphere(groundCheck.position,groundDistance,groundMask);
    }

    public bool OnSlpe()//斜坡判断，暂时先放着
    {
        if (isJump)
        { return false; }
            
        RaycastHit hit;//射线bool类型

        //向下的射线检查是否是斜坡
        if(Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2 * slopeForceLength))
        {
            if(hit.normal != Vector3.up)
            {
                return true;
            }
        }
        //float slopeAngle = Vector3.Angle(hit.normal, transform.forward) - 90f;这个是角度判断，以后可以
        //看看有什么用法

        return false;
    }

    /// <summary>
    /// 优化：
    /// 土狼时间
    /// 移动优化
    /// </summary>
}
