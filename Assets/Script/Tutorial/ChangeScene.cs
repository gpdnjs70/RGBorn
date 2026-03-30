using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName; // 전환할 씬 이름

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            /**
            GameData.SetSpawnPoint(collision.transform.position);
            Debug.Log($"[ChangeScene] 이전 씬 마지막 위치 저장 완료: {collision.transform.position}");
            **/

            SceneManager.LoadScene(sceneName);
        }
    }
}
