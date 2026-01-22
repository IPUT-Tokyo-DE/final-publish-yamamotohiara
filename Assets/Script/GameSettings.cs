using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    // staticにすることで、シーンが変わってもこの数値は消えずに残ります
    public static int PlayerCount = 2;

    // ボタンから呼ばれる関数
    public void SelectPlayerCount(int count)
    {
        PlayerCount = count;
        // 「MainGame」は自分のゲームシーンの名前に合わせて変えてください
        SceneManager.LoadScene("MainGame");
    }
}