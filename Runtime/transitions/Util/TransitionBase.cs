using System;
using BeatThat.Requests;
using UnityEngine;

namespace BeatThat.Transitions
{
    public abstract class TransitionBase : RequestBase, Transition
    {
        override protected void ExecuteRequest()
        {
            StartTransition();
        }

        public event Action<Transition> Completed;

        public string transitionName { get { return this.id; } set { this.id = value; } }

        override public string loggingName { get { return this.transitionName; } }

        public void StartTransition()
        {
            StartTransition(Time.time);
        }

        public void StartTransition(float time)
        {
            StartTransition(time, true);
        }

        public void StartTransition(float time, bool andRun)
        {
            if (this.debug)
            {
                Debug.Log("[" + Time.frameCount + "]" + GetType() + "[" + this.transitionName
                    + "]::StartTransition (andRun=" + andRun + ") isTransitionRunning=" + this.isTransitionRunning + ", status=" + this.status);
            }

            if (this.isTransitionRunning)
            {
                return;
            }


            this.startTime = time + delay;

            UpdateStatus(RequestStatus.IN_PROGRESS);

            DoStartTransition(time);

            if (andRun)
            {
                TransitionRunner.AddTransition(this);
            }
        }

        virtual protected void DoStartTransition(float time)
        {
        }

        public void UpdateTransition(float time, float deltaTime)
        {
            if (!this.isTransitionRunning)
            {
                return;
            }

            if (time < this.startTime)
            {
                return;
            }

            DoUpdateTransition(time, deltaTime);
        }

        virtual protected void DoUpdateTransition(float time, float deltaTime)
        {
        }

        public Transition WithCompletedAction(Action<Transition> a)
        {
            this.Completed += a;
            return this;
        }

        override protected void BeforeCancel()
        {
            if (!this.isTransitionRunning)
            {
                return;
            }

            DoCancelTransition();

            if (this.debug)
            {
                Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType() + "::Cancel");
            }
        }

        virtual protected void DoCancelTransition() { }

        public void Complete()
        {
            Complete(false);
        }

        private bool isCompleteEarly { get; set; }

        sealed override protected void BeforeCompletionCallback()
        {
            if (this.debug)
            {
                Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "] " + GetType()
                    + "::BeforeCompletionCallback status" + this.status);
            }

            if (this.isCompleteEarly)
            {
                CompleteTransitionEarly();
            }
            else
            {
                CompleteTransition();
            }
        }

        sealed override protected void AfterCompletionCallback()
        {
            if (this.debug)
            {
                Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "] " + GetType()
                    + "::AfterCompletionCallback status " + this.status);
            }

            if (this.Completed != null)
            {
                this.Completed(this);
            }

            AfterTransitionCompleteCallback();
        }

        virtual protected void AfterTransitionCompleteCallback() { }

        protected void Complete(bool early)
        {
            if (this.debug)
            {
                Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType()
                    + "::Complete (early=" + early + ") status=" + this.status);
            }
            if ((this as Request).IsDoneOrCancelled())
            {
                return;
            }
            this.isCompleteEarly = early;
            CompleteRequest();
        }


        public void CompleteEarly()
        {
            Complete(true);
        }

        virtual protected void CompleteTransition() { }

        virtual protected void CompleteTransitionEarly()
        {
            CompleteTransition();
        }

        public float delay { get; set; }
        public bool didTransitionStart { get { return this.status != RequestStatus.NONE; } }
        public bool isTransitionComplete { get { return this.status == RequestStatus.DONE && !this.hasError; } }
        public bool isTransitionRunning { get { return this.IsQueuedOrInProgress(); } }
        public float startTime { get; protected set; }

        override public string ToString()
        {
            return "[" + GetType() + " " + this.transitionName + " running=" + this.isTransitionRunning
                + ", complete=" + this.isTransitionComplete + "]";
        }

    }
}

