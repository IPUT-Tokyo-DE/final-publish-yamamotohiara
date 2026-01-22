using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    [Header("UI設定")]
    public GameObject[] okTexts;    // 4人分の「OK!」
    public GameObject[] pressTexts; // 4人分の「PRESS ANY BUTTON」
    public GameObject nextButton;   // 「ルールを確認する」ボタン
    public GameObject rulePanel;    // ルール説明パネル

    private int joinedCount = 0;

    void Start()
    {
        nextButton.SetActive(false);
        rulePanel.SetActive(false);

        // シーン開始時に保存データをリセット（2周目対策）
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.ResetData();
        }

        for (int i = 0; i < okTexts.Length; i++)
        {
            okTexts[i].SetActive(false);
            pressTexts[i].SetActive(true);
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // ★重要：デバイス情報を取得
        InputDevice device = playerInput.devices[0];

        // PlayerDataManagerにデバイスを保存
        if (PlayerDataManager.Instance != null)
        {
            // まだ登録されていないデバイスならリストに追加
            if (!PlayerDataManager.Instance.joinedDevices.Contains(device))
            {
                PlayerDataManager.Instance.joinedDevices.Add(device);

                // UIの見た目を更新
                if (joinedCount < okTexts.Length)
                {
                    okTexts[joinedCount].SetActive(true);
                    pressTexts[joinedCount].SetActive(false);
                    joinedCount++;
                }
            }
            else
            {
                // すでに登録済みのデバイス（2回押しなど）なら、生成されたキャラを消す
                Destroy(playerInput.gameObject);
                return;
            }
        }

        if (joinedCount >= 2)
        {
            nextButton.SetActive(true);
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(nextButton);
            }
        }
    }

    public void ShowRules()
    {
        rulePanel.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}