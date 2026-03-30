using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributeMono : MonoBehaviour
{
    [SerializeField] private ColorMode mode;

    private Animator animator;
    private ColorAttribute attribute;
    [SerializeField] private AnimatorOverrideController colorOverride;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attribute = GetComponent<ColorAttribute>();
    }

    private void Start()
    {
        attribute.SetColor(mode);

        if (animator != null && colorOverride != null)
        {
            animator.runtimeAnimatorController = colorOverride;
        }
    }
}
