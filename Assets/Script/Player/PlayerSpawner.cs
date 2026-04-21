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

        // 데이터가 있고, 현재 씬과 일치할 때
        if (data != null && data.lastSceneName == SceneManager.GetActiveScene().name)
        {
            if (data.isRespawn && savePointPos != null)
            {
                // 세이브 포인트(부활 지점)로 이동
                transform.position = savePointPos.position;
            }
            else if (entrancePos != null)
            {
                // 입구로 이동
                transform.position = entrancePos.position;
            }
        }
    }
}
