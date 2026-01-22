using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections; // コルーチンを使うために追加

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    private Rigidbody rb;
    private Vector2 inputVector;

    [Header("プレイヤーごとの色（1P, 2P, 3P, 4P）")]
    public Color[] playerColors = { Color.red, Color.blue, Color.yellow, Color.green };

    private ScoreManager scoreManager;

    [Header("見た目の制御用")]
    public GameObject visualModel;

    // ★追加：スタン状態のフラグ
    private bool isStunned = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scoreManager = Object.FindFirstObjectByType<ScoreManager>();

        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        SetPlayerColor(playerIndex);
        UpdateVisibility();
    }

    public void SetPlayerColor(int index)
    {
        Renderer renderer = visualModel?.GetComponentInChildren<Renderer>() ?? GetComponentInChildren<Renderer>();
        if (renderer != null && index < playerColors.Length)
        {
            renderer.material.color = playerColors[index];
        }
    }

    // ★追加：スタン（動けなくなる）命令
    public void Stun(float duration)
    {
        if (isStunned) return; // すでにスタン中なら無視
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        inputVector = Vector2.zero; // 入力をリセット

        // スタン中の見た目演出（例：少しグレーにする、または点滅させるなどはお好みで）
        yield return new WaitForSeconds(duration);

        isStunned = false;
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Title")
        {
            Destroy(gameObject);
            return;
        }

        scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        UpdateVisibility();

        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        SetPlayerColor(playerIndex);
    }

    public void UpdateVisibility()
    {
        if (visualModel == null) return;
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Title")
        {
            visualModel.SetActive(false);
            if (rb != null) rb.isKinematic = true;
        }
        else
        {
            visualModel.SetActive(true);
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    void OnMove(InputValue value)
    {
        if (SceneManager.GetActiveScene().name == "Title") return;
        // ★スタン中は新しい入力を受け付けない
        if (isStunned)
        {
            inputVector = Vector2.zero;
            return;
        }
        inputVector = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        if (rb == null || rb.isKinematic) return;

        // ★スタン状態なら移動処理をスキップ
        if (isStunned)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // 横移動だけ止める
            return;
        }

        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

        // --- 半径5.5の円形ステージ制限 ---
        float radiusLimit = 5.5f;
        Vector3 currentPos = transform.position;
        Vector2 horizontalPos = new Vector2(currentPos.x, currentPos.z);

        if (horizontalPos.magnitude > radiusLimit)
        {
            Vector2 clampedPos = horizontalPos.normalized * radiusLimit;
            transform.position = new Vector3(clampedPos.x, currentPos.y, clampedPos.y);

            Vector3 velocity = rb.linearVelocity;
            Vector3 outwardDir = new Vector3(horizontalPos.x, 0, horizontalPos.y).normalized;
            float outwardSpeed = Vector3.Dot(velocity, outwardDir);

            if (outwardSpeed > 0)
            {
                rb.linearVelocity -= outwardDir * outwardSpeed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            int realIndex = GetComponent<PlayerInput>().playerIndex;
            scoreManager = Object.FindFirstObjectByType<ScoreManager>();

            if (scoreManager != null)
            {
                Destroy(other.gameObject);
                scoreManager.AddScore(realIndex, 100);
            }
        }
    }
}