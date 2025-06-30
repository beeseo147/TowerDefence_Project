using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 작성자 : 공통
//사용하지않음
public class Guage : MonoBehaviour
{
    [SerializeField] Image m_Image;
    [SerializeField] Image m_ShadowImage;

    [SerializeField] int m_ShadowSpeed = 10;
    int m_maxValue;
    int m_value;

    // 게이지 설정
    public void SetGuage(float value, float maxValue)
    {
        if (maxValue == 0) m_Image.fillAmount = 0f;
        else m_Image.fillAmount = maxValue / value;

        if(m_Image.fillAmount > 0.5f)
        {
            m_Image.color = Color.green;
        }
        else
        {
            m_Image.color = Color.red;
        }
    }
    // 게이지 텍스트 설정
    public void SetText(float value)
    {
        
    }
    // 게이지 시작
    void Start()
    {
        SetGuage(100, 100);
    }

    // 게이지 업데이트
    void Update()
    {
        if(m_ShadowImage != null)
        {
            float v = Mathf.Lerp(m_ShadowImage.fillAmount, m_Image.fillAmount, m_ShadowSpeed * Time.deltaTime);
            m_ShadowImage.fillAmount = v;
        }
        //카메라에 대칭으로 회전
        gameObject.transform.rotation = Camera.main.transform.rotation;
    }
}
