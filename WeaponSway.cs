using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float amout;//摇摆幅度
    public float smoothAmout;//摇摆平滑值
    public float maxAmout;//最大摇摆幅度

    [SerializeField]
    public Vector3 originPosition;//获取手臂初始位置
    public Vector3 armposition;//变化后的位置

    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.localPosition;//localposition是当前子集位置（相当于父级物体的变换位置），但是position就是父级的坐标位置
    }

    // Update is called once per frame
    void Update()
    {
        //获取鼠标轴值（变化值）
        float moveX = -Input.GetAxis("Mouse X") * amout;
        float moveY = -Input.GetAxis("Mouse Y") * amout;
        //限制
        Mathf.Clamp(moveX, -maxAmout, maxAmout);
        Mathf.Clamp(moveY, -maxAmout, maxAmout);
        //获取鼠标目的位置
        armposition =  new Vector3(moveX, moveY, 0);
        //手臂位置变化
        //lerp的作用是从a点到b点进行a+（b-a）*t的算法，t为0，就是a点，t为1，就是b点，t为0.5，就是a到b的一半，也就是说
        //从a到b进行线性插值变化
        transform.localPosition = Vector3.Lerp(transform.localPosition , armposition + originPosition , Time.deltaTime * smoothAmout);
    }
}
