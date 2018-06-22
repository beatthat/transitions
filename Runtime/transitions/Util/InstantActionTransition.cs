using System;

namespace BeatThat.Transitions
{
    /// <summary>
    /// Transition wrapper for an action that completes as soon as the transition starts
    /// </summary>
    public class InstantActionTransition : TransitionBase
	{
		public static readonly Transition DONE = new InstantActionTransition();

		static InstantActionTransition()
		{
			DONE.StartTransition();
		}
		
		public InstantActionTransition(params System.Action[] actions)
		{
			m_actions = actions;
		}
		
		override protected void DoStartTransition(float time)
		{
			Complete();
		}
		
		override protected void DoUpdateTransition(float time, float dtime)
		{
		}
		
		override protected void CompleteTransition()
		{
			if(m_actions == null) {
				return;
			}

			foreach(Action a in m_actions) {
				a();
			}
		}
		
		private readonly System.Action[] m_actions;
		
	}
}


