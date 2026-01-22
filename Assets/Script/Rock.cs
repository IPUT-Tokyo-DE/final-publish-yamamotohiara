using UnityEngine;

public class Rock : MonoBehaviour
{
    public GameObject shadowPrefab;
    private GameObject currentShadow;
    private Material shadowMaterial;

    void Start()
    {
        if (shadowPrefab != null)
        {
            currentShadow = Instantiate(shadowPrefab);
            // 生成した瞬間に真下に配置（中心から飛んでくるのを防止）
            currentShadow.transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);

            if (currentShadow.TryGetComponent<Collider>(out Collider col)) col.enabled = false;
            if (currentShadow.TryGetComponent<Renderer>(out Renderer rend)) shadowMaterial = rend.material;
        }
    }

    void Update()
    {
        if (currentShadow == null) return;

        currentShadow.transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
        float height = transform.position.y;
        float clampHeight = Mathf.Max(0, height);

        // ★岩なので影をアイテムより大きく設定 (例: 最大4.0倍)
        float scale = Mathf.Lerp(4.0f, 0.2f, clampHeight / 15f);
        currentShadow.transform.localScale = new Vector3(scale, scale, 1f);

        if (shadowMaterial != null)
        {
            Color color = shadowMaterial.GetColor("_BaseColor");
            color.a = Mathf.Lerp(0.6f, 0.1f, clampHeight / 15f); // 影も少し濃いめに
            shadowMaterial.SetColor("_BaseColor", color);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            // 1. 減点処理
            var pInput = other.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (pInput != null)
            {
                ScoreManager sm = Object.FindFirstObjectByType<ScoreManager>();
                if (sm != null) sm.SubtractScore(pInput.playerIndex, 10);

                // 2. スタン処理（2秒間動けなくする）
                var controller = other.GetComponent<PlayerController>();
                if (controller != null) controller.Stun(2.0f);
            }

            DestroyRock();
        }
        else if (other.CompareTag("Stage"))
        {
            DestroyRock();
        }
    }

    void DestroyRock()
    {
        if (currentShadow != null) Destroy(currentShadow);
        Destroy(gameObject);
    }
}