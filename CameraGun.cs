using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 摄像机的旋转
/// 玩家视角旋转联动
/// 摄像机垂直旋转上下旋转
/// </summary>
public class CameraGun : MonoBehaviour
{
    public float spinSensitivity = 100f;//视觉灵敏度
    public Transform playerPosition;//玩家当前位置
    private float Yratation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * spinSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * spinSensitivity * Time.deltaTime;

        Yratation -= mouseY; //因为垂直视角并不应该是瞬间给予，而是需要累计加
        Yratation = Mathf.Clamp(Yratation, -89f, 89f);//限定可视范围的，避免人物异种化（向上抬头90，向下低头90）
        transform.localRotation = Quaternion.Euler(Yratation, 0f, 0f);//玩家纵向旋转控制，其实也就是摄像机的变化？这里存疑
        playerPosition.Rotate(Vector3.up * mouseX);//up是坐标轴（0,1,0）的简写，实际控制的是玩家的横向旋转
    }

}
