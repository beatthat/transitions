using UnityEngine;

using BeatThat.Anim;
using System;

namespace BeatThat.Anim.Comp
{
	public abstract class TimedTransitionBase : MonoBehaviour, TimedTransition
	{
		public string m_transitionName;
		public float m_duration = 0.3f;
		public float m_delay;

		public event Action StatusUpdated
		{
			add { this.transitionDelegate.StatusUpdated += value; }
			remove { this.transitionDelegate.StatusUpdated -= value; }
		}

		public void Cancel ()
		{
			this.transitionDelegate.Cancel();
		}

		public void Execute (Action callback = null)
		{
			this.transitionDelegate.Execute(callback);
		}

		public bool isCancelled { get { return this.status == RequestStatus.CANCELLED; } }

		public RequestStatus status { get { return this.transitionDelegate.status; } }

		public float progress { get { return this.transitionDelegate.progress; } }

		public bool hasError { get { return this.transitionDelegate.hasError; } }

		public bool debug { get { return this.transitionDelegate.debug; } set { this.transitionDelegate.debug = value; } }

		public string error { get { return this.transitionDelegate.error; } }

		public void Dispose () { this.transitionDelegate.Dispose(); } 
	
		public event Action<Transition> Completed
		{
			add {
				this.transitionDelegate.Completed += value;
			}
			remove {
				this.transitionDelegate.Completed -= value;
			}
		}
		
		public string transitionName
		{
			get {
				return !string.IsNullOrEmpty (m_transitionName) ? m_transitionName : this.transitionDelegate.transitionName;
			}
			set {
				m_transitionName = value;
				this.transitionDelegate.transitionName = value;
			}
		}
		
		protected abstract TimedTransition transitionDelegate { get; }
		
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
			this.transitionDelegate.StartTransition(time, false);
			if(andRun) {
				TransitionRunner.AddTransition(this);
			}
		}
		
		public TimedTransition WithDelay(float delay)
		{
			this.delay = delay;
			return this;
		}
		
		public Transition WithCompletedAction(Action<Transition> a)
		{
			this.transitionDelegate.WithCompletedAction(a);
			return this;
		}

		public bool m_debug;
		public void UpdateTransition(float time, float deltaTime)
		{
			
			this.transitionDelegate.UpdateTransition(time, deltaTime);

			if(this.debug || m_debug) {
				Debug.LogWarning("[" + Time.frameCount + "] " + GetType() + "[" + this.name + "]::UpdateTransition with time=" + time + ", deltaTime=" + deltaTime 
//					+ ", Time.time=" + Time.time + ", Time.deltaTime="  + Time.deltaTime
//					+ ", timeUnscaled=" + Time.unscaledTime + ", deltaTimeUnscaled="  + Time.unscaledDeltaTime
					+ ", startTime=" + (this.transitionDelegate as BeatThat.Anim.TransitionBase).startTime 
					+ ", timeIn=" + this.timeIn + ", timeRem=" + this.timeRem + ", dur=" + this.dur);
			}


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
			Complete();
		}
		
		public float timeIn
		{
			get {
				return this.transitionDelegate.timeIn;
			}
		}

		public float timeRem
		{
			get {
				return this.transitionDelegate.timeRem;
			}
		}
		
		public float pctComplete
		{
			get {
				return this.transitionDelegate.pctComplete;
			}
		}
		
		public bool isTransitionComplete
		{
			get {
				return this.transitionDelegate.isTransitionComplete;
			}
		}
		
		public float dur
		{
			get {
				return this.transitionDelegate.dur;
			}
			set {
				this.transitionDelegate.dur = value;
				m_duration = value;
			}
		}
		
		public float delay
		{
			get {
				return this.transitionDelegate.delay;
			}
			set {
				this.transitionDelegate.delay = value;
				m_delay = value;
			}
		}
		
	}
}
