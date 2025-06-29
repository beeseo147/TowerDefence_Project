using UnityEngine;
using UnityEngine.UI;

public class PlayerPositionDisplay : MonoBehaviour
{
    [Header("UI ����")]
    [SerializeField] private Text positionText;

    [Header("�÷��̾� ����")]
    [SerializeField] private Transform playerTransform;

    [Header("ǥ�� ����")]
    [SerializeField] private bool showDecimalPlaces = true;
    [SerializeField] private int decimalPlaces = 1;

    private void Start()
    {
        // Text ������Ʈ�� �Ҵ���� �ʾҴٸ� �ڵ����� ã��
        if (positionText == null)
        {
            positionText = GetComponent<Text>();
        }

        // Text ������Ʈ�� ������ ���ٸ� ���
        if (positionText == null)
        {
            Debug.LogWarning("PlayerPositionDisplay: Text ������Ʈ�� ã�� �� �����ϴ�. Inspector���� �Ҵ����ּ���.");
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
                // �Ҽ����� 0�̾ �׻� ǥ�õǵ��� ��Ȯ�� ������ ���
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
            positionText.text = "�÷��̾ �Ҵ���� ����";
        }
    }

    // Inspector���� �÷��̾� �Ҵ��� ���� ���� �޼���
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    // �Ҽ��� ǥ�� ���� ���
    public void ToggleDecimalPlaces()
    {
        showDecimalPlaces = !showDecimalPlaces;
    }
}