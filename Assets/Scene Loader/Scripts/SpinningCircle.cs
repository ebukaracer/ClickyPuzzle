using UnityEngine;

namespace Racer.LoadManager
{
    public class SpinningCircle : MonoBehaviour
    {
        private readonly Vector3[] rotations = new Vector3[]
        {
        new Vector3( 0f, 0f, 0f ),
        new Vector3( 0f, 0f, 30f ),
        new Vector3( 0f, 0f, 60f ),
        new Vector3( 0f, 0f, 90f ),
        new Vector3( 0f, 0f, 120f ),
        new Vector3( 0f, 0f, 150f ),
        new Vector3( 0f, 0f, 180f ),
        new Vector3( 0f, 0f, 210f ),
        new Vector3( 0f, 0f, 240f ),
        new Vector3( 0f, 0f, 270f ),
        new Vector3( 0f, 0f, 300f ),
        new Vector3( 0f, 0f, 330f )
        };

        int index = 0;

        float time;

        [Range(-1, 1f), SerializeField]
        float animationDelay = 0.04f;

        [SerializeField]
        public bool clockwise;

        private float nextFrameTime = 0f;

        void Awake()
        {
            nextFrameTime = Time.realtimeSinceStartup + animationDelay;
        }

        private void Start()
        {
            LoadManager.Instance.OnLoadFinished += Instance_OnLoadFinished; ;
        }

        void Update()
        {
            time = Time.realtimeSinceStartup;

            while (time >= nextFrameTime)
            {
                nextFrameTime += animationDelay;

                if (!clockwise)
                    index = (index + 1) % rotations.Length;
                else if (--index < 0)
                    index = rotations.Length - 1;

                transform.localEulerAngles = rotations[index];
            }
        }

        private void Instance_OnLoadFinished()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            time = 0;

            LoadManager.Instance.OnLoadFinished -= Instance_OnLoadFinished;
        }
    }
}