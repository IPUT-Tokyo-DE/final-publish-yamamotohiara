using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        // 自分のオブジェクトについているマネージャーを取得
        PlayerInputManager manager = GetComponent<PlayerInputManager>();

        if (PlayerDataManager.Instance != null && manager != null)
        {
            Debug.Log($"保存された {PlayerDataManager.Instance.joinedDevices.Count} 人のプレイヤーを生成します");

            // タイトルでメモしたデバイスを一人ずつ取り出す
            foreach (var device in PlayerDataManager.Instance.joinedDevices)
            {
                // そのデバイス（コントローラー）を使ってプレイヤーを生成！
                manager.JoinPlayer(pairWithDevice: device);
            }
        }
        else
        {
            Debug.LogError("PlayerDataManagerが見つからないか、Managerがありません！");
        }
    }
}