//using UnityEngine;
//using System.Collections;
//
//namespace BeatThat.Anim 
//{
//	/// <summary>
//	/// Transition wrapper for a coroutine
//	/// </summary>
//	public class CoroutineTransition : TransitionBase
//	{
//		public delegate IEnumerator CoroutineAction();
//		
//		public CoroutineTransition(MonoBehaviour runner, CoroutineAction coroutine)
//		{
//			m_runner = runner;
//			m_coroutine = coroutine;
//		}
//		
//		override protected void DoStartTransition(float time)
//		{
//			m_runner.StartCoroutine(RunCoroutine());
//		}
//		
//		override protected void DoUpdateTransition(float time, float dtime)
//		{
//		}
//		
//		private IEnumerator RunCoroutine()
//		{
//			yield return m_runner.StartCoroutine(m_coroutine());
//			Complete();
//		}
//		
//		private readonly CoroutineAction m_coroutine;
//		private readonly MonoBehaviour m_runner;
//	}
//}
//
