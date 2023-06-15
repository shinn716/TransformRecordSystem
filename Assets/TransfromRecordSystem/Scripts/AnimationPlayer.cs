using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationController))]
public class AnimationPlayer : MonoBehaviour
{
    [SerializeField] bool playOnStart = true;
    [SerializeField] bool loop = false;
    [SerializeField] float currentTime = 0;
    [SerializeField] float speed = 1;

    private bool isPlaying = false;

    private void Start()
    {
        isPlaying = playOnStart;
    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            currentTime += Time.deltaTime * speed;
            if (loop)
            {
                if (currentTime > AnimationController.instance.GetTotalTime())
                    currentTime = 0;
            }
            currentTime = Mathf.Clamp(currentTime, 0, AnimationController.instance.GetTotalTime());
            AnimationController.instance.Loop(currentTime);
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        isPlaying = true;
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        isPlaying = false;
        currentTime = 0;
        AnimationController.instance.Loop(currentTime);
    }
}
