using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("プレハブ設定")]
    public GameObject itemPrefab; // アイテムのプレハブ
    public GameObject rockPrefab; // ★岩のプレハブ

    [Header("生成設定")]
    public float spawnInterval = 1.0f; // 何秒おきに降らすか
    public float spawnRadius = 4.5f;   // ステージの半径
    public float spawnHeight = 15f;    // どの高さから降らすか

    [Range(0, 1)]
    public float rockChance = 0.3f;    // ★岩が出る確率（0.3 = 30%）

    void Start()
    {
        // spawnInterval秒ごとに「Spawn」関数を実行し続ける
        InvokeRepeating("Spawn", 0f, spawnInterval);
    }

    void Spawn()
    {
        Vector3 spawnPoint = Vector3.zero;
        bool canSpawn = false;
        int attempts = 0; // 無限ループ防止用

        // 他のオブジェクトと重ならない場所を探す（最大10回試行）
        while (!canSpawn && attempts < 10)
        {
            attempts++;

            // 円形の範囲内でランダムな位置を計算
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            spawnPoint = new Vector3(randomPos.x, spawnHeight, randomPos.y);

            // ★生成予定地点の周りに他のオブジェクトがないかチェック
            // 半径1.0m以内にCollider（ItemやRock）がなければOK
            Collider[] colliders = Physics.OverlapSphere(spawnPoint, 1.0f);
            if (colliders.Length == 0)
            {
                canSpawn = true;
            }
        }

        // 安全な場所が見つかった場合のみ生成
        if (canSpawn)
        {
            // 確率に基づいて、岩かアイテムのどちらかを決める
            GameObject prefabToSpawn = (Random.value < rockChance) ? rockPrefab : itemPrefab;

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, spawnPoint, Quaternion.identity);
            }
        }
    }
}