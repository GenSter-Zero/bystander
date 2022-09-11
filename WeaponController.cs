using System.Collections;
using System.Collections.Generic;
using UnityEngine;//�����ռ�
using UnityEngine.UI;

/// <summary>
/// �������ϵͳ
/// </summary>
public class WeaponController : MonoBehaviour
{
    public PlayerMovement PM;//���ýű�
    private Animator anim;//����״̬��
    private AnimatorStateInfo Animatorinfo;//��ȡ����
    private Camera GunCamera;
    public Transform casingSpawnPoint;//�����׳�λ��
    public Transform casingPrefab;//�ӵ���Ԥ����


    [Header("�������")]
    public Transform shooterPoint;//���λ��
    private Vector3 shootDirection;//�������
    private RaycastHit hit;//�����
    public int buttetsMag = 30;//һ����ϻ�ӵ�����
    public int range = 100;//�������
    public int bulletLeft = 240;//�ӵ�����
    public int currentBullets;//ʣ���ӵ�����
    private bool GunShootInput;//��������ж�
    public float fireRate;//����ٶ�
    private float fireTimer = 0f;//��ʱ��
    private int BullectToload;//������Ҫ��װ�ӵ���

    [Header("�ж�����")]
    private bool noShoot;//��������������ж�
    private bool isAiming;//��׼�ж�

    [Header("UI����")]
    public Image CrossHairUI;//׼��UI
    public Text AmmoTextUI;//�ӵ�����UI
    public Text ShootModelTextUI;//���ģʽUI

    [Header("��λ����")]
    [SerializeField]
    [Tooltip("��������")] private KeyCode reloading = KeyCode.R;//������
    [Tooltip("���Ӱ���")] private KeyCode inspect = KeyCode.N;//���Ӽ�
    [Tooltip("ģʽ�л�����")] private KeyCode swtichModel = KeyCode.B;//���ģʽ�л�����

    [Header("��Ч����")]
    public ParticleSystem muzzleFlash;//ǹ�ڻ�����Ч
    public Light muzzleFlashLight;//ǹ�ڻ���ƹ�
    public GameObject hitParticle;//�ӵ�����������Ч
    public GameObject bullectHole;//����

    [Header("��Ƶ����")]
    [SerializeField]private AudioSource audioSource;
    public AudioClip AK47soundClip;//AK47�����ЧƬ��
    public AudioClip AllReloadingClip;//�ղֻ���
    public AudioClip ReloadingClip;//��ͨ����

    public enum ShootMode { AutoFire , SemiGun };
    public ShootMode shootingMode;
    private int ModeNum = 1;//ģʽ�л����м������1ȫ�Զ���2���Զ�
    private string shootName;

    // Start is called before the first frame update
    void Start()
    {
        GunCamera = Camera.main;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentBullets = buttetsMag;//Ĭ���ӵ�
        shootName = "ȫ�Զ�";
        shootingMode = ShootMode.AutoFire;//Ĭ��ȫ�Զ�ģʽ
        UpdateUI();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(swtichModel) && ModeNum == 0)
        {
            ModeNum = 1;
            shootName = "ȫ�Զ�";
            shootingMode = ShootMode.AutoFire;
            ShootModelTextUI.text = shootName;
        }
        else if(Input.GetKeyDown(swtichModel) && ModeNum == 1)
        {      
            ModeNum = 0;
            shootName = "���Զ�";
            shootingMode = ShootMode.SemiGun;
            ShootModelTextUI.text = shootName;
        }
        
        switch(shootingMode)
        {
            case ShootMode.AutoFire:
                GunShootInput = Input.GetMouseButton(0);
                fireRate = 0.1f;
                break;
            case ShootMode.SemiGun:
                GunShootInput = Input.GetMouseButtonDown(0);
                fireRate = 0.3f;
                break;
        }

        if(GunShootInput && currentBullets > 0)//�������
        {
            GunFire();
        }
        else
        {
            muzzleFlashLight.enabled = false;//��������
        }

        Animatorinfo = anim.GetCurrentAnimatorStateInfo(0);//�������
        if(Animatorinfo.IsName("reload_ammo_left")||Animatorinfo.IsName("reload_out_of_ammo")||Animatorinfo.IsName("take_out"))
        {
            noShoot = true;
        }
        else
        {
            noShoot = false;
        }

