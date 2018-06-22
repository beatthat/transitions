namespace BeatThat.Transitions
{
    // wrapper for a proxy transition that must not be instantiated until StartTransition is called. 
    public class JITTransition : ProxyTransitionBase
	{
		public JITTransition(System.Func<Transition> transitionFactory)
		{
			m_transitionFactory = transitionFactory;
		}
		
		override protected Transition CreateTransition()
		{
			return m_transitionFactory();
		}
		
		private readonly System.Func<Transition> m_transitionFactory;
	}
}


