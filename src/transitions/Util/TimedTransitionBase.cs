using UnityEngine;

namespace BeatThat.Anim 
{
	public abstract class TimedTransitionBase : TransitionBase, TimedTransition
	{
		protected TimedTransitionBase(float dur = 0.3f)
		{
			this.dur = dur;
		}

		sealed override protected void DoStartTransition(float time)
		{
			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType() 
					+ "::DoStartTransition with duration " + this.dur);
			}

			DoBeforeFirstDisplayUpdate(time);
			this.displayPctComplete = 0f;
		}

		virtual protected void DoBeforeFirstDisplayUpdate(float time) {}

		virtual protected void DoTimedComplete() {}
		
		override protected void DoUpdateTransition(float time, float deltaTime)
		{	
			this.timeIn = Mathf.Clamp(time - this.startTime, 0f, this.dur);

			this.displayPctComplete = this.pctComplete;
			
			if(timeIn >= this.dur) {
				if(this.debug) {
					Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType() 
						+ "::DoUpdateTransition completing transition: dur=" + this.dur + ", timeIn=" + timeIn);
				}

				Complete();
			}
		}

		sealed override protected void CompleteTransition()
		{
			this.displayPctComplete = 1f;
			DoAfterDisplayComplete();
		}

		virtual protected void DoAfterDisplayComplete() {}
		
		public TimedTransition WithDelay(float d)
		{
			this.delay = d;
			return this;
		}

		public float timeIn { get; protected set; }

		public float timeRem { get { return this.dur - this.timeIn; } }

		override public float progress { get { return this.pctComplete; } } 

		public float pctComplete
		{
			get {
				return this.dur <= 0f ? 0f : Mathf.Clamp01 (this.timeIn / this.dur);
			}
		}
		
		public float dur { get; set; }

		/// <summary>
		/// Override display pct complete to make the transition display differently as it runs.
		/// By default functions like a timer that does nothing.
		/// </summary>
		abstract protected float displayPctComplete { set; }
	}
}

