using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject shadowPrefab; // 影用プレハブ
    private GameObject currentShadow;
    private Material shadowMaterial; // 影のマテリアルを保持する変数

    void Start()
    {
        // 1. 影を生成
        if (shadowPrefab != null)
        {
            currentShadow = Instantiate(shadowPrefab);

            // ★【追加】生成した瞬間に、とりあえず今のアイテムの真下にワープさせる
            // これがないと、一瞬だけ(0,0,0)に見えてから移動するので、中心から飛んできたように見えます
            currentShadow.transform.position = new Vector3(transform.position.x, 0.02f, transform.position.z);

            // 2. 影のRendererからマテリアルを取得
            Renderer rend = currentShadow.GetComponent<Renderer>();
            if (rend != null)
            {
                // インスタンス化されたマテリアルを取得（これで個別に透明度が変えられます）
                shadowMaterial = rend.material;
            }
        }
    }

    void Update()
    {
        if (currentShadow == null) return;

        // 3. 影をアイテムの真下（地面の高さ Y=0.2）に配置
        currentShadow.transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);

        // 4. アイテムの現在の高さを取得
        float height = transform.position.y;

        // 5. 影の大きさを計算（地面に近いほど大きく：2.0、高いほど小さく：0.5）
        float scale = Mathf.Lerp(2.0f, 0.2f, height / 15f);
        currentShadow.transform.localScale = new Vector3(scale, scale, scale);

        // 6. 影の透明度を計算（地面に近いほど濃く：0.6、高いほど薄く：0.1）
        if (shadowMaterial != null)
        {
            // URPの標準的な色指定は "_BaseColor" です
            Color color = shadowMaterial.GetColor("_BaseColor");
            color.a = Mathf.Lerp(0.6f, 0.1f, height / 15f);
            shadowMaterial.SetColor("_BaseColor", color);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ステージやプレイヤーに当たったら自分と影を消す
        if (other.gameObject.CompareTag("Stage") || other.gameObject.CompareTag("Player"))
        {
            if (currentShadow != null) Destroy(currentShadow);
            Destroy(gameObject);
        }
    }

    // 万が一スクリプトやオブジェクトが削除された時も影を消す
    private void OnDestroy()
    {
        if (currentShadow != null) Destroy(currentShadow);
    }
}