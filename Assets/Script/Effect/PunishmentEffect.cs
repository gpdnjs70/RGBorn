using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishmentEffect : MonoBehaviour
{
    public float lifeTime = 0.7f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
