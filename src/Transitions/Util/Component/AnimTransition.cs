using UnityEngine;

using BeatThat.Anim;

namespace BeatThat.Anim.Comp
{
	public class AnimTransition : TransitionBase
	{
		public Animation m_animation;
		public bool m_reverse = false;
		public string m_clipName;
		
		public bool reverse
		{
			get {
				return m_reverse;
			}
			set {
				m_reverse = value;
			}
		}
		
		public Animation anim
		{
			get {
				if(m_animation == null) {
					return GetComponent<Animation>();
				}
				return m_animation;
			}
		}
		
		public string clipName
		{
			get {
				if(!string.IsNullOrEmpty(m_clipName)) {
					return m_clipName;
				}
				return this.transitionName;
			}
			set {
				m_clipName = value;
				m_transition = null;
			}
		}
		
		public AnimationClip clip
		{
			get {
				if(this.clipName != null && this.anim != null) {
					return this.anim.GetClip(this.clipName);
				}
				else {
					return null;
				}
			}
		}
		
		
		override protected Transition transitionDelegate
		{
			get {
				if(m_transition == null) {
					AnimationClip c = this.clip;
					
					if(c == null) {
						Debug.LogWarning("[" + Time.time + "] " + this.Path()
							+ " no clip for name '" + this.clipName + "'");
						
						return new InstantActionTransition(() => {});
					}
					
					m_transition = new BeatThat.Anim.AnimTransition(this.anim, c, this.transitionName, this.reverse);
				}
				
				return m_transition;
			}
		}
		
	#if UNITY_EDITOR
		public void EditorRefresh()
		{
			if(!Application.isPlaying) {
				m_transition = null;
				if(m_animation != null && m_animation == this.transform.GetComponent<Animation>()) {
					m_animation = null;
				}
			}
		}
	#endif
		
		private BeatThat.Anim.AnimTransition m_transition;
	}
}
