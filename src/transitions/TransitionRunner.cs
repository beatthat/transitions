using UnityEngine;

namespace BeatThat.Anim
{
	/// <summary>
	/// Global singleton transition runner. Ensures transitions complete their lifecycles correctly regardless of scene state.
	/// </summary>
	public static class TransitionRunner
	{
		/// <summary>
		/// Updates a transition until complete. Does NOT call Transition.StartTranstion.
		/// </param>
		public static void AddTransition(Transition t)
		{
			var runner = TransitionRunner.impl;
			if(runner == null) {
				Debug.LogWarning("AddTransition called when runner is null, (in the process of App shutdown?)");
				return;
			}

			runner.AddTransition(t);
		}

		public static TransitionRunnerImpl impl
		{
			get {
				var t = m_impl.value;
				if(t != null) {
					return t;
				}

				if(TransitionRunner.hasInit) {
					// app shut down, don't create a new runner
					return null;
				}

				TransitionRunnerImpl tr = GameObjectUtils.FindObjectOfType<TransitionRunnerImpl>();

				if(tr == null) {
					tr = new GameObject("TransitionRunner").AddComponent<UnityUpdateTransitionRunner>();
				}

				TransitionRunner.impl = tr;

				return tr;
			}
			private set {
				if(m_impl.value != null) {
					m_impl.value.Destroyed -= TransitionRunner.OnRunnerDestroyed;
				}

				m_impl.value = value;

				if(value is Component) {
					Object.DontDestroyOnLoad(((Component)value).gameObject);
				}

				if(value != null) {
					value.Destroyed += TransitionRunner.OnRunnerDestroyed;
					TransitionRunner.hasInit = true;
				}
			}
		}

		public static bool hasInit
		{
			get; private set;
		}

		public static void RunnerAwoke(TransitionRunnerImpl t)
		{
			m_impl.value = t;
		}

		private static void OnRunnerDestroyed(TransitionRunnerImpl t)
		{
			TransitionRunner.impl = null;
		}

		private static SafeRef<TransitionRunnerImpl> m_impl = new SafeRef<TransitionRunnerImpl>(null);
	}

}
