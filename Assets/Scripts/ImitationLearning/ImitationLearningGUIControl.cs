using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ImitationLearningGUIControl : MonoBehaviour
{
	private enum EState
	{
		None,
		SetInitialServoOffsets,
		SetTargetPosition,
		SetFinalServoOffsets,
		DoAction
	}

	private readonly string[] NextStateButtonNames = new string[] { "Next", "Set Initial", "Set Target", "Set Final", "Do Action" };

	[SerializeField] private LampAgent teacherAgent;
	[SerializeField] private LampAgent studentAgent;
	[SerializeField] private TeacherBrainDecision decision;
	[SerializeField] private ServoSlidersParent servoSlidersParent;
	[SerializeField] private SelectableParent targetSlidersParent;
	[SerializeField] private Button previousStateButton;
	[SerializeField] private Button nextStateButton;

	private Text nextStateButtonText;
	private EState state = EState.None;
	private Vector2 targetPosition = Vector2.zero;
	private float[] initialServoOffsets = new float[Lamp.NumberOfServos];
	private float[] finalServoOffsets = new float[Lamp.NumberOfServos];

	private void Start()
	{
		nextStateButtonText = nextStateButton.GetComponentInChildren<Text>();

		// When a teacher has taken action, we request that the student also takes that action
		// Also move the gui to its next state (re-activate UI)
		teacherAgent.ActionWasTaken += RequestStudentDecision;
		teacherAgent.ActionWasTaken += GoToNextState;

		GoToNextState();
	}

	private void SetLampServoOffset(int servo, float normalizedOffset)
	{
		teacherAgent.ControlledLamp.GetServo(servo).SetOffsetNormalized(normalizedOffset);
	}

	private void DoAction()
	{
	// Set the teacher and students initial state
	// Trigger the teacher to take the action

		decision.SetActions(finalServoOffsets);
		teacherAgent.SetServoPositionsNormalizedAndUpdateState(initialServoOffsets);
		teacherAgent.RequestDecision();

		studentAgent.SetServoPositionsNormalizedAndUpdateState(initialServoOffsets);
		studentAgent.SetTarget(targetPosition);
	}

	private void RequestStudentDecision()
	{
		studentAgent.RequestDecision();
	}

	#region GoToState
	public void GoToNextState()
	{
		switch (state)
		{
		case EState.None:
			servoSlidersParent.SetChildrenInteractable(true);
			targetSlidersParent.SetChildrenInteractable(false);
			previousStateButton.interactable = false;
			nextStateButton.interactable = true;
			state = EState.SetInitialServoOffsets;
			break;
		case EState.SetInitialServoOffsets:
			servoSlidersParent.SetChildrenInteractable(false);
			targetSlidersParent.SetChildrenInteractable(true);
			previousStateButton.interactable = true;
			teacherAgent.GetNormalizedServoPositionsNonAlloc(initialServoOffsets);
			ResetTargetPosition();
			state = EState.SetTargetPosition;
			break;
		case EState.SetTargetPosition:
			servoSlidersParent.SetChildrenInteractable(true);
			targetSlidersParent.SetChildrenInteractable(false);
			state = EState.SetFinalServoOffsets;
			break;
		case EState.SetFinalServoOffsets:
			servoSlidersParent.SetChildrenInteractable(false);
			targetSlidersParent.SetChildrenInteractable(false);
			teacherAgent.GetNormalizedServoPositionsNonAlloc(finalServoOffsets);
			state = EState.DoAction;
			break;
		case EState.DoAction:
			previousStateButton.interactable = false;
			nextStateButton.interactable = false;
			state = EState.None;
			DoAction();
			break;
		}

		nextStateButtonText.text = NextStateButtonNames[(int)state];
	}

	public void GoToPreviousState()
	{
		switch (state)
		{
		case EState.None:
			break;
		case EState.SetInitialServoOffsets:
			break;
		case EState.SetTargetPosition:
			servoSlidersParent.SetChildrenInteractable(true);
			targetSlidersParent.SetChildrenInteractable(false);
			previousStateButton.interactable = false;
			state = EState.SetInitialServoOffsets;
			break;
		case EState.SetFinalServoOffsets:
			servoSlidersParent.SetChildrenInteractable(false);
			targetSlidersParent.SetChildrenInteractable(true);
			SetServoSlidersToCurrentOffsets();
			state = EState.SetTargetPosition;
			break;
		case EState.DoAction:
			servoSlidersParent.SetChildrenInteractable(true);
			targetSlidersParent.SetChildrenInteractable(false);
			state = EState.SetFinalServoOffsets;
			break;
		}

		nextStateButtonText.text = NextStateButtonNames[(int)state];
	}
	#endregion

	#region SetServosOffset
	// SetLampServo#Offset functions are used within the Unity Editor in the GUI's event system

	public void SetLampServo0Offset(float normalizedOffset)
	{
		SetLampServoOffset(0, normalizedOffset);
	}

	public void SetLampServo1Offset(float normalizedOffset)
	{
		SetLampServoOffset(1, normalizedOffset);
	}

	public void SetLampServo2Offset(float normalizedOffset)
	{
		SetLampServoOffset(2, normalizedOffset);
	}

	public void SetLampServo3Offset(float normalizedOffset)
	{
		SetLampServoOffset(3, normalizedOffset);
	}

	public void SetLampServo4Offset(float normalizedOffset)
	{
		SetLampServoOffset(4, normalizedOffset);
	}

	private void SetServoSlidersToCurrentOffsets()
	{
	// Set slider positions based on servo positions

		int maxServoIndex = Lamp.NumberOfServos - 1;
		Slider slider;

		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			slider = servoSlidersParent.GetSlider(i);
			slider.value = initialServoOffsets[maxServoIndex - i];
			slider.onValueChanged.Invoke(slider.value);
		}
	}
	#endregion

	#region SetTargetPosition
	public void SetTargetViewportPositionX(float x)
	{
		targetPosition.x = x;
		teacherAgent.SetTarget(targetPosition);
	}

	public void SetTargetViewportPositionY(float y)
	{
		targetPosition.y = y;
		teacherAgent.SetTarget(targetPosition);
	}

	private void ResetTargetPosition()
	{
	// Set target back to the center of the camera view and zero out sliders

		targetPosition = Vector2.zero;
		teacherAgent.SetTarget(targetPosition);

		foreach (var slider in targetSlidersParent.GetComponentsInChildren<Slider>())
		{
			slider.value = 0f;
		}
	}
	#endregion
}
