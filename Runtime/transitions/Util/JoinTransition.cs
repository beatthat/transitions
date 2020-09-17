using BeatThat.Requests;
using System;
using System.Collections.Generic;

namespace BeatThat.Transitions
{
    /// <summary>
    /// Runs transitions simultaneous as a group/barrier and does not complete until all subtransitions are complete
    /// </summary>
    public class JoinTransition : TransitionBase, MultiTransition
    {
        public JoinTransition() {}

        public JoinTransition(string name)
        {
            this.transitionName = name;
        }

        override protected void DoStartTransition(float time)
        {
            bool allComplete = true;
            foreach (Transition t in m_subtransitions)
            {
                allComplete &= !StartSubtransition(t, time);
            }
            if (allComplete)
            {
                Complete();
            }
        }

        public void AddT(Transition t)
        {
            Add(t);
        }

        public JoinTransition Add(Transition t)
        {
            m_subtransitions.Add(t);
            return this;
        }

        public void AddA(Action a, string name = null)
        {
            AddAction(a, name);
        }

        /// <summary>
        /// Convenience function, adds an action to group of transitions
        /// </summary>
        public JoinTransition AddAction(Action a, string name = null)
        {
            var t = new InstantActionTransition(a);
            if (name != null)
            {
                t.transitionName = name;
            }
            m_subtransitions.Add(t);
            return this;
        }

        /// <summary>
        /// Add a transition[s] to the group "just in time", e.g. a Transition factory function 
        /// is passed in and no associated Transition is created until it is time to start that Transition.
        /// </summary>
        public JoinTransition AddJIT(params System.Func<Transition>[] subtransitions)
        {
            if (subtransitions != null)
            {
                foreach (Func<Transition> tfac in subtransitions)
                {
                    if (tfac != null)
                    {
                        m_subtransitions.Add(new JITTransition(tfac));
                    }
                }
            }
            return this;
        }

        override protected void DoUpdateTransition(float time, float deltaTime)
        {
            if (!this.isTransitionRunning)
            {
                return;
            }

            bool allComplete = true;
            foreach (Transition t in m_subtransitions)
            {
                if (t.isTransitionRunning)
                {
                    t.UpdateTransition(time, deltaTime);
                }
                if (t.isTransitionRunning)
                {
                    allComplete = false;
                }
            }

            if (allComplete)
            {
                Complete();
            }
        }

        override protected void CompleteTransition()
        {
            foreach (Transition st in m_subtransitions)
            {
                if (st == null)
                {
                    continue;
                }
                if (!(st as Request).IsDoneOrCancelled())
                {
                    st.Complete();
                }
            }
        }

        override protected void CompleteTransitionEarly()
        {
            foreach (Transition st in m_subtransitions)
            {
                if (st == null)
                {
                    continue;
                }
                if (!(st as Request).IsDoneOrCancelled())
                {
                    st.CompleteEarly();
                }
            }
        }

        private static bool StartSubtransition(Transition t, float time)
        {
            if (t == null)
            {
                return false;
            }
            t.StartTransition(time, false); // pass andRun=FALSE to make sure this transition is not updated independently
            return t.isTransitionRunning;
        }

        public bool hasSubtransitions { get { return this.subtransitionCount > 0; } }

        public int subtransitionCount { get { return m_subtransitions.Count; } }


        private List<Transition> m_subtransitions = new List<Transition>();
    }
}

