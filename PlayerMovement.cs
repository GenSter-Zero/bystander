using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����ƶ�����
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Collider Collider;

    [Header("�ƶ�����")]
    public Vector3 moveDirction;//�����ƶ���������
    public Vector3 jumpDirction;//������Ծ��������
    public float playerSpeed;//�����ٶ�
    public float runSpeed = 25f;//�����ٶ�
    public float walkSpeed = 15f;//�ƶ��ٶ�
    public float jumpSpeed = 5f;//��Ծ�е��ƶ��ٶ�
    public float jumpHeight = 10f;//��Ծ�߶�
    public float gravity = -40f;//����

    [Header("�ж�����")]
    public bool isRun;//shift�ж�
    public bool isW;//ǰ���ж�
    public bool isJump;//��Ծ�ж�
    public bool isRunning;//�����ж�
    public bool isWalk;//�ƶ��ж�
    public bool isGrounded;//�������ж�

    [Header("�������")]
    public Transform groundCheck;//�������
    public LayerMask groundMask;//�������
    private float groundDistance = 0.1f;//������뾶
    public float slopeForce = 6.0f;//б��ʱ����
    public float slopeForceLength = 2.0f;//б��ʱ���߳���

    [Header("��������")]
    [SerializeField]
    private AudioSource audioSource;
    public AudioClip walkingSound;
    public AudioClip runingSound;

    [Header("��λ����")]
    [SerializeField]
    [Tooltip("���ܰ���")] public KeyCode runInputName = KeyCode.LeftShift;

    private void Start()
    {
        //��ȡplayer���ϵ�characterController���
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();     
    }

    private void Update()
    {
        CheckGround();
        Movement();
    }

    public void Movement()//�ƶ�
    {
        float h = Input.GetAxisRaw("Horizontal");//��ǰ˵�����ǵ�+raw
        float v = Input.GetAxisRaw("Vertical");
        isRun = Input.GetKey(runInputName);//������
        isW = Input.GetKey(KeyCode.W);
        isRunning = isRun && isW;//����
        isWalk = (Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0) ? true : false;//��·

        playerSpeed = (isRunning) ? runSpeed : walkSpeed;//��Ԫ�������������ifelse����Ӧ��ֻ��wǰ��ʱ����
        moveDirction = (transform.right * h + transform.forward * v).normalized;//���������ƶ����򲢱�׼��Ϊ1
        Jump();

        if (isGrounded && jumpDirction.y<0)//���ϸ�������
        {
            jumpDirction.y = -40f;    
        }
 
        jumpDirction.y += gravity * Time.deltaTime;//���ڵ���͸�������
        characterController.Move((moveDirction * playerSpeed + jumpDirction) * Time.deltaTime);//���ƶ��ٶ��뷽���Ͻ����ƶ�
        PlayerSound();

        //�����б���˶�
        //if (OnSlpe()){characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);}

        //moveDirction = (transform.right * h + transform.forward * v).normalized;//���������ƶ����򲢱�׼��Ϊ1
        //characterController.Move(moveDirction * playerSpeed * Time.deltaTime);//���ƶ��ٶ��뷽���Ͻ����ƶ�
        //Jump();
        //characterController.Move(jumpDirction * Time.deltaTime);
    }

    public void Jump()//��Ծ
    {
        isJump = Input.GetButtonDown("Jump");//������
        if (isJump && isGrounded)
        {
            jumpDirction.y = Mathf.Sqrt(jumpHeight * -2f * gravity);//��������С�ӣ��߶����ٶȵĹ�ϵ��
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
    public void CheckGround()//������
    {
        //�ڴ˵㴴��һ���뾶ָ�������棬��֮ǰ�����߼�ⲻͬ�������mask����κζ���������ײ����Ϊtrue
        isGrounded = Physics.CheckSphere(groundCheck.position,groundDistance,groundMask);
    }

    public bool OnSlpe()//б���жϣ���ʱ�ȷ���
    {
        if (isJump)
        { return false; }
            
        RaycastHit hit;//����bool����

        //���µ����߼���Ƿ���б��
        if(Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2 * slopeForceLength))
        {
            if(hit.normal != Vector3.up)
            {
                return true;
            }
        }
        //float slopeAngle = Vector3.Angle(hit.normal, transform.forward) - 90f;����ǽǶ��жϣ��Ժ����
        //������ʲô�÷�

        return false;
    }

    /// <summary>
    /// �Ż���
    /// ����ʱ��
    /// �ƶ��Ż�
    /// </summary>
}
