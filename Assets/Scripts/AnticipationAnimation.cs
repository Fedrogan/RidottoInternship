using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnticipationAnimation : MonoBehaviour
{
    //public event Action Anticipation;

    [SerializeField] private float deactivationDuration;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private CanvasGroup[] particlesDisablers;

    private static readonly int Play = Animator.StringToHash("isPlay");

    public bool IsActive { get; private set; }

    public void Activate(object param = null)
    {     
        IsActive = true;

        StopAllCoroutines();

        canvasGroup.alpha = 1f;

        animator.enabled = true;
        animator.SetBool(Play, true);

        StartCoroutine(CoActivate());
    }

    private IEnumerator CoActivate()
    {
        yield return new WaitForSeconds(deactivationDuration);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Play();
            particlesDisablers[i].alpha = 1f;
        }
    }

    public void Deactivate()
    {
        animator.SetBool(Play, false);

        StartCoroutine(CoDeactivate());

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop();
            particlesDisablers[i].alpha = 0f;
        }
    }

    private IEnumerator CoDeactivate()
    {
        yield return new WaitForSeconds(deactivationDuration);

        IsActive = false;

        canvasGroup.alpha = 0f;

        animator.enabled = false;
    }
}
