using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class MaintainPlayers : MonoBehaviour
{
    private static MaintainPlayers instance;
    private PlayerInputManager inputManager;

    private void Awake()
    {
        // 二重生成防止
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        inputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // タイトルシーンに戻った時だけリセットを実行
        if (scene.name == "Title")
        {
            StartCoroutine(ResetInputManagerCoroutine());
        }
    }

    private IEnumerator ResetInputManagerCoroutine()
    {
        if (inputManager != null)
        {
            // 一旦無効にする（これで内部のプレイヤー情報がクリアされる）
            inputManager.enabled = false;

            // システムが落ち着くまで1フレーム待つ
            yield return null;

            // 再び有効にする（これで新しい参加を受け付けられるようになる）
            inputManager.enabled = true;

            Debug.Log("PlayerInputManagerを再起動しました。参加可能です。");
        }
    }
}