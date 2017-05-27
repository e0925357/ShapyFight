using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
	public class PlSoundManager : MonoBehaviour
	{
		[SerializeField]
		private AudioClip hurtSound;
		[SerializeField]
		private AudioClip attackSound;
		[SerializeField]
		private AudioClip jumpSound;

		private AudioSource audioS;

		void Start()
		{
			audioS = GetComponentInChildren<AudioSource>();
		}

		public void playSound(SoundType type, float pitchRange = 0f)
		{
			if (pitchRange > 0f)
			{
				audioS.pitch = 1 + Random.Range(-pitchRange, pitchRange);
			}

			switch (type)
			{
				case SoundType.Hurt:
					audioS.clip = hurtSound;
					break;
				case SoundType.Attack:
					audioS.clip = attackSound;
					break;
				case SoundType.Jump:
					audioS.clip = jumpSound;
					break;
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
			}

			audioS.Play();
		}
	}

	public enum SoundType
	{
		Hurt, Attack, Jump
	}
}