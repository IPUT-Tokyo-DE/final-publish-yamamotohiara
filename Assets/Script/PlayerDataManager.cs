using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    // 参加したデバイスの情報を保存するリスト
    public List<InputDevice> joinedDevices = new List<InputDevice>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // このオブジェクトだけがシーンをまたぐ
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ★タイトルに戻った時に呼び出してリストを空にする関数
    public void ResetData()
    {
        joinedDevices.Clear();
    }
}