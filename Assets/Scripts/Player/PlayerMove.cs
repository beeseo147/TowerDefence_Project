using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;//�̵� �ӵ�
    CharacterController cc;
    public GameObject inventoryUI;
    public float jumpPower = 5; //���� �Ŀ�
    bool isJumping = false;

    public float gravity = -20f; //�߷� ���ӵ�
    float yVelocity = 0; //���� �ӵ�
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        inventoryUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //����� �Է��� �޴´�
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");
        //������ ���Ѵ�
        Vector3 dir = new Vector3(h, 0, v);
        //����ڰ� �ٶ󺸴� �������� ��ȯ
        dir = Camera.main.transform.TransformDirection(dir);

        //�߷��� ������ ���� ���� �߰�
        yVelocity += gravity * Time.deltaTime;
        if(cc.isGrounded) //�ٴڿ� ���� ���, y�ӵ��� 0����..
        {
            yVelocity = 0;
            isJumping = true;
        }
       
        if (ARAVRInput.GetDown(ARAVRInput.Button.Two,
            ARAVRInput.Controller.RTouch) && isJumping)
        {
            yVelocity = jumpPower;
            isJumping = false;
        }
     
        dir.y = yVelocity;
        //�̵��� �Ѵ�.
        cc.Move(dir * speed * Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Tab) && inventoryUI.activeSelf == false)
        {
            OnOpenInventory();
        }
        if(Input.GetKeyDown(KeyCode.Escape) && inventoryUI.activeSelf == true)
        {
            OnCloseInventory();
        }
    }
    void OnOpenInventory()
    {
        //�κ��丮 Ȱ��ȭ�� ���� ���� 
        inventoryUI.SetActive(true);
        Time.timeScale = 0;
    }
    void OnCloseInventory()
    {
        inventoryUI.SetActive(false);
        Time.timeScale = 1;
    }
}
