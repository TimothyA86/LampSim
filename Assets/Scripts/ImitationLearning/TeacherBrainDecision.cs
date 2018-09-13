using System.Collections.Generic;
using UnityEngine;

public class TeacherBrainDecision : MonoBehaviour, Decision
{
// Simply "decides" actions that are given to it via SetActions()
// Used duing imitation learning training

	private List<float> actions = new List<float>();

	public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
	{
		return actions.ToArray();
	}

	public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
	{
		// We don't keep track of any previous information
		return new List<float>();
	}

	public void SetActions(float[] newActions)
	{
		actions.Clear();

		for (int i = 0; i < newActions.Length; ++i)
		{
			actions.Add(newActions[i]);
		}
	}
}
