using UnityEngine;
using System.Collections.Generic;
using System;

namespace BeatThat.Anim
{

	/// <summary>
	/// Runs a chain of sub Transitions in sequence.
	/// </summary>
	public class ChainTransition : TransitionBase
	{
		public ChainTransition() {}

		public ChainTransition(string name)
		{
			this.transitionName = name;
		}
		
		override protected void DoStartTransition(float time)
		{
			if(!this.isTransitionRunning) {
				return;
			}

			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "] " + GetType() +"::DoStartTransition " + this.transitionName + " num subtransitions=" + m_subtransitions.Count);
			}

			this.subIx = 0;
			Transition t;
			if(!MoveToIncompleteSub(time, out t)) {
				Complete();
			}
		}
		
		public int subtransitionCount { get { return m_subtransitions.Count; } }

		public ChainTransition Add(Transition subtransition)
		{
			m_subtransitions.Add(subtransition);
			return this;
		}

		/// <summary>
		/// Adds a Transition[s] to the end of the chain
		/// </summary>
		public ChainTransition Add(params Transition[] subtransitions)
		{
			m_subtransitions.AddRange(subtransitions);
			return this;
		}
		
		/// <summary>
		/// Convenience function, adds an action to the end of the chain of transitions
		/// </summary>
		public ChainTransition AddAction(Action a, string name = null)
		{
			var t = new InstantActionTransition(a);
			if(name != null) {
				t.transitionName = name;
			}
			m_subtransitions.Add(t);
			return this;
		}
		
		/// <summary>
		/// Adds a Transition to the end of the chain IFF it is not null.
		/// </summary>
		public ChainTransition AddNotNull(Transition sub)
		{
			if(sub != null) {
				m_subtransitions.Add(sub);
			}
			return this;
		}
		
		/// <summary>
		/// Add a transition[s] to the end of the chain "just in time", e.g. a Transition factory function 
		/// is passed in and no associated Transition is created until it is time to start that Transition.
		/// This is necessary for chained transitions where some subtransition in the chain
		/// uses, say, GUI components that do not and must not be instantiated until the transition runs.
		/// </summary>
		public ChainTransition AddJIT(params Func<Transition>[] subtransitions)
		{
			if(subtransitions != null) {
				foreach(Func<Transition> tfac in subtransitions) {
					if(tfac != null) {
						m_subtransitions.Add(new JITTransition(tfac));
					}
				}
			}
			return this;
		}
		
		/// <summary>
		/// Inserts a Transition[s] at the given chain index
		/// </summary>
		public ChainTransition Insert(int index, params Transition[] subtransitions)
		{
			m_subtransitions.InsertRange(index, subtransitions);
			return this;
		}
		
		override protected void DoUpdateTransition(float time, float deltaTime)
		{
			if(!this.isTransitionRunning) {
				Debug.LogWarning("ChainTransition::DoUpdateTransition " + this.transitionName + " is NOT running!");
				return;
			}

			Transition t;
			if(!MoveToIncompleteSub(time, out t)) {
				if(this.debug) {
					Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType() + "::DoUpdateTransition no more active, will complete");
				}

				Complete();
				return;
			}

			while(UpdateFinishesTransition(t, time, deltaTime)) {
				if(!MoveToIncompleteSub(time, out t)) {
					if(this.debug) {
						Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType() + "::DoUpdateTransition no more active, will complete");
					}
					Complete();
					break;
				}
			}
		}
		
		private bool StartSubtransition(Transition t, float time)
		{
			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType() 
					+ "::StartSubtransition received [" + t.transitionName + "/" + t.GetType() + "]");
			}

			t.StartTransition(time, false); // pass andRun=FALSE to make sure this transition is not updated independently
			return t.isTransitionRunning;
		}
		
		private bool UpdateFinishesTransition(Transition t, float time, float deltaTime)
		{
			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.transitionName + "]" + GetType() 
					+ "::UpdateFinishesTransition received [" + t.transitionName + "/" + t.GetType()  + "] in status " + t.status);
			}

			if(!t.didTransitionStart) {
				t.StartTransition(time, false); // pass andRun=FALSE to make sure this transition is not updated independently
			}

			if(t.isTransitionRunning) { // was cancelled/invalid
				t.UpdateTransition(time, deltaTime);
			}

			return !t.isTransitionRunning;
		}
		
		override protected void CompleteTransition()
		{
			foreach(Transition st in m_subtransitions) {
				if(!st.isTransitionComplete) {
					st.Complete();
				}
			}
		}
		
		override protected void CompleteTransitionEarly()
		{
			foreach(Transition st in m_subtransitions) {
				if(!st.isTransitionComplete) {
					st.CompleteEarly();
				}
			}
		}
		
		protected bool MoveToIncompleteSub(float time, out Transition t)
		{
			for(; this.subIx < m_subtransitions.Count; this.subIx++) {
				t = m_subtransitions[this.subIx];
				if(t == null) {
					if(this.debug) {
						Debug.LogWarning("ChainTransition::MoveToIncompleteSub " + this.transitionName + " transition[" + this.subIx + "] is null");
					}
					continue;
				}

				if(t.isTransitionComplete) {
					if(this.debug) {
						Debug.Log("ChainTransition::MoveToIncompleteSub " + this.transitionName 
							+ " transition[" + this.subIx + "][" + t.transitionName + "/" + t.GetType() + "] is complete");
					}
					continue;
				}

				if(t.isTransitionRunning) {
					return true;
				}

				if(this.debug) {
					Debug.Log("ChainTransition::MoveToIncompleteSub " + this.transitionName 
						+ " attempted to start transition[" + this.subIx + "][" + t.transitionName + "/" + t.GetType() + "]");
				}

				if(StartSubtransition(t, time)) {
					if(this.debug) {
						Debug.Log("ChainTransition::MoveToIncompleteSub " + this.transitionName 
							+ " attempted to start transition[" + this.subIx + "][" + t.transitionName + "/" + t.GetType() + "] is running");
					}
					return true;
				}
			}

			t = null;
			return false;
		}
		
		protected int subIx { get; private set; }
		
		private readonly List<Transition> m_subtransitions = new List<Transition>();
	}
}
