using UnityEngine;

namespace Assets.Scripts.Sounds
{
	public class PitchedSound : MonoBehaviour
	{
		[SerializeField]
		protected float pitchVariation = 0.1f;
		[SerializeField]
		protected AudioSource source;

		private float startPitch;

		void Start()
		{
			startPitch = source.pitch;
		}

		public void playWithRandomPitch()
		{
			playWithRandomPitch(pitchVariation);
		}

		public void playWithRandomPitch(float variation)
		{
			source.pitch = startPitch + Random.Range(-pitchVariation, pitchVariation);
			source.Play();
		}
	}
}
