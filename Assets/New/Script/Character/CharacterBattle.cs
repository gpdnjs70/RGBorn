using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    Animator animator;
    private State state;

    private Vector3 slideTargetPosition;
    private Action onSlideComplete;

    private bool isPlayerTeam;
    private GameObject selectionCircleGameObject;

    private CharacterAttack characterAttack;

    private enum State
    {
        Idle,
        Sliding,
        Busy,
    }

    void Start()
    {
        
    }

    public void Setup(bool isPlayerTeam)
    {
        this.isPlayerTeam = isPlayerTeam;

        PlayAnimIdle();
    }

    private void Awake()
    {
        selectionCircleGameObject = transform.Find("selectionCircle").gameObject;
        animator = GetComponent<Animator>();
        characterAttack = GetComponent<CharacterAttack>();

        HideSelectionCircle();
        state = State.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Busy:
                break;
            case State.Sliding:
                float slideSpeed = 10f;
                transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;

                float reachedDistance = 1f;
                if(Vector3.Distance(GetPosition(), slideTargetPosition) < reachedDistance)
                {
                    // Arrived at Slide Target Position
                    transform.position = slideTargetPosition;
                    onSlideComplete();
                }
                break;
        }
    }

    private void PlayAnimIdle()
    {
        if (isPlayerTeam)
        {
            animator.SetFloat("Direction", 1);
        }
        else
        {
            animator.SetFloat("Direction", -1);
        }
    }

    public bool IsIdle()
    {
        return state == State.Idle;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Attack(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 2f;
        Vector3 startingPosition = GetPosition();

        // Slide to Target
        SlideToPosition(slideTargetPosition, () =>
        {
            // Arrived at Target, attack him
            state = State.Busy;
            Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
            StartCoroutine(PlayAttackAndContinue(targetCharacterBattle, () =>
            {
                // Attack completed, slide back
                SlideToPosition(startingPosition, () =>
                {
                    // Slide back completed, back to idle
                    state = State.Idle;
                    // characterBase.PlayAnimIdle(attackDir);
                    PlayAnimIdle();
                    onAttackComplete();
                });
            }));
        });
    }

    // 애니메이션 재생과 대기를 처리하는 헬퍼 코루틴
    IEnumerator PlayAttackAndContinue(CharacterBattle targetCharacterBattle, Action onComplete)
    {
        animator.SetTrigger("Attack");
        yield return null;

        float duration = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(duration * 0.5f);

        if (characterAttack != null && targetCharacterBattle != null)
        {
            character attacker = GetComponent<character>();
            character target = targetCharacterBattle.GetComponent<character>();

            characterAttack.ExecuteAttack(attacker, target);
        }

        yield return new WaitForSeconds(duration * 0.5f);
        onComplete?.Invoke();
    }

    private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete)
    {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        state = State.Sliding;

        if(slideTargetPosition.x >= 0)
        {
            animator.SetFloat("Direction", 1);
        }
        else
        {
            animator.SetFloat("Direction", -1);
        }
    }

    public void HideSelectionCircle()
    {
        selectionCircleGameObject.SetActive(false);
    }

    public void ShowSelectionCircle()
    {
        selectionCircleGameObject.SetActive(true);
    }

    public bool IsPlayerTeam()
    {
        return isPlayerTeam;
    }
}
