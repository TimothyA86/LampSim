using UnityEngine;

public class LampTrainingAcademy : Academy
{
	[SerializeField, Range(0f, 1f)] private float defaultSuccessThreshold = 0.01f;

	public float SuccessThreshold
	{
		get
		{
			float threshold;

			if (resetParameters.TryGetValue("SuccessThreshold", out threshold))
			{
				return threshold;
			}

			return defaultSuccessThreshold;
		}
	}

	public override void InitializeAcademy()
	{
		base.InitializeAcademy();
		resetParameters.Add("SuccessThreshold", defaultSuccessThreshold);
	}
}
