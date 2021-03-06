using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnticipationAnimation : MonoBehaviour
{
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

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Play();
            particlesDisablers[i].alpha = 1f;
        }
    }

    public void Deactivate()
    {
        animator.enabled = false;
        animator.SetBool(Play, false);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop();
            particlesDisablers[i].alpha = 0f;
        }

        IsActive = false;

        canvasGroup.alpha = 0f;

        animator.enabled = false;
    }
}
