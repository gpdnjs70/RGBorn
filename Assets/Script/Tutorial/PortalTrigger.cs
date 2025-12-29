using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 페이드 매니저 호출
        FadeTransition.Instance.FadeToScene(nextSceneName);
    }
}