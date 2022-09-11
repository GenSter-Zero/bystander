using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���������ת
/// ����ӽ���ת����
/// �������ֱ��ת������ת
/// </summary>
public class CameraGun : MonoBehaviour
{
    public float spinSensitivity = 100f;//�Ӿ�������
    public Transform playerPosition;//��ҵ�ǰλ��
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

        Yratation -= mouseY; //��Ϊ��ֱ�ӽǲ���Ӧ����˲����裬������Ҫ�ۼƼ�
        Yratation = Mathf.Clamp(Yratation, -89f, 89f);//�޶����ӷ�Χ�ģ������������ֻ�������̧ͷ90�����µ�ͷ90��
        transform.localRotation = Quaternion.Euler(Yratation, 0f, 0f);//���������ת���ƣ���ʵҲ����������ı仯���������
        playerPosition.Rotate(Vector3.up * mouseX);//up�������ᣨ0,1,0���ļ�д��ʵ�ʿ��Ƶ�����ҵĺ�����ת
    }

}
