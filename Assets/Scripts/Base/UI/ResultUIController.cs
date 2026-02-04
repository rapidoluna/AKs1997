using UnityEngine;
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
        statusText.text = success ? "EXTRACTION SUCCESS" : "PLAYER KILLED IN ACTION";
        statusText.color = success ? Color.cyan : Color.red;

        if (operatorData != null)
        {
            string[] pool = success ? operatorData.successComments : operatorData.kiaComments;
            commentText.text = pool[Random.Range(0, pool.Length)];
        }

        StartCoroutine(CountStatRoutine(killsText, "KILLS: ", 0, data.kills));
        StartCoroutine(CountStatRoutine(scoreText, "SCORE: ", 0, (int)data.score));
        StartCoroutine(CountStatRoutine(damageText, "DAMAGE TAKEN: ", 0, (int)data.damageTaken));
        if (timeText != null)
            timeText.text = "TIME: " + data.GetFormattedTime();

        data.StopSession();
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