using UnityEngine;
namespace Dobeil
{

	[System.Serializable]
	public class TargetData
	{
		public Transform transform;
		public Health health;
		public TargetData() { }
		public TargetData(Transform _transform)
		{
			transform = _transform;
		}
		public TargetData(Transform _transform, Health _helth)
		{
			transform = _transform;
			health = _helth;
		}
	}
}