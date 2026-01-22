using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TimerManager : MonoBehaviour
{
    public float timeRemaining = 30f; // 制限時間
    public TextMeshProUGUI timerText;
    private bool isGameActive = true;

    void Update()
    {
        if (isGameActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                isGameActive = false;
                EndGame();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timerText.text = "TIME: " + Mathf.CeilToInt(timeToDisplay).ToString();
    }

    void EndGame()
    {
        timerText.text = "FINISH!";

        // 1. 全プレイヤーの動きを止める
        var players = Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            p.DeactivateInput();
        }

        // 2. アイテムが降ってくるのを止める
        var spawner = Object.FindFirstObjectByType<ItemSpawner>();
        if (spawner != null)
        {
            spawner.enabled = false;
            spawner.CancelInvoke("Spawn");
        }

        // --- ★ここが重要：ScoreManagerを見つけて結果発表を命令する ---
        ScoreManager sm = Object.FindFirstObjectByType<ScoreManager>();
        if (sm != null)
        {
            sm.ShowResult();
        }

        Debug.Log("ゲーム終了！リザルトを表示します。");
    }
}