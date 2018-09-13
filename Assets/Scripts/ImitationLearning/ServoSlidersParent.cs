using UnityEngine.UI;
using UnityEngine.Assertions;

public class ServoSlidersParent : SelectableParent
{
	private Slider[] sliders;

	protected override void Awake()
	{
		base.Awake();
		sliders = GetComponentsInChildren<Slider>();
		Assert.IsTrue(sliders.Length == Lamp.NumberOfServos, "ERROR [" + name + "] There should be exactly one slider per servo.");
	}

	public Slider GetSlider(int index)
	{
		Assert.IsTrue(index >= 0 && index < Lamp.NumberOfServos, "ERROR [" + name + "] Trying to access servo slider [" + index + "] that does not exist.");
		return sliders[index];
	}
}
