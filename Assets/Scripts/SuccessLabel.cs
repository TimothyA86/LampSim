using UnityEngine;
using UnityEngine.UI;

public class SuccessLabel : MonoBehaviour
{
	[SerializeField] private Text text;
	[SerializeField] private LampAgent lampAgent;

	private void Awake()
	{
		text.text = "Successful: 0\n";
		text.text += "Success Rate: 0%\n";
	}

	private void OnEnable()
	{
		lampAgent.ActionWasTaken += UpdateLabel;
	}

	private void OnDisable()
	{
		lampAgent.ActionWasTaken -= UpdateLabel;
	}

	private void UpdateLabel()
	{
		text.text = "Successful: " + lampAgent.SuccessFulActions + "\n";
		text.text += "Success Rate: " + (float)lampAgent.SuccessFulActions / lampAgent.ActionsTaken * 100f + "%\n";
	}
}
