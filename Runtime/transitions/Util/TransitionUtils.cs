using System.Collections.Generic;
using BeatThat.Pools;
using UnityEngine;

namespace BeatThat.Transitions
{
    public static class TransitionUtils 
	{
		public static Dictionary<string, Transition[]> FindAndGroupTransitions(Transform tr)
		{
			var d = new Dictionary<string, Transition[]>();
			
			int missingNameSeq = 1;

			using(var transitions = ListPool<Transition>.Get()) {

				tr.GetComponents<Transition>(transitions);

				foreach(Transition t in transitions) {
					
					if(string.IsNullOrEmpty(t.transitionName)) {
						t.transitionName = "unset-" + missingNameSeq;
						missingNameSeq++;
					}
					
					Transition[] existingList;
					if(d.TryGetValue(t.transitionName, out existingList)) {
						Transition[] newList = new Transition[existingList.Length + 1];
						
						System.Array.Copy(existingList, newList, existingList.Length);
						newList[newList.Length - 1] = t;
						d[t.transitionName] = newList;
					}
					else {
						d[t.transitionName] = new Transition[] { t };
					}
				}
			}
			
			return d;
		}
	}
}


