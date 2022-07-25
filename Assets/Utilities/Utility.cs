using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Racer.Utilities
{
    static public class Utility
    {

        static Camera _cameraMain;
        /// <summary>
        /// Gets a one time reference to the Camera.Main Method. 
        /// </summary>
        public static Camera CameraMain
        {
            get
            {
                if (_cameraMain == null)
                    _cameraMain = Camera.main;

                return _cameraMain;
            }
        }


        static readonly Dictionary<float, WaitForSeconds> WaitDelay = new Dictionary<float, WaitForSeconds>();
        /// <summary>
        /// Container that stores/reuses newly created WaitForSeconds.
        /// </summary>
        /// <param name="time">time(s) to wait</param>
        /// <returns>new WaitForSeconds</returns>
        public static WaitForSeconds GetWaitForSeconds(float time)
        {
            if (WaitDelay.TryGetValue(time, out var waitForSeconds)) return waitForSeconds;

            WaitDelay[time] = new WaitForSeconds(time);

            return WaitDelay[time];
        }



        static PointerEventData _eventDataCurrentPosition;

        static readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
        /// <summary>
        /// Checks if the mouse/pointer is over a UI element.
        /// </summary>
        /// <returns>true if the pointer is over a UI element</returns>
        public static bool IsPointerOverUI()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };

            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _raycastResults);

            return _raycastResults.Count > 0;
        }

        /// <summary>
        /// Gets the world position of a canvas element, can be used to spawn a 3d element in the 2d canvas
        /// </summary>
        /// <param name="rectTransform"> Canvas element(ui elements) </param>
        /// <returns>The position of the canvas element in world space</returns>
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform rectTransform)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, rectTransform.position, CameraMain, out var output);

            return output;
        }


        public static bool CheckColliderPresent(Vector3 spawnPoint, LayerMask objects)
        {
            var hitColliders = Physics.OverlapSphere(spawnPoint, 5f, objects);

            if (hitColliders.Length > 0)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Shuffles an array of strings
        /// </summary>
        /// <param name="texts">String Array to Shuffle</param>
        /// <returns>Shuffled string[]</returns>
        public static string[] RandomizeTexts(string[] texts)
        {

            return texts.OrderBy(x => new Random().Next())
                              .ToArray();
        }

        /// <summary>
        /// Converts a millisecond value to seconds
        /// </summary>
        /// <param name="value">Millisecond value</param>
        public static int ConvertMsToSeconds(float value)
        {
            int TIME_MULTIPLIER = 1000;

            return (int)(value * TIME_MULTIPLIER);
        }
    }
}