        if(Input.GetKeyDown(reloading) && currentBullets < buttetsMag && bulletLeft > 0)//��������
        {
            RelodingAmmo();
        }

        aiming();

        if (Input.GetKeyDown(inspect))//�鿴��������
        {
            anim.SetTrigger("inspect");
        }

        anim.SetBool("Run", PM.isRunning);
        anim.SetBool("Walk", PM.isWalk);
        

        if(fireTimer<fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// ���
    /// </summary>
    public void GunFire()
    {
        //������ٲ�������ô��ֱ��������������������ÿִ֡�����ߴ�������û���ӵ�ֱ�Ӳ�����������
        if (fireTimer<fireRate|| noShoot || PM.isRunning){return;}//���ﻹ���޸ģ�Ӧ������ҿ�ʼ���������Ͳ���������
        shootDirection = shooterPoint.forward;//�������
        if(Physics.Raycast(shooterPoint.position, shootDirection, out hit, range))//���ߵ㣬���߷��򣬴�С
        {
            //ʵ������Ч�������ϴε��Ż������ڴ���������Ч,�������Ҫѧϰŷ������openGL
            GameObject hitParticleEffect = Instantiate(hitParticle, hit.point, Quaternion.FromToRotation(Vector3.up,hit.normal));
            //ʵ����������Ч
            GameObject bullectHoleEffect = Instantiate(bullectHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            //��Ч��ʱ����
            Destroy(hitParticleEffect, 1f);
            Destroy(bullectHoleEffect, 3f);
        }
        if(!isAiming)
        {
            anim.CrossFadeInFixedTime("fire", 0.1f);//δ��׼�µĿ��𶯻�
        }
        else
        {
            anim.CrossFadeInFixedTime("aim_fire", 0.1f);//��׼״̬�¿��𶯻�
        }

        Instantiate(casingPrefab, casingSpawnPoint.transform.position, casingSpawnPoint.transform.rotation);
        GunSound();//���������Ч
        muzzleFlash.Play();//���Ż����Ч
        muzzleFlashLight.enabled = true;//���ŵƹ���Ч
        currentBullets--;
        UpdateUI();
        fireTimer = 0f;//���������������ô���ü�ʱ��
    }
    
    public void UpdateUI()//�ӵ�ʣ����UI
    {
        AmmoTextUI.text = currentBullets + "/" + bulletLeft;
        ShootModelTextUI.text = shootName;
    }
    public void RelodingAmmo()//�����߼�
    {
        if(bulletLeft <= 0)
        {
            return;
        }
        ReloadingAnim();
        //������Ҫ��������
        BullectToload = buttetsMag - currentBullets;
        //��������װ������
        BullectToload = (bulletLeft >= BullectToload) ? BullectToload : bulletLeft;
        //��ϻ���٣���ҩ����
        bulletLeft -= BullectToload;
        currentBullets += BullectToload;
        UpdateUI();//����UI
    }
    public void GunSound()//ǹе��������
    {
        audioSource.clip = AK47soundClip;
        audioSource.Play();
    }
    public void ReloadingAnim()//װ������
    {
        if(currentBullets > 0)
        {
            anim.Play("reload_ammo_left",0,0);//����Ҳ�в�Σ����Ժ����˽�,���һ���ʱ������ǹ
            audioSource.clip = ReloadingClip;
            audioSource.Play();

        }
        if(currentBullets == 0)
        {
            anim.Play("reload_out_of_ammo", 0, 0);
            audioSource.clip = AllReloadingClip;
            audioSource.Play();

        }
    }
    public void aiming()//��׼����
    {
        if(Input.GetMouseButton(1) && !noShoot && !PM.isRunning)
        {
            isAiming = true;
            anim.SetBool("Aim", true);//�����䶯
            CrossHairUI.gameObject.SetActive(false);//׼����ʧ
            GunCamera.fieldOfView = 25;//��׼ʱ��Ұ��С,����������Ұ�����붯���Ż�
        }
        else
        {
            isAiming = false;
            anim.SetBool("Aim", false);
            CrossHairUI.gameObject.SetActive(true);
            GunCamera.fieldOfView = 60;//��׼ʱ��Ұ��С
        }
    }
}
