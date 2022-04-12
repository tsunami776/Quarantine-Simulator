using System;
using System.Collections;
using UnityEngine;

namespace Assets.Backpack.GeneralUtilities
{
    public class ReversableAnimationTrackableCoroutine: TrackableCoroutine
    {
        public float CurrentElapsedTimeDirectionAdjusted { get; set; }
    }

    public class FloatAnimation
    {
        public enum Direction
        {
            Forward,
            Backward
        }

        public static ReversableAnimationTrackableCoroutine OverAnimationCurve(AnimationCurve animation, Action<float> onTick, float startTime = 0, Direction direction = Direction.Forward)
        {
            var trackableCoroutine = new ReversableAnimationTrackableCoroutine();
            trackableCoroutine.Init(OverAnimationCurveInternal(animation, startTime, onTick, direction, trackableCoroutine));
            return trackableCoroutine;
        }

        private static IEnumerator OverAnimationCurveInternal(AnimationCurve animation, float startTime, Action<float> onTick, Direction direction, ReversableAnimationTrackableCoroutine trackableCoroutine)
        {
            if(direction != Direction.Forward && direction != Direction.Backward) throw new Exception($"Unsupported direction: {direction}");

            var elapsedTimeDirectionAdjusted = startTime;
            var totalTime = 1f;

            while ((direction == Direction.Forward ? elapsedTimeDirectionAdjusted < totalTime : elapsedTimeDirectionAdjusted >= 0) && !trackableCoroutine.IsForceStopRequested)
            {
                var timeCompletion = elapsedTimeDirectionAdjusted / totalTime;
                var value = animation.Evaluate(timeCompletion);

                onTick(value);
                elapsedTimeDirectionAdjusted = direction == Direction.Forward? (elapsedTimeDirectionAdjusted  + Time.deltaTime) : elapsedTimeDirectionAdjusted - Time.deltaTime;
                trackableCoroutine.CurrentElapsedTimeDirectionAdjusted = Mathf.Clamp(elapsedTimeDirectionAdjusted, 0f, 1f);

                trackableCoroutine.OnBeforeYieldReturn();
                yield return new WaitForEndOfFrame();
            }

            if (!trackableCoroutine.IsForceStopRequested)
            {
                onTick(animation.Evaluate(direction == Direction.Forward ? 1 : 0));
                trackableCoroutine.CurrentElapsedTimeDirectionAdjusted = direction == Direction.Forward ? 1 : 0;
            }

            trackableCoroutine.OnFinished();
        }
    }
}