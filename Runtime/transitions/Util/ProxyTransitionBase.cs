using System;
using BeatThat.Requests;
using UnityEngine;

namespace BeatThat.Transitions
{
    /// <summary>
    /// Wraps a transition, allowing things like instatiate (the wrapped) transition at start time.
    /// </summary>
    public abstract class ProxyTransitionBase : RequestBase, Transition
	{
		public event Action<Transition> Completed;

		override protected void ExecuteRequest()
		{
			StartTransition();
		}
			
		override protected void BeforeCancel()
		{
			if(!this.isTransitionRunning) {
				return;
			}

			if(this.proxyTransition != null) {
				this.proxyTransition.Cancel();
			}

			DoCancelTransition();
		}

		virtual protected void DoCancelTransition() {}

		public string transitionName { get; set; }
		
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
			StartTransition(Time.time, true);
		}
		
		public void StartTransition(float time)
		{
			StartTransition(time, true);
		}
		
		public void StartTransition(float time, bool andRun)
		{
			if(this.isTransitionRunning) {
				return;
			}
			UpdateToInProgress();
			DoStartTransition(time);
			if(andRun) {
				TransitionRunner.AddTransition(this);
			}
		}

		virtual protected void DoStartTransition(float time)
		{
			Transition t = InitTransition();
			if(t == null) {
				if(this.debug) {
					Debug.LogWarning("[" + Time.frameCount + "] " + GetType() + "::DoStartTranstion " + this.transitionName + " InitTransition returned null");
				}
				Complete();
				return;
			}
				
			t.StartTransition(time, false);
		}
		
		virtual public void UpdateTransition(float time, float deltaTime)
		{
			var t = this.proxyTransition;
			if(t == null) {
				Complete();
				Debug.LogWarning("[" + Time.frameCount + "] " + GetType() + "::UpdateTransition: proxy transition is null!");
				return;
			}
				
			this.proxyTransition.UpdateTransition(time, deltaTime);

			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "] " + GetType() + "::UpdateTransition [" + t.transitionName + "/" + t.GetType()  + "] in status " + t.status + ", localStatus is " + this.status);
			}

		}

		public void Complete()
		{
			Complete(false);
		}
		
		public void CompleteEarly()
		{
			Complete(true);
		}


		protected void Complete(bool early)
		{
			if(this.isTransitionComplete) {
				return;
			}

			this.isCompleteEarly = early;

			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType() + "::Complete (early=" + early + ")"); 
			}

			CompleteRequest();

		}

		private bool isCompleteEarly { get; set; }

		sealed override protected void BeforeCompletionCallback()
		{
			if(this.status == RequestStatus.CANCELLED) {
				return;
			}

			Transition t = InitTransition();
			if(t == null) {
				if(this.debug) {
					Debug.Log("[" + Time.frameCount + "]["+ this.transitionName + "] " + GetType() 
						+ "::BeforeCompletionCallback proxy transition is null!");
				}

				return;
			}

			if(!t.IsQueuedOrInProgress()) {
				return;
			}

			if(isCompleteEarly) {
				t.CompleteEarly();
			}
			else {
				t.Complete();
			}
		}

		sealed override protected void AfterCompletionCallback()
		{
			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "]["+ this.transitionName + "] " + GetType() 
					+ "::AfterCompletionCallback status" + this.status);
			}

			if(this.Completed != null) {
				this.Completed(this);
			}
		}



		public Transition WithCompletedAction(Action<Transition> a)
		{
			this.Completed += a;
			return this;
		}
		

		public bool didTransitionStart { get { return this.status != RequestStatus.NONE; } } 
		public bool isTransitionComplete { get { return this.status == RequestStatus.DONE && !this.hasError; } }
		public bool isTransitionRunning { get { return this.proxyTransition != null && this.IsQueuedOrInProgress(); } }
		
		protected abstract Transition CreateTransition();
		
		protected Transition InitTransition()
		{
			var t = this.proxyTransition;

			if(t == null) {
				this.proxyTransition = t = CreateTransition();
			}

			if(t == null) {
				return null;
			}

			if(!this.hasInitTransition) {
				this.hasInitTransition = true;
				t.StatusUpdated += this.proxyTransitionStatusUpdatedAction;
			}

			return t;
		}

		private void OnProxyTransitionStatusUpdated()
		{
			if(!this.IsQueuedOrInProgress()) {
				return;
			}

			var t = this.proxyTransition;

			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "]" + GetType() + "::OnProxyTransitionStatusUpdated " + this.transitionName 
					+ ", status=" +  (t != null? t.status.ToString(): "null"));
			}

			if(t == null) {
				Cancel();
				return;
			}

			var s = this.proxyTransition.status;

			switch(s) {
			case RequestStatus.CANCELLED:
			case RequestStatus.DONE:
				CompleteRequest();
				break;
			default:
				UpdateStatus(s);
				break;
			}
		}
		private Action proxyTransitionStatusUpdatedAction { get { return m_proxyTransitionStatusUpdatedAction?? (m_proxyTransitionStatusUpdatedAction = this.OnProxyTransitionStatusUpdated); } }
		private Action m_proxyTransitionStatusUpdatedAction;


		private bool hasInitTransition { get; set; }

		virtual protected Transition proxyTransition { get; set; }

		override public string ToString()
		{
			return "[" + GetType() + " " + this.transitionName + " running=" + this.isTransitionRunning 
				+ ", complete=" + this.isTransitionComplete + "]";
		}
	}
}

