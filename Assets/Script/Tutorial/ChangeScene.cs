using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName; // 전환할 씬 이름

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // --- FadeTransition 인스턴스가 있는지 확인 후 호출 ---
            if (FadeTransition.Instance != null)
            {
                // 단순히 씬을 로드하는 게 아니라, 페이드 효과를 동반한 함수를 호출합니다.
                FadeTransition.Instance.FadeToScene(sceneName);
            }
            else
            {
                // 만약 씬에 FadeTransition이 없다면 비상용으로 기존 방식을 사용합니다.
                Debug.LogWarning("FadeTransition Instance를 찾을 수 없어 일반 로드를 실행합니다.");
                SceneManager.LoadScene(sceneName);
            }

            // 한 번 부딪히면 다시 실행되지 않도록 콜라이더를 끕니다. (중복 방지)
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
