using UnityEngine;
using UnityEngine.UI; // 이미지 제어를 위해 추가
using TMPro;
using System.Collections;

public class ResultUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Rank Settings")]
    [SerializeField] private Image rankImage; // 랭크 이미지가 표시될 UI Image
    [SerializeField] private Sprite[] rankSprites; // 등급별 스프라이트 (0: S, 1: A, 2: B...)

    [SerializeField] private OperatorData operatorData;
    [SerializeField] private float countDuration = 2f;

    private void Start()
    {
        DisplayResults();
    }

    private void DisplayResults()
    {
        var data = GameSessionManager.Instance;
        if (data == null) return;

        bool success = data.isExtracted;
        statusText.text = success ? ">>>successful run<<<" : "<<<critical death>>>";

        if (operatorData != null)
        {
            string[] pool = success ? operatorData.successComments : operatorData.kiaComments;
            commentText.text = pool[Random.Range(0, pool.Length)];
        }

        Rank type = CalculateRank(data.score, data.kills, data.playTime);
        SetRankImage(type);

        StartCoroutine(CountStatRoutine(killsText, "킬: ", 0, data.kills));
        StartCoroutine(CountStatRoutine(scoreText, "점수: ", 0, (int)data.score));
        StartCoroutine(CountStatRoutine(damageText, "받은 데미지: ", 0, (int)data.damageTaken));

        if (timeText != null)
            timeText.text = "총 플레이 시간: " + data.GetFormattedTime();

        data.StopSession();
    }

    public enum Rank { S, A, B, C, F }

    private Rank CalculateRank(float score, int kills, float time)
    {
        if (score >= 200000 && kills >= 40 && time < 300) return Rank.S;
        if (score >= 100000 && kills >= 15) return Rank.A;
        if (score >= 50000 && kills >= 10) return Rank.B;
        if (score >= 25000 && kills >= 5) return Rank.C;
        return Rank.F;
    }

    private void SetRankImage(Rank rank)
    {
        if (rankImage == null || rankSprites.Length == 0) return;

        int index = (int)rank;
        if (index < rankSprites.Length)
        {
            rankImage.sprite = rankSprites[index];
            rankImage.gameObject.SetActive(true);
        }
    }

    private IEnumerator CountStatRoutine(TextMeshProUGUI targetText, string label, int start, int end)
    {
        float elapsed = 0f;
        while (elapsed < countDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countDuration;
            int current = (int)Mathf.Lerp(start, end, t);
            targetText.text = label + current.ToString();
            yield return null;
        }
        targetText.text = label + end.ToString();
    }
}