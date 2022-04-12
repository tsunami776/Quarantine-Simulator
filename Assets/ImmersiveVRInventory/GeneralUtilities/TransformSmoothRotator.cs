using System.Collections;
using UnityEngine;

namespace Assets.Backpack.GeneralUtilities
{
    public static class TransformSmoothRotator
    {
        public static TrackableCoroutine RotateOverSeconds(Transform transformToRotate, Quaternion end, float seconds)
        {
            var trackableCoroutine = new TrackableCoroutine();
            return trackableCoroutine.Init(RotateOverSecondsInternal(transformToRotate, end, seconds, trackableCoroutine));
        }

        private static IEnumerator RotateOverSecondsInternal(Transform transformToRotate, Quaternion end, float seconds, TrackableCoroutine trackableCoroutine)
        {
            float elapsedTime = 0;
            var startingRotation = transformToRotate.rotation;
            while (elapsedTime < seconds && !trackableCoroutine.IsForceStopRequested)
            {
                var timeCompletion = elapsedTime / seconds;
                var newRotation = Quaternion.Lerp(startingRotation, end, timeCompletion);
                transformToRotate.rotation = newRotation;
                elapsedTime += Time.deltaTime;

                trackableCoroutine.OnBeforeYieldReturn();
                yield return new WaitForEndOfFrame();
            }

            if (!trackableCoroutine.IsForceStopRequested)
            {
                var finalRotation = Quaternion.Lerp(startingRotation, end, 1);
                transformToRotate.rotation = finalRotation;
            }

            trackableCoroutine.OnFinished();
        }
    }
}