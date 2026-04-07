using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitDoor : MonoBehaviour
{
    private TutorialManager_0 manager;

    private void Start()
    {
        manager = FindFirstObjectByType<TutorialManager_0>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 태그 확인
        if (collision.CompareTag("Player"))
        {
            manager.PlayerReachedDoor();
        }
    }
}
