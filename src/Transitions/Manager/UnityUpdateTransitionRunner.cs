using UnityEngine;
using System.Collections.Generic;
using BeatThat.Anim;

namespace BeatThat.Anim
{
	/// <summary>
	/// Simple global transition runner that steps transistion on unity update;
	/// </summary>
	public class UnityUpdateTransitionRunner : MonoBehaviour, TransitionRunnerImpl
	{
		public event System.Action<TransitionRunnerImpl> Destroyed;

		public void AddTransition(Transition t)
		{
			m_runningTransitions.Add(t);
			this.enabled = true;
		}

		void Awake()
		{
			TransitionRunner.RunnerAwoke(this);
		}
			
		void Update()
		{
			if(m_runningTransitions.Count == 0) {
				this.enabled = false;
				return;
			}

			for(int i = m_runningTransitions.Count - 1; i >= 0; i--) {
				Transition t = m_runningTransitions[i];
				if(t.isTransitionRunning) {
					t.UpdateTransition(Time.time, Time.deltaTime);
				}
				
				if(!t.isTransitionRunning) {
					m_runningTransitions.RemoveAt(i);
				}
			}
			if(Input.GetKeyDown(KeyCode.T)) {
				Debug.Log ("[" + Time.time + "] " + GetType() + "::UnityUpdateTransitionRunner nrunning=" 
				           + m_runningTransitions.Count);
			}
		}
		
		void OnDestroy()
		{
			m_runningTransitions.Clear();

			if(this.Destroyed != null) {
				Destroyed(this);
			}
		}
		
		private readonly List<Transition> m_runningTransitions = new List<Transition>();
	}
}
