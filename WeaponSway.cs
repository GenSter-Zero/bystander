using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float amout;//ҡ�ڷ���
    public float smoothAmout;//ҡ��ƽ��ֵ
    public float maxAmout;//���ҡ�ڷ���

    [SerializeField]
    public Vector3 originPosition;//��ȡ�ֱ۳�ʼλ��
    public Vector3 armposition;//�仯���λ��

    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.localPosition;//localposition�ǵ�ǰ�Ӽ�λ�ã��൱�ڸ�������ı任λ�ã�������position���Ǹ���������λ��
    }

    // Update is called once per frame
    void Update()
    {
        //��ȡ�����ֵ���仯ֵ��
        float moveX = -Input.GetAxis("Mouse X") * amout;
        float moveY = -Input.GetAxis("Mouse Y") * amout;
        //����
        Mathf.Clamp(moveX, -maxAmout, maxAmout);
        Mathf.Clamp(moveY, -maxAmout, maxAmout);
        //��ȡ���Ŀ��λ��
        armposition =  new Vector3(moveX, moveY, 0);
        //�ֱ�λ�ñ仯
        //lerp�������Ǵ�a�㵽b�����a+��b-a��*t���㷨��tΪ0������a�㣬tΪ1������b�㣬tΪ0.5������a��b��һ�룬Ҳ����˵
        //��a��b�������Բ�ֵ�仯
        transform.localPosition = Vector3.Lerp(transform.localPosition , armposition + originPosition , Time.deltaTime * smoothAmout);
    }
}
