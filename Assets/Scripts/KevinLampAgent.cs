using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class KevinLampAgent : LampAgent
{
	protected override float EvaluateAction(float[] vectorAction, out ActionResultFlags resultFlags)
	{
		//resultFlags = ActionResultFlags.None;

		//currentTargetViewportPosition = lamp.WorldToViewportPoint(target.position);
		//bool targetIsInFrontOfCamera = currentTargetViewportPosition.z > 0;
		//float currentSqrOffset = ((Vector2)currentTargetViewportPosition).sqrMagnitude;
		//float maxSqrOffset = lamp.ViewPortExtends.sqrMagnitude;

		//if (targetIsInFrontOfCamera)
		//{
		//	if (currentSqrOffset <= academy.SuccessThreshold)
		//	{
		//		resultFlags |= ActionResultFlags.Success;
		//		return 1f;
		//	}

		//	return 1f - Mathf.Clamp01(currentSqrOffset / maxSqrOffset);
		//}

		//return -1f + Mathf.Clamp01(currentSqrOffset / maxSqrOffset);
		// ------------------------------------------------------------------------------------------------------------
		//resultFlags = ActionResultFlags.None;

		//currentTargetViewportPosition = lamp.WorldToViewportPoint(target.position);
		//bool targetIsInFrontOfCamera = currentTargetViewportPosition.z > 0;
		//float initialSqrOffset = ((Vector2)initialTargetViewportPosition).sqrMagnitude;
		//float currentSqrOffset = ((Vector2)currentTargetViewportPosition).sqrMagnitude;

		//if (targetIsInFrontOfCamera && currentSqrOffset <= academy.SuccessThreshold)
		//{
		//	resultFlags |= ActionResultFlags.Success;
		//	return 1f;
		//}

		//return targetIsInFrontOfCamera ? -Mathf.Clamp01(currentSqrOffset / initialSqrOffset) : -1f;
		// ------------------------------------------------------------------------------------------------------------
		resultFlags = ActionResultFlags.None;

		currentTargetViewportPosition = lamp.WorldToViewportPoint(target.position);
		bool targetIsInFrontOfCamera = currentTargetViewportPosition.z > 0;
		float initialSqrOffset = ((Vector2)initialTargetViewportPosition).sqrMagnitude;
		float currentSqrOffset = ((Vector2)currentTargetViewportPosition).sqrMagnitude;
		float maxSqrOffset = lamp.ViewPortExtends.sqrMagnitude;

		if (Mathf.Abs(currentTargetViewportPosition.x) <= 1f && Mathf.Abs(currentTargetViewportPosition.y) <= 1f)
		{
			if (targetIsInFrontOfCamera)
			{
				if (currentSqrOffset <= initialSqrOffset)
				{
					float reward = Mathf.Pow(0.1f, currentSqrOffset / initialSqrOffset);

					if (currentSqrOffset <= academy.SuccessThreshold)
					{
						resultFlags |= ActionResultFlags.Success;
						reward = 1f;
					}

					return reward;
				}
			}
		}

		return -1f;
	}
}