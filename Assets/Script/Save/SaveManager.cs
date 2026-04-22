using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    public string lastSceneName; // 저장된 씬 이름
    public bool isRespawn;      // true면 세이브 포인트, false면 맵 입구 스폰
    public float currentMaxHP = 50f;    // 도달할 고정 HP 값
    public float attackMultiplier = 1f; // 누적된 공격력 배율 (1.0 = 100%)
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    // 데이터 저장 (씬 이동 시나 세이브 포인트 밟을 때 호출)
    public void SaveGame(bool isRespawnPoint, string targetSceneName = null, SaveData updatedData = null)
    {
        // 1. 만약 외부에서 계산된 데이터를 줬다면 그걸 쓰고, 아니면 기존 파일을 로드함
        SaveData data = (updatedData != null) ? updatedData : LoadGame();

        if (data == null)
        {
            data = new SaveData();
        }

        data.lastSceneName = (targetSceneName != null) ? targetSceneName : SceneManager.GetActiveScene().name;
        data.isRespawn = isRespawnPoint;

        // 객체를 JSON 문자열로 변환
        string json = JsonUtility.ToJson(data, true);
        // 파일로 쓰기
        File.WriteAllText(savePath, json);

        Debug.Log("게임 저장 완료: " + savePath);
    }

    // 데이터 로드 (메인 메뉴의 이어하기 버튼에서 호출)
    public SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return null; // 저장 파일이 없을 경우
    }
}
