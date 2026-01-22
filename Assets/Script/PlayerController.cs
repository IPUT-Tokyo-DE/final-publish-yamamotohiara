using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    private Rigidbody rb;
    private Vector2 inputVector;

    [Header("プレイヤーごとの色（1P, 2P, 3P, 4P）")]
    public Color[] playerColors = { Color.red, Color.blue, Color.yellow, Color.green };

    private ScoreManager scoreManager;

    // ★追加：効果音を鳴らすためのAudioSource
    private AudioSource soundManagerAudio;

    [Header("見た目の制御用")]
    public GameObject visualModel;

    private bool isStunned = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scoreManager = Object.FindFirstObjectByType<ScoreManager>();

        // ★SoundManagerという名前のオブジェクトからAudioSourceを取得しておく
        GameObject smObj = GameObject.Find("SoundManager");
        if (smObj != null)
        {
            soundManagerAudio = smObj.GetComponent<AudioSource>();
        }

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

    public void Stun(float duration)
    {
        if (isStunned) return;
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        inputVector = Vector2.zero;
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

        // ★シーンが切り替わった時も再取得
        GameObject smObj = GameObject.Find("SoundManager");
        if (smObj != null)
        {
            soundManagerAudio = smObj.GetComponent<AudioSource>();
        }

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

        if (isStunned)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

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
            // ★お餅に「ぷにっ」と消える演出がある場合はそちらのメソッドを呼ぶ
            // もしない場合は、今まで通りここでDestroy
            Item item = other.gameObject.GetComponent<Item>();

            int realIndex = GetComponent<PlayerInput>().playerIndex;
            scoreManager = Object.FindFirstObjectByType<ScoreManager>();

            if (scoreManager != null)
            {
                // ★効果音を鳴らす
                if (soundManagerAudio != null)
                {
                    soundManagerAudio.PlayOneShot(soundManagerAudio.clip);
                }

                // 演出メソッドがあれば呼び、なければ即消去
                if (item != null)
                {
                    item.CollectAndAnimate(); // ぷにっと消える演出
                }
                else
                {
                    Destroy(other.gameObject);
                }

                scoreManager.AddScore(realIndex, 100);
            }
        }
    }
}