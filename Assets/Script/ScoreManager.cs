using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [Header("4人分のスコアテキストを順番に登録")]
    public TextMeshProUGUI[] scoreTexts;

    [Header("結果表示用のテキスト")]
    public TextMeshProUGUI resultText;

    [SerializeField] private int[] scores = new int[4];
    private bool isGameOver = false;

    // ★追加：リザルト表示後の入力禁止タイマー
    private float inputLockTimer = 0f;
    private const float LOCK_DURATION = 3.0f;

    void Start()
    {
        if (resultText != null) resultText.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++) UpdateScoreText(i);
    }

    public void AddScore(int playerIndex, int amount)
    {
        if (isGameOver) return;
        if (playerIndex >= 0 && playerIndex < scores.Length)
        {
            scores[playerIndex] += amount;
            UpdateScoreText(playerIndex);
        }
    }

    void UpdateScoreText(int index)
    {
        if (index < scoreTexts.Length && scoreTexts[index] != null)
        {
            scoreTexts[index].text = "P" + (index + 1) + ": " + scores[index];
        }
    }

    public void ShowResult()
    {
        if (isGameOver) return;
        isGameOver = true;

        // ★リザルト開始と同時に3秒タイマーをセット
        inputLockTimer = LOCK_DURATION;

        // --- 勝利判定処理 ---
        int winnerIndex = 0;
        int maxScore = -1;
        bool tie = false;

        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] > maxScore)
            {
                maxScore = scores[i];
                winnerIndex = i;
                tie = false;
            }
            else if (scores[i] == maxScore && maxScore != 0)
            {
                tie = true;
            }
        }

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            string message = (maxScore == 0) ? "DRAW!" : (tie ? "TIE GAME!" : $"PLAYER {winnerIndex + 1} WINS!");

            // ★最初は「Wait...」などを出しておくと親切です
            resultText.text = $"{message}\n<size=30>Please wait...</size>";
        }
    }

    void Update()
    {
        if (!isGameOver) return;

        // ★タイマーが0より大きい間は、カウントダウンしてここで処理を終了する
        if (inputLockTimer > 0)
        {
            inputLockTimer -= Time.deltaTime;
            return; // 下の Input.anyKeyDown は実行されません
        }

        // ★3秒経過したらテキストを更新して入力を受け付ける
        if (resultText != null && !resultText.text.Contains("Press Any Key"))
        {
            // メッセージの後半を「Press Any Key」に差し替え
            string currentMainMessage = resultText.text.Split('\n')[0];
            resultText.text = $"{currentMainMessage}\n<size=30>Press Any Key to Title</size>";
        }

        if (Input.anyKeyDown)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                if (p != null) Destroy(p.transform.root.gameObject);
            }
            SceneManager.LoadScene("Title");
        }
    }

    // ScoreManager.cs の中に以下の関数を書き足してください
    public void SubtractScore(int playerIndex, int amount)
    {
        if (isGameOver) return;
        if (playerIndex >= 0 && playerIndex < scores.Length)
        {
            scores[playerIndex] -= amount;

            // スコアがマイナスにならないようにガード（任意）
            if (scores[playerIndex] < 0) scores[playerIndex] = 0;

            UpdateScoreText(playerIndex);
        }
    }
}