namespace BeatThat.Transitions
{
    public interface MultiTransition : Transition
	{
		void AddT(Transition t);

		void AddA(System.Action a, string name = null);

		bool hasSubtransitions { get; }
	}
}

