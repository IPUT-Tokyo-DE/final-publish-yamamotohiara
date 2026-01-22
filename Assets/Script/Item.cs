using UnityEngine;
using System.Collections; // コルーチンを使うために必要

public class Item : MonoBehaviour
{
    public GameObject shadowPrefab; // 影用プレハブ
    private GameObject currentShadow;
    private Material shadowMaterial; // 影のマテリアルを保持する変数

    // ★追加：すでに取られたかどうかを判定するフラグ
    private bool isCollected = false;

    void Start()
    {
        if (shadowPrefab != null)
        {
            currentShadow = Instantiate(shadowPrefab);
            currentShadow.transform.position = new Vector3(transform.position.x, 0.02f, transform.position.z);

            Renderer rend = currentShadow.GetComponent<Renderer>();
            if (rend != null)
            {
                shadowMaterial = rend.material;
            }
        }
    }

    void Update()
    {
        // ★回収済みなら影の更新を止める
        if (currentShadow == null || isCollected) return;

        currentShadow.transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
        float height = transform.position.y;
        float clampHeight = Mathf.Max(0, height);

        float scale = Mathf.Lerp(2.0f, 0.2f, clampHeight / 15f);
        currentShadow.transform.localScale = new Vector3(scale, scale, 1f);

        if (shadowMaterial != null)
        {
            Color color = shadowMaterial.GetColor("_BaseColor");
            color.a = Mathf.Lerp(0.6f, 0.1f, clampHeight / 15f);
            shadowMaterial.SetColor("_BaseColor", color);
        }
    }

    // ★【追加】PlayerControllerから呼ばれる「ぷにっ」と消える演出
    public void CollectAndAnimate()
    {
        if (isCollected) return;
        isCollected = true;

        // 影をすぐに消す
        if (currentShadow != null) Destroy(currentShadow);

        // 重力を無視して物理的な動きを止める
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        // ぷにっとして消えるアニメーション開始
        StartCoroutine(PuniRoutine());
    }

    private IEnumerator PuniRoutine()
    {
        Vector3 startScale = transform.localScale;
        // ぷにっとなるサイズ（横に1.5倍、縦に0.4倍）
        Vector3 targetScale = new Vector3(startScale.x * 1.5f, startScale.y * 0.4f, startScale.z * 1.5f);

        float duration = 0.15f; // 0.15秒で変形
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 徐々に変形させる
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
        }

        // 変形が終わったら消去
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 回収演出中は当たり判定を無視
        if (isCollected) return;

        // 地面（Stage）に当たった時だけ即座に消える
        // Playerとの接触はPlayerController側のCollectAndAnimateで処理される
        if (other.gameObject.CompareTag("Stage"))
        {
            if (currentShadow != null) Destroy(currentShadow);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (currentShadow != null) Destroy(currentShadow);
    }
}