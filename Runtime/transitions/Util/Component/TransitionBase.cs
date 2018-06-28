using System;
using BeatThat.Requests;
using UnityEngine;

namespace BeatThat.Transitions.Comp
{
    public abstract class TransitionBase : MonoBehaviour, Transition
	{
		public event Action StatusUpdated
		{
			add { this.transitionDelegate.StatusUpdated += value; }
			remove { this.transitionDelegate.StatusUpdated -= value; }
		}

		public event Action<Transition> Completed
		{
			add { this.transitionDelegate.Completed += value; }
			remove { this.transitionDelegate.Completed -= value; }
		}

		public void Cancel ()
		{
			this.transitionDelegate.Cancel();
		}

		public void Execute (Action callback = null, bool callbackOnCancelled = false)
		{
            this.transitionDelegate.Execute(callback, callbackOnCancelled);
		}

		public bool isCancelled { get { return this.status == RequestStatus.CANCELLED; } }

		public RequestStatus status { get { return this.transitionDelegate.status; } }

		public float progress { get { return this.transitionDelegate.progress; } }

		public bool hasError { get { return this.transitionDelegate.hasError; } }

		public bool debug { get { return this.transitionDelegate.debug; } set { this.transitionDelegate.debug = value; } }

		public string error { get { return this.transitionDelegate.error; } }

		public void Dispose () { this.transitionDelegate.Dispose(); } 

		public string m_transitionName;
		

		
		virtual public string transitionName
		{
			get {
				return !string.IsNullOrEmpty (m_transitionName) ? m_transitionName : this.transitionDelegate.transitionName;
			}
			set {
				m_transitionName = value;
				this.transitionDelegate.transitionName = value;
			}
		}
		
		protected abstract Transition transitionDelegate { get; }
		
		public Transition WithName(string name)
		{
			this.transitionName = name;
			return this;
		}
				
		public T WithName<T>(string name) where T : class, Transition
		{
			this.transitionName = name;
			return this as T;
		}
		
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
			if (!this.transitionDelegate.isTransitionRunning) {
				SendMessage("OnTransitionStart", transitionName, SendMessageOptions.DontRequireReceiver);
			}
			this.transitionDelegate.StartTransition(time, false);
			if(andRun) {
				TransitionRunner.AddTransition(this);
			}
		}
		
		public Transition WithCompletedAction(System.Action<Transition> a)
		{
			this.transitionDelegate.WithCompletedAction(a);
			return this;
		}
		
		public void UpdateTransition(float time, float deltaTime)
		{
			this.transitionDelegate.UpdateTransition(time, deltaTime);
		}

		public bool didTransitionStart
		{
			get {
				return this.transitionDelegate.didTransitionStart;
			}
		}
		
		public bool isTransitionRunning
		{
			get {
				return this.transitionDelegate.isTransitionRunning;
			}
		}
		
		public void Complete()
		{
			this.transitionDelegate.Complete();
		}
		
		virtual public void CompleteEarly()
		{
			this.transitionDelegate.CompleteEarly();
		}
		
		public bool isTransitionComplete
		{
			get {
				return this.transitionDelegate.isTransitionComplete;
			}
		}
	}

}

