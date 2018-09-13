using UnityEngine;

public class RunInBackground : MonoBehaviour
{
// Used to keep the applicatio running during training sessions even if the window does not have focus

	[SerializeField] private bool runInBackground;

	private void Awake()
	{
		Application.runInBackground = runInBackground;
	}
}