using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttribute : MonoBehaviour
{
    [SerializeField] private ColorMode mode;
    private ColorAttribute attribute;

    private void Awake()
    {
        attribute = GetComponent<ColorAttribute>();
        attribute.SetColor(mode);
    }
}
