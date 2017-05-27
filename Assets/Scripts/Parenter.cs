using UnityEngine;

namespace Assets.Scripts
{
	public class Parenter : MonoBehaviour
	{
		[SerializeField]
		private string parentTag;

		void OnEnable()
		{
			GameObject parentObject = GameObject.FindGameObjectWithTag(parentTag);

			if (parentObject == null)
			{
				Debug.LogWarning(string.Format("Cannot find parent with tag '{0}'", parentTag));
			}
			else
			{
				transform.parent = parentObject.transform;
			}
		}
	}
}