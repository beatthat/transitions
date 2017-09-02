using UnityEngine;

namespace BeatThat.Anim 
{
	/// <summary>
	/// Just functions like a timer. Doesn't display anything
	/// </summary>
	public class TimerTransition : TimedTransitionBase
	{
		override protected float displayPctComplete
		{
			set {
			}
		}
	}
}

