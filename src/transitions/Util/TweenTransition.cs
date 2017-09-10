using UnityEngine;

namespace BeatThat.Anim
{
	public abstract class TweenTransition : TimedTransitionBase 
	{	
		protected TweenTransition(float duration = 0.3f, Interpolate.EaseType easeType = Interpolate.EaseType.Linear) : base(duration)
		{
			m_easeType = easeType;
			m_easeFunction = Interpolate.Ease(m_easeType);
		}
		
		override protected float displayPctComplete
		{
			set {
				var easedPct = m_easeFunction(0, 1f, value, 1f);
				DisplayEasedPctComplete(easedPct);
			}
		}

		protected abstract void DisplayEasedPctComplete(float pct);

		private readonly Interpolate.Function m_easeFunction;
		private Interpolate.EaseType m_easeType;
	}
}

