using System.Collections.Generic;
using System;

namespace BeatThat.Anim 
{
	/// <summary>
	/// Runs transitions simultaneous as a group/barrier and does not complete until all subtransitions are complete
	/// </summary>
	public class JoinTransition : TransitionBase
	{
		
		override protected void DoStartTransition(float time)
		{
			bool allComplete = true;
			foreach(Transition t in m_subtransitions) {
				allComplete &= !StartSubtransition (t, time);
			}
			
			if(allComplete) {
				Complete();
			}
		}

		public JoinTransition Add(Transition t)
		{
			m_subtransitions.Add(t);
			return this;
		}

//			public JoinTransition Add(params Transition[] subtransitions)
//			{
//				foreach(var t in subtransitions) {
//					if(t == null) {
//						Debug.LogWarning("Illegal null sub transition");
//						continue;
//					}
//					m_subtransitions.Add(t);
//				}
//				return this;
//			}
		
		/// <summary>
		/// Convenience function, adds an action to group of transitions
		/// </summary>
		public JoinTransition AddAction(Action a)
		{
			m_subtransitions.Add(new InstantActionTransition(a));
			return this;
		}
		
		/// <summary>
		/// Add a transition[s] to the group "just in time", e.g. a Transition factory function 
		/// is passed in and no associated Transition is created until it is time to start that Transition.
		/// </summary>
		public JoinTransition AddJIT(params System.Func<Transition>[] subtransitions)
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
		
		override protected void DoUpdateTransition(float time, float deltaTime)
		{
			if(!this.isTransitionRunning) {
				return;
			}

			bool allComplete = true;
			foreach(Transition t in m_subtransitions) {
				if(t.isTransitionRunning) { 
					t.UpdateTransition(time, deltaTime);
				}
				if(t.isTransitionRunning) {
					allComplete = false;
				}
			}
			
			if(allComplete) {
				Complete();
			}
		}
		
		override protected void CompleteTransition()
		{				
			foreach(Transition st in m_subtransitions) {
				if(st == null) {
					continue;
				}

				if(!st.isTransitionComplete) {
					st.Complete();
				}
			}
		}
		
		override protected void CompleteTransitionEarly()
		{				
			foreach(Transition st in m_subtransitions) {
				if(st == null) {
					continue;
				}

				if(!st.isTransitionComplete) {
					st.CompleteEarly();
				}
			}
		}
		
		private static bool StartSubtransition(Transition t, float time)
		{
			if(t == null) {
				return false;
			}
			t.StartTransition(time, false); // pass andRun=FALSE to make sure this transition is not updated independently
			return t.isTransitionRunning;
		}
		
		
		private List<Transition> m_subtransitions = new List<Transition>();
	}
}
