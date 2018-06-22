using System.Collections;
using UnityEngine;

namespace BeatThat.Transitions
{
    /// <summary>
    /// Makes it easy to run a transition within a coroutine
    /// </summary>
    public class CoroutineTransitionRunner 
	{
		public CoroutineTransitionRunner(Transition t)
		{
			m_transition = t;
		}
		
		
		public IEnumerator RunTransition()
		{
			m_transition.StartTransition(UnityEngine.Time.time, false);
			
			while(m_transition.isTransitionRunning) {
				m_transition.UpdateTransition(UnityEngine.Time.time, UnityEngine.Time.deltaTime);
				yield return new WaitForEndOfFrame();
			}
		}
		
		private Transition m_transition;
	}
}


