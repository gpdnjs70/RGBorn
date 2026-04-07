using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxTrigger : MonoBehaviour
{
    private TutorialManager_0 manager;

    private void Start()
    {
        manager = FindFirstObjectByType<TutorialManager_0>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            manager.PlayerReachedBox(); // 매니저에게 플레이어 왔다고 알림
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            manager.PlayerLeftBox(); // 멀어짐 (false로 리셋)
        }
    }
}
