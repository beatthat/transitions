using BeatThat.Requests;
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
        public SafeTransition(
            Transition t,
            GameObject go,
            System.Func<bool> validTest = null,
            bool treatCompletedTransitionsAsInvaid = true)
        {
            base.proxyTransition = t;
            this.transitionName = t.transitionName;
            this.gameObject = go;
            this.validTest = validTest;
            this.treatCompletedTransitionsAsInvaid = treatCompletedTransitionsAsInvaid;

        }

        public SafeTransition(Transition t, Component c) : this(t, c.gameObject) { }

        override protected Transition CreateTransition()
        {
            return this.proxyTransition;
        }

        private bool treatCompletedTransitionsAsInvaid { get; set; }

        override protected Transition proxyTransition
        {
            get
            {
                var t = base.proxyTransition;
                return (
                    t != null
                    && this.gameObject != null
                    && !ValidTestExistsAndFails()
                    && !(this.treatCompletedTransitionsAsInvaid && (t as Request).IsDoneOrCancelled())
                ) ? t : null;
            }
            set
            {
                base.proxyTransition = value;
            }
        }

        private bool ValidTestExistsAndFails()
        {
            return this.validTest != null && !this.validTest();
        }

        protected GameObject gameObject { get; private set; }
        protected System.Func<bool> validTest { get; private set; }
    }
}

