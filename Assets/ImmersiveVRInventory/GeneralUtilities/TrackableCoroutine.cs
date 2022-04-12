using System;
using System.Collections;
using UnityEngine;

namespace Assets.Backpack.GeneralUtilities
{
    public class TrackableCoroutine
    {
        public event EventHandler<EventArgs> BeforeYieldReturn;
        public event EventHandler<EventArgs> BeforeStart;
        public event EventHandler<EventArgs> Finished;

        private IEnumerator CoroutineEnumerator { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsForceStopRequested { get; set; }

        public TrackableCoroutine Init(IEnumerator coroutineEnumerator)
        {
            CoroutineEnumerator = coroutineEnumerator;
            return this;
        }

        public void Start(Func<IEnumerator, Coroutine> startCoroutine)
        {
            IsInProgress = true;
            BeforeStart?.Invoke(this, EventArgs.Empty);
            startCoroutine(CoroutineEnumerator);
        }

        public void OnBeforeYieldReturn()
        {
            BeforeYieldReturn?.Invoke(this, EventArgs.Empty);
        }

        public void OnFinished()
        {
            IsInProgress = false;
            Finished?.Invoke(this, EventArgs.Empty);
        }

        public void ForceStop()
        {
            IsInProgress = false;
            IsForceStopRequested = true;
        }
    }
}