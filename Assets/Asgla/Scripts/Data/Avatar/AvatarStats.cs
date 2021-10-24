using System;

namespace Asgla.Data.Avatar {

	[Serializable]
	public class AvatarStats {

		public float Health = -1;
		public float HealthMax = -1;

		public float Energy = -1;
		public float EnergyMax = -1;

		public void Restore() {
			SetHealth(HealthMax);
			SetEnergy(EnergyMax);
		}

		public bool IsFullEnergy() {
			return Energy >= HealthMax;
		}

		public bool IsFullHealth() {
			return Health >= HealthMax;
		}

		public void DecreaseHealth(float amount) {
			SetHealth(Health - amount);
		}

		public void DecreaseEnergy(float amount) {
			SetEnergy(Energy - amount);
		}

		public void IncreaseHealth(float amount) {
			SetHealth(Health + amount);
		}

		public void IncreaseEnergy(float amount) {
			SetEnergy(Energy + amount);
		}

		public void SetHealth(float h) {
			Health = h;

			if (Health <= 0)
				Health = 0;
			else if (Health > HealthMax)
				Health = HealthMax;
		}

		public void SetEnergy(float e) {
			Energy = e;
			if (Energy < 0)
				Energy = 0;
			else if (Energy > EnergyMax)
				Energy = EnergyMax;
		}

	}

}