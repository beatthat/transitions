using System.Collections.Generic;
using System;

namespace BeatThat.Anim 
{
	// Functions like an if/else statement, where the first IF condition that returns TRUE (if any) is the transition that executes
	public class ConditionalTransition : ProxyTransitionBase
	{	
		public ConditionalTransition If(Func<bool> ifCond, Transition thenTrans)
		{
			m_ifBranches.Add (new ConditionTransitionPair(ifCond, thenTrans));
			return this;
		}
		
		public ConditionalTransition If_JIT(Func<bool> ifCond, Func<Transition> thenTrans)
		{
			return If(ifCond, new JITTransition(thenTrans));
		}
		
		public ConditionalTransition Else(Transition elseTrans)
		{
			m_elseTransition = elseTrans;
			return this;
		}
		
		public ConditionalTransition Else_JIT(Func<Transition> elseTrans)
		{
			return Else(new JITTransition(elseTrans));
		}
		
		override protected Transition CreateTransition()
		{
			foreach(ConditionTransitionPair ctp in m_ifBranches) {
				if(ctp.isConditionMet) {
					return ctp.transition;
				}
			}
			return m_elseTransition;
		}
		
		private readonly List<ConditionTransitionPair> m_ifBranches = new List<ConditionTransitionPair>();
		private Transition m_elseTransition;
	}
	
	// seems like should be a struct but valuetypes + generic data structures cause some headaches on iOS
	public class ConditionTransitionPair 
	{
		public ConditionTransitionPair(Func<bool> condition, Transition t)
		{
			this.transition = t;
			m_condition = condition;
		}
		
		public bool isConditionMet
		{
			get {
				return m_condition();
			}
		}
		
		public Transition transition
		{
			get; private set;
		}
		
		private readonly Func<bool> m_condition;
	}
}

