using UnityEngine;

namespace Racer.LoadManager
{
    /// <summary>
    /// Fades-in/out canvas-group alpha via animation wired up in the scene.
    /// See also: <see cref="LoadManager"/>.
    /// Enables a loading animation on the go.
    /// See also: <see cref="LoadTask"/>.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    internal class AnimatorTask : LoadTask
    {
        private Animator _loaderAnimator;

        private void Start()
        {
            _loaderAnimator = GetComponent<Animator>();

            // Initialize to the animation playback-time
            LoadManager.Instance.InitDelay = .25f;

            LoadManager.Instance.OnLoadStarted += Instance_OnLoadStarted;

            LoadManager.Instance.OnLoadInit += Instance_OnLoadInit;

            if (fadeOnStart)
                FadeOut();
        }

        private void Instance_OnLoadStarted()
        {
            EnableLoaderDefaultAnimation();
        }

        private void Instance_OnLoadInit()
        {
            FadeIn();
        }

        private void FadeOut()
        {
            _loaderAnimator.SetTrigger(Animator.StringToHash("Fade_Out"));
        }

        private void FadeIn()
        {
            _loaderAnimator.SetTrigger(Animator.StringToHash("Fade_In"));
        }

        private void OnDisable()
        {
            LoadManager.Instance.OnLoadInit -= Instance_OnLoadInit;
        }
    }
}