using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class BattleStarter : MonoBehaviour
{
    [Header("四隅の開始地点を順番に登録")]
    public Transform[] spawnPoints;

    IEnumerator Start()
    {
        // 1. PlayerSpawnerが生成を終えるまで、ほんの少しだけ待つ
        yield return new WaitForSeconds(0.1f);

        // 2. 現在のシーンにいるPlayerInputをすべて取得
        PlayerInput[] allPlayers = Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        // プレイヤーインデックス（1P, 2P...）順に並び替える
        List<PlayerInput> sortedPlayers = new List<PlayerInput>(allPlayers);
        sortedPlayers.Sort((a, b) => a.playerIndex.CompareTo(b.playerIndex));

        Debug.Log("配置対象のプレイヤー数: " + sortedPlayers.Count);

        // 3. 配置と色塗りの実行
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            if (i < spawnPoints.Length)
            {
                GameObject pObj = sortedPlayers[i].gameObject;

                // --- ワープ処理 ---
                Rigidbody rb = pObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true; // 位置移動中は物理を止める
                }

                pObj.transform.position = spawnPoints[i].position;
                pObj.transform.rotation = spawnPoints[i].rotation;

                if (rb != null)
                {
                    rb.isKinematic = false; // 動けるようにする
                }

                // --- 色塗りと見た目の更新 ---
                int pIndex = sortedPlayers[i].playerIndex;
                pObj.SendMessage("SetPlayerColor", pIndex, SendMessageOptions.DontRequireReceiver);
                pObj.SendMessage("UpdateVisibility", SendMessageOptions.DontRequireReceiver);

                // 入力を有効化
                sortedPlayers[i].ActivateInput();

                Debug.Log($"Player {pIndex} を {spawnPoints[i].name} に配置しました");
            }
        }
    }
}