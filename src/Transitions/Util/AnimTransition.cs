using UnityEngine;

namespace BeatThat.Anim 
{
	public class AnimTransition : TransitionBase 
	{
		public AnimTransition(Animation anim, AnimationClip clip, string name) 
			: this(anim, clip, name, false) 
		{
		}
		
		public AnimTransition(Animation anim, AnimationClip clip, string name, bool reverse) 	
		{
			m_animation = anim;
			m_animationGo = m_animation.gameObject;
			m_clip = clip;
			this.transitionName = name;
			m_reverse = reverse;
		}
		
		override protected void DoStartTransition(float time)
		{
			Animation a = this.anim;
			if(a != null) {
				a.Play(m_clip.name);
				
				if(m_reverse) {
					a[m_clip.name].time = a[m_clip.name].length;
					a[m_clip.name].speed = -1f;
				}
				else {
					a[m_clip.name].time = 0f;
					a[m_clip.name].speed = 1f;
				}
			}
		}
		
		override protected void DoUpdateTransition(float time, float deltaTime)
		{
			if(this.debug) {
				Debug.Log("[" + Time.frameCount + "] " + GetType() + "::DoUpdateTransition time=" + time + ", deltaTime=" + deltaTime + ", status=" + this.status);
			}

			Animation a = this.anim;

			if(a == null || !a.IsPlaying(m_clip.name)) {
				Complete();
			}
			else {
				if(m_reverse) {
					if(a[m_clip.name].time <= 0f) {
						Complete();
					}
				}
				else {
					if(a[m_clip.name].time >= m_clip.length) {
						Complete();
					}
				}
			}
		}
			
		override protected void CompleteTransitionEarly()
		{	
			Animation a = this.anim;

			if(a != null) {
				if(!a.IsPlaying(m_clip.name)) {
					a.Play(m_clip.name);
				}
				
				a[m_clip.name].normalizedTime = (m_reverse)? 0f: 1f;
				a.Sample();
			}
		}

		private Animation anim
		{
			get {
				return m_animationGo == null ? null : m_animation;
			}
		}
		
		private bool m_reverse;
		private readonly Animation m_animation;
		private readonly GameObject m_animationGo;
		private AnimationClip m_clip;
	}
}
