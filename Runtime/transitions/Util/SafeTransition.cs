using UnityEngine;

namespace BeatThat.Transitions
{

    /// <summary>
    /// Wrapper for a transition that that uses unity game objects that makes the transition 'safe'
    /// in the sense that if the transition's game object is destroyed, 
    /// the transition stops running with no null-ref errors
    /// </summary>
    public class SafeTransition : ProxyTransitionBase
	{
		public SafeTransition(Transition t, GameObject go, System.Func<bool> validTest = null)
		{
			base.proxyTransition = t;
			this.transitionName = t.transitionName;
			this.gameObject = go;
			this.validTest = validTest;

		}
		
		public SafeTransition(Transition t, Component c) : this(t, c.gameObject) {}

		#region implemented abstract members of ProxyTransitionBase
		override protected Transition CreateTransition()
		{
			return this.proxyTransition;
		}
		#endregion

		override protected Transition proxyTransition
		{
			get {
				if(this.gameObject == null || FailsValidTest()) {
					this.proxyTransition = null;
				}

				return base.proxyTransition;
			}
			set {
				base.proxyTransition = value;
			}
		}

		private bool FailsValidTest()
		{
			if(this.validTest == null) {
				return false;
			}
			return !this.validTest();
		}

		protected GameObject gameObject { get; private set; }
		protected System.Func<bool> validTest { get; private set; }
	}
}

