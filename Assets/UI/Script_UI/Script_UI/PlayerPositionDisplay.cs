using UnityEngine;
using UnityEngine.UI;

public class PlayerPositionDisplay : MonoBehaviour
{
    [Header("UI 설정")]
    [SerializeField] private Text positionText;

    [Header("플레이어 설정")]
    [SerializeField] private Transform playerTransform;

    [Header("표시 설정")]
    [SerializeField] private bool showDecimalPlaces = true;
    [SerializeField] private int decimalPlaces = 1;

    private void Start()
    {
        // Text 컴포넌트가 할당되지 않았다면 자동으로 찾기
        if (positionText == null)
        {
            positionText = GetComponent<Text>();
        }

        // Text 컴포넌트가 여전히 없다면 경고
        if (positionText == null)
        {
            Debug.LogWarning("PlayerPositionDisplay: Text 컴포넌트를 찾을 수 없습니다. Inspector에서 할당해주세요.");
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
                // 소수점이 0이어도 항상 표시되도록 정확한 포맷팅 사용
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
            positionText.text = "플레이어가 할당되지 않음";
        }
    }

    // Inspector에서 플레이어 할당을 위한 편의 메서드
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    // 소수점 표시 여부 토글
    public void ToggleDecimalPlaces()
    {
        showDecimalPlaces = !showDecimalPlaces;
    }
}