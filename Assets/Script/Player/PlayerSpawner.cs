using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public Transform entrancePos;    // 씬의 입구 (정상 진입 시)
    public Transform savePointPos;   // 씬의 세이브 포인트 (부활/이어하기 시)

    void Start()
    {
        if (SaveManager.Instance == null) return;

        SaveData data = SaveManager.Instance.LoadGame();
        string currentScene = SceneManager.GetActiveScene().name;

        // 1. 데이터가 아예 없으면 (첫 시작 등) 입구로
        if (data == null)
        {
            if (entrancePos != null) transform.position = entrancePos.position;
            return;
        }

        if (data.lastSceneName == currentScene)
        {
            if (data.isRespawn && savePointPos != null)
            {
                // 이어하기 혹은 죽어서 부활한 경우 -> 세이브 포인트
                transform.position = savePointPos.position;
                Debug.Log("세이브 포인트에서 스폰");
            }
            else if (entrancePos != null)
            {
                // 이전 씬에서 넘어온 경우 (isRespawn이 false일 것) -> 입구
                transform.position = entrancePos.position;
                Debug.Log("입구에서 스폰");
            }
        }
    }
}
