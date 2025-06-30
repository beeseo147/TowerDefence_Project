using UnityEngine;
using UnityEngine.UI;

// 작성자 : 현주옥
// 플레이어 위치 표시 클래스
// 기능 : 플레이어 위치 표시, 플레이어 위치 표시 속성 설정, 플레이어 위치 표시 속성 반환
public class PlayerPositionDisplay : MonoBehaviour
{
    [Header("UI ????")]
    [SerializeField] private Text positionText;

    [Header("?÷???? ????")]
    [SerializeField] private Transform playerTransform;

    [Header("??? ????")]
    [SerializeField] private bool showDecimalPlaces = true;
    [SerializeField] private int decimalPlaces = 1;

    // 플레이어 위치 표시 시작
    private void Start()
    {
        // Text 컴포넌트가 있으면 찾음
        if (positionText == null)
        {
            positionText = GetComponent<Text>();
        }

        // Text 컴포넌트가 없으면 경고 메시지 출력
        if (positionText == null)
        {
            Debug.LogWarning("PlayerPositionDisplay: Text ????????? ??? ?? ???????. Inspector???? ??????????.");
        }
    }

    private void Update()
    {
        if (playerTransform != null && positionText != null)
        {
            Vector3 position = playerTransform.position;

            string positionString;
            if (showDecimalPlaces)
            {
                // 소수점 1자리까지 표시
                string xStr = position.x.ToString("F1");
                string yStr = position.y.ToString("F1");
                string zStr = position.z.ToString("F1");
                positionString = $"X.{xStr} Y.{yStr} Z.{zStr}";
            }
            else
            {
                positionString = $"X.{Mathf.RoundToInt(position.x)} Y.{Mathf.RoundToInt(position.y)} Z.{Mathf.RoundToInt(position.z)}";
            }

            positionText.text = positionString;
        }
        else if (playerTransform == null)
        {
            positionText.text = "플레이어 위치 정보를 표시할 수 없습니다.";
        }
    }

    // Inspector에서 플레이어 위치 정보를 표시할 수 없습니다.
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    // 소수점 표시 토글
    public void ToggleDecimalPlaces()
    {
        showDecimalPlaces = !showDecimalPlaces;
    }
}