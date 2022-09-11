using System.Collections;
using System.Collections.Generic;
using UnityEngine;//命名空间
using UnityEngine.UI;

/// <summary>
/// 武器射击系统
/// </summary>
public class WeaponController : MonoBehaviour
{
    public PlayerMovement PM;//引用脚本
    private Animator anim;//动画状态机
    private AnimatorStateInfo Animatorinfo;//读取动画
    private Camera GunCamera;
    public Transform casingSpawnPoint;//弹壳抛出位置
    public Transform casingPrefab;//子弹壳预制体


    [Header("射击设置")]
    public Transform shooterPoint;//射击位置
    private Vector3 shootDirection;//射击向量
    private RaycastHit hit;//射击点
    public int buttetsMag = 30;//一个弹匣子弹数量
    public int range = 100;//武器射程
    public int bulletLeft = 240;//子弹备弹
    public int currentBullets;//剩余子弹数量
    private bool GunShootInput;//射击操作判断
    public float fireRate;//射击速度
    private float fireTimer = 0f;//计时器
    private int BullectToload;//计算需要填装子弹数

    [Header("判断条件")]
    private bool noShoot;//不允许射击条件判断
    private bool isAiming;//瞄准判断

    [Header("UI设置")]
    public Image CrossHairUI;//准心UI
    public Text AmmoTextUI;//子弹备量UI
    public Text ShootModelTextUI;//射击模式UI

    [Header("键位设置")]
    [SerializeField]
    [Tooltip("换弹按键")] private KeyCode reloading = KeyCode.R;//换弹键
    [Tooltip("检视按键")] private KeyCode inspect = KeyCode.N;//检视键
    [Tooltip("模式切换按键")] private KeyCode swtichModel = KeyCode.B;//射击模式切换按键

    [Header("特效设置")]
    public ParticleSystem muzzleFlash;//枪口火焰特效
    public Light muzzleFlashLight;//枪口火焰灯光
    public GameObject hitParticle;//子弹击中粒子特效
    public GameObject bullectHole;//弹孔

    [Header("音频设置")]
    [SerializeField]private AudioSource audioSource;
    public AudioClip AK47soundClip;//AK47射击音效片段
    public AudioClip AllReloadingClip;//空仓换弹
    public AudioClip ReloadingClip;//普通换弹

    public enum ShootMode { AutoFire , SemiGun };
    public ShootMode shootingMode;
    private int ModeNum = 1;//模式切换的中间参数，1全自动，2半自动
    private string shootName;

    // Start is called before the first frame update
    void Start()
    {
        GunCamera = Camera.main;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentBullets = buttetsMag;//默认子弹
        shootName = "全自动";
        shootingMode = ShootMode.AutoFire;//默认全自动模式
        UpdateUI();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(swtichModel) && ModeNum == 0)
        {
            ModeNum = 1;
            shootName = "全自动";
            shootingMode = ShootMode.AutoFire;
            ShootModelTextUI.text = shootName;
        }
        else if(Input.GetKeyDown(swtichModel) && ModeNum == 1)
        {      
            ModeNum = 0;
            shootName = "半自动";
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

        if(GunShootInput && currentBullets > 0)//射击操作
        {
            GunFire();
        }
        else
        {
            muzzleFlashLight.enabled = false;//武器闪光
        }

        Animatorinfo = anim.GetCurrentAnimatorStateInfo(0);//动画检测
        if(Animatorinfo.IsName("reload_ammo_left")||Animatorinfo.IsName("reload_out_of_ammo")||Animatorinfo.IsName("take_out"))
        {
            noShoot = true;
        }
        else
        {
            noShoot = false;
        }

        if(Input.GetKeyDown(reloading) && currentBullets < buttetsMag && bulletLeft > 0)//换弹动画
        {
            RelodingAmmo();
        }

        aiming();

        if (Input.GetKeyDown(inspect))//查看武器动画
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
    /// 射击
    /// </summary>
    public void GunFire()
    {
        //如果射速不到，那么就直接跳出射击代码命令，减少每帧执行射线次数或者没有子弹直接不允许进行射击
        if (fireTimer<fireRate|| noShoot || PM.isRunning){return;}//这里还需修改，应当在玩家开始射击动作后就不允许奔跑了
        shootDirection = shooterPoint.forward;//射击方向
        if(Physics.Raycast(shooterPoint.position, shootDirection, out hit, range))//射线点，射线方向，大小
        {
            //实例化的效果，比上次的优化是能在打击点产生特效,这里后期要学习欧拉角与openGL
            GameObject hitParticleEffect = Instantiate(hitParticle, hit.point, Quaternion.FromToRotation(Vector3.up,hit.normal));
            //实例出弹孔特效
            GameObject bullectHoleEffect = Instantiate(bullectHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            //特效定时回收
            Destroy(hitParticleEffect, 1f);
            Destroy(bullectHoleEffect, 3f);
        }
        if(!isAiming)
        {
            anim.CrossFadeInFixedTime("fire", 0.1f);//未瞄准下的开火动画
        }
        else
        {
            anim.CrossFadeInFixedTime("aim_fire", 0.1f);//瞄准状态下开火动画
        }

        Instantiate(casingPrefab, casingSpawnPoint.transform.position, casingSpawnPoint.transform.rotation);
        GunSound();//播放射击音效
        muzzleFlash.Play();//播放火光特效
        muzzleFlashLight.enabled = true;//播放灯光特效
        currentBullets--;
        UpdateUI();
        fireTimer = 0f;//如果完成了射击，那么重置计时器
    }
    
    public void UpdateUI()//子弹剩余数UI
    {
        AmmoTextUI.text = currentBullets + "/" + bulletLeft;
        ShootModelTextUI.text = shootName;
    }
    public void RelodingAmmo()//换弹逻辑
    {
        if(bulletLeft <= 0)
        {
            return;
        }
        ReloadingAnim();
        //计算需要换弹数量
        BullectToload = buttetsMag - currentBullets;
        //计算真正装填数量
        BullectToload = (bulletLeft >= BullectToload) ? BullectToload : bulletLeft;
        //弹匣减少，弹药补充
        bulletLeft -= BullectToload;
        currentBullets += BullectToload;
        UpdateUI();//更新UI
    }
    public void GunSound()//枪械声音播放
    {
        audioSource.clip = AK47soundClip;
        audioSource.Play();
    }
    public void ReloadingAnim()//装弹动画
    {
        if(currentBullets > 0)
        {
            anim.Play("reload_ammo_left",0,0);//动画也有层次，可以后续了解,并且换弹时不允许开枪
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
    public void aiming()//瞄准动作
    {
        if(Input.GetMouseButton(1) && !noShoot && !PM.isRunning)
        {
            isAiming = true;
            anim.SetBool("Aim", true);//动画变动
            CrossHairUI.gameObject.SetActive(false);//准星消失
            GunCamera.fieldOfView = 25;//瞄准时视野变小,后续进行视野渐变与动画优化
        }
        else
        {
            isAiming = false;
            anim.SetBool("Aim", false);
            CrossHairUI.gameObject.SetActive(true);
            GunCamera.fieldOfView = 60;//瞄准时视野变小
        }
    }
}
