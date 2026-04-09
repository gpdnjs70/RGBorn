using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bookTrigger : MonoBehaviour
{
    private TutorialManager_0 manager;

    private void Start()
    {
        manager = Object.FindFirstObjectByType<TutorialManager_0>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (manager != null)
            {
                manager.PlayerReachedBook();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (manager != null)
            {
                manager.PlayerLeftBook();
            }
        }
    }
}
