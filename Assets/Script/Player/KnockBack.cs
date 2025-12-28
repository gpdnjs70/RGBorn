using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KnockBack : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float strentgh = 16, delay = 0.2f;

    public UnityEvent OnBegin, OnDone;

    public void PlayFeedBack(GameObject sender)
    {
        Debug.Log("KnockBack");
        StopAllCoroutines();
        OnBegin?.Invoke();

        Vector2 direction = (transform.position - sender.transform.position).normalized;
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(direction * strentgh, ForceMode2D.Impulse);
        
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb2d.velocity = Vector3.zero;
        OnDone?.Invoke();
    }
}
