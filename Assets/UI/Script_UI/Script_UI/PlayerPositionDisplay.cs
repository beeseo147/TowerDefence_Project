using UnityEngine;
using UnityEngine.UI;

// �ۼ��� : ���ֿ�
// �÷��̾� ��ġ ǥ�� Ŭ����
// ��� : �÷��̾� ��ġ ǥ��, �÷��̾� ��ġ ǥ�� �Ӽ� ����, �÷��̾� ��ġ ǥ�� �Ӽ� ��ȯ
public class PlayerPositionDisplay : MonoBehaviour
{
    [Header("UI ????")]
    [SerializeField] private Text positionText;

    [Header("?��???? ????")]
    [SerializeField] private Transform playerTransform;

    [Header("??? ????")]
    [SerializeField] private bool showDecimalPlaces = true;
    [SerializeField] private int decimalPlaces = 1;

    // �÷��̾� ��ġ ǥ�� ����
    private void Start()
    {
        // Text ������Ʈ�� ������ ã��
        if (positionText == null)
        {
            positionText = GetComponent<Text>();
        }

        // Text ������Ʈ�� ������ ��� �޽��� ���
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
                // �Ҽ��� 1�ڸ����� ǥ��
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
            positionText.text = "�÷��̾� ��ġ ������ ǥ���� �� �����ϴ�.";
        }
    }

    // Inspector���� �÷��̾� ��ġ ������ ǥ���� �� �����ϴ�.
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    // �Ҽ��� ǥ�� ���
    public void ToggleDecimalPlaces()
    {
        showDecimalPlaces = !showDecimalPlaces;
    }
}