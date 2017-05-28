using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Math
{
	public static class MathUtil
	{
		public static bool eq(float f1, float f2)
		{
			return f1 == f2 || Mathf.Approximately(f1, f2);
		}

		/// <summary>
		/// An easing function with a quadratic in/out smoothing ranging from 0 to 1 in both x and y.
		/// The Integral of this function between 0 and 1 is 0.5
		/// </summary>
		/// <param name="t">The time parameter of the easing, should be between 0 and 1</param>
		/// <returns>The eased value between 0 and 1.</returns>
		public static float quadraticEasingInOut(float t)
		{
			if (t < 0.5f) return 4f*t*t/2f;
			else return (2f - (2f*t - 2f)*(2f*t - 2f))/2f;
		}
	}
}