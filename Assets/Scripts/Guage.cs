using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Guage : MonoBehaviour
{
    [SerializeField] Image m_Image;
    [SerializeField] Image m_ShadowImage;

    [SerializeField] int m_ShadowSpeed = 10;
    int m_maxValue;
    int m_value;

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
    public void SetText(float value)
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        SetGuage(100, 100);
    }

    // Update is called once per frame
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
