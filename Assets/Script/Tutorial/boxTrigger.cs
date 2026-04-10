using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxTrigger : MonoBehaviour
{
    private TutorialManager_0 manager;
    private Animator anim;

    private void Start()
    {
        manager = FindFirstObjectByType<TutorialManager_0>();
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("어떤 물체가 들어옴: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            manager.PlayerReachedBox(); // 매니저에게 플레이어 왔다고 알림
            // Debug.Log("Hit Box");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            manager.PlayerLeftBox(); // 멀어짐 (false로 리셋)
        }
    }

    public void PlayOpenAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("Open");
        }
    }

    public void PlayFinishAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("Finish");
        }
    }
}
