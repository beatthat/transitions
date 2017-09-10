
namespace BeatThat.Anim
{
		/// <summary>
	/// For case where you want a single transition to run at a time, 
	/// and if a new transition starts before the old one ends, the old transition needs to be ended safely
	/// by calling Transition.Complete()
	/// </summary>
	public class StateTransitionRunner 
	{
		
		public bool hasTransitionRunning
		{
			get {
				return m_activeTransition != null && m_activeTransition.isTransitionRunning;
			}
		}
		
		public void StartTransition(Transition t, float time)
		{
			if(m_activeTransition != null) {
				m_activeTransition.CompleteEarly();
			}
			
			m_activeTransition = t;
			if(m_activeTransition != null) {
				m_activeTransition.StartTransition(time, false);
			}
		}
		
		public void Update(float time, float deltaTime)
		{
			if(m_activeTransition != null) {
				if(m_activeTransition.isTransitionRunning) {
					m_activeTransition.UpdateTransition(time, deltaTime);
				}
				if(!m_activeTransition.isTransitionRunning) {
					m_activeTransition = null;
				}
			}
		}
		
		private Transition m_activeTransition;

	}
}
