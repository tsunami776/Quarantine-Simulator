using System.Collections;
using Assets.Backpack.Extensions;
using UnityEngine;

namespace Assets.Backpack.GeneralUtilities
{
    public class ScaleSmoothMutator
    {
        public static TrackableCoroutine ScaleToFit(ScaleToFitTarget scaleToFitTarget, Bounds fitToBounds, float seconds, out Vector3 targetScale)
        {
            var trackableCoroutine = new TrackableCoroutine();
            targetScale = scaleToFitTarget.MeshBounds.GetScaleToFitWithinBound(fitToBounds);
            return trackableCoroutine.Init(ScaleToFitInternal(targetScale.x * scaleToFitTarget.Transform.localScale, seconds, trackableCoroutine, scaleToFitTarget.Transform));
        }

        private static IEnumerator ScaleToFitInternal(Vector3 targetScale, float seconds, TrackableCoroutine trackableCoroutine, Transform targetTransform)
        {
            var elapsedTime = 0f;
            var initialScale = targetTransform.localScale;
            while (elapsedTime < seconds && !trackableCoroutine.IsForceStopRequested)
            {
                var timeCompletion = elapsedTime / seconds;
                targetTransform.localScale = Vector3.Lerp(initialScale, targetScale, timeCompletion);
                elapsedTime += Time.deltaTime;

                trackableCoroutine.OnBeforeYieldReturn();
                yield return new WaitForEndOfFrame();
            }

            if (!trackableCoroutine.IsForceStopRequested)
            {
                targetTransform.localScale = targetScale;
            }

            trackableCoroutine.OnFinished();
        }

        public static TrackableCoroutine Scale(Transform target, Vector3 targetScale, float seconds)
        {
            var trackableCoroutine = new TrackableCoroutine();
            return trackableCoroutine.Init(ScaleInternal(target, targetScale, seconds, trackableCoroutine));
        }

        private static IEnumerator ScaleInternal(Transform target, Vector3 targetScale, float seconds, TrackableCoroutine trackableCoroutine)
        {
            var elapsedTime = 0f;
            var initialScale = target.localScale;
            while (elapsedTime < seconds && !trackableCoroutine.IsForceStopRequested)
            {
                var timeCompletion = elapsedTime / seconds;
                target.localScale = Vector3.Lerp(initialScale, targetScale, timeCompletion);
                elapsedTime += Time.deltaTime;

                trackableCoroutine.OnBeforeYieldReturn();
                yield return new WaitForEndOfFrame();
            }

            if (!trackableCoroutine.IsForceStopRequested)
            {
                target.localScale = targetScale;
            }

            trackableCoroutine.OnFinished();
        }
    }

    public class ScaleToFitTarget
    {
        public Bounds MeshBounds { get; }
        public Transform Transform { get; }

        public ScaleToFitTarget(Bounds meshBounds, Transform transform)
        {
            MeshBounds = meshBounds;
            Transform = transform;
        }
    }
}