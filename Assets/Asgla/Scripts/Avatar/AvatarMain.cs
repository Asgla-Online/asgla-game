using Asgla.Area;
using Asgla.Data.Avatar;
using Asgla.Data.Avatar.Helper;
using Asgla.Data.Skill;
using Asgla.UI;
using Asgla.UI.Unit_Frame;
using Asgla.Utility;
using UnityEngine;

namespace Asgla.Avatar {
	public abstract class AvatarMain : MonoBehaviour, IAvatar {

		[SerializeField] protected AvatarUtility _utility;

		protected Animator _animator = null;

		protected int _popTextSortingOrder = 1;

		protected Vector2 _position = Vector2.zero;

		protected Rigidbody2D _rigidBody2D = null;

		protected AvatarStats _stats = new AvatarStats();

		protected AreaLocal AreaLocal;

		public virtual void Stats(AvatarStats stats) {
			if (stats != null) {
				if (stats.HealthMax >= 0) {
					Debug.LogFormat("HealthMax>>>>>>>> {0}", stats.HealthMax);
					Utility().SmallUnitFrameOne.Health.SetValueMax(stats.HealthMax);
					Stats().HealthMax = stats.HealthMax;
				}

				if (stats.Health >= 0) {
					Debug.LogFormat("Health>>>>>>>> {0}", stats.Health);
					Utility().SmallUnitFrameOne.Health.SetValue(stats.Health);
					Stats().SetHealth(stats.Health);
				}

				if (stats.EnergyMax >= 0) {
					Debug.LogFormat("EnergyMax>>>>>>>> {0}", stats.EnergyMax);
					Utility().SmallUnitFrameOne.Energy.SetValueMax(stats.EnergyMax);
					Stats().EnergyMax = stats.EnergyMax;
				}

				if (stats.Energy >= 0) {
					Debug.LogFormat("Energy>>>>>>>> {0}", stats.Energy);
					Utility().SmallUnitFrameOne.Energy.SetValue(stats.Energy);
					Stats().SetEnergy(stats.Energy);
				}

				Debug.LogFormat("{0} Select Health {1}, HealthMax {2}", Id(), Stats().Health, Stats().HealthMax);
				Debug.LogFormat("{0} Select Energy {1}, EnergyMax {2}", Id(), Stats().Energy, Stats().EnergyMax);
			}
		}

		public abstract void State(AvatarState state);

		public virtual void Area(AreaLocal areaLocal) {
			AreaLocal = areaLocal;
		}

		public virtual void Move(Vector2 vector) {
			if (State() == AvatarState.DEAD)
				return;

			Position(vector);

			Flip(transform.position.x > _position.x);
		}

		public virtual void Position(Vector2 vector2) {
			_position = vector2;
		}

		public virtual void Scale(float scale) {
			Vector2 vector3 = new Vector3 {x = scale, y = scale, z = scale};
			Animator().transform.localScale = vector3;

			Vector3 scaleFlame = Utility().transform.localScale;
			scaleFlame.Set(scale * 4f, scale * 4f, scale * 4f);
			Utility().transform.localScale = scaleFlame;
		}

		public virtual void OnDeath() {
			Animator().SetTrigger("IsDead");
			_popTextSortingOrder = 1;

			Stats().DecreaseHealth(Stats().Health);
			Stats().DecreaseEnergy(Stats().Energy);
		}

		public virtual void Attack(AvatarMain avatar, int damage) {
			avatar.Stats().DecreaseHealth(damage);
		}

		public virtual void Damaged(int damage, SkillDamageType type) {
			PopupText($"{damage} {type}", type);
		}

		public virtual void PlayAnimation(bool play) {
			Animator().speed = play ? 1 : 0;
		}

		public virtual void Flip(bool flip) {
			Vector3 rotation = Vector3.zero;
			rotation.y = flip ? 180 : (float) 0;
			Animator().transform.eulerAngles = rotation;
		}

		public abstract int Id();

		public abstract int DatabaseId();

		public abstract string Name();

		public abstract int Level();

		public virtual AvatarStats Stats() {
			return _stats;
		}

		public abstract AvatarState State();

		public abstract EntityType Type();

		public virtual AreaLocal Area() {
			return AreaLocal;
		}

		public virtual Vector2 Position() {
			return transform.position;
		}

		public virtual float Scale() {
			return _animator.transform.localScale.x;
		}

		public virtual GameObject Avatar() {
			return gameObject;
		}

		public virtual Animator Animator() {
			return _animator;
		}

		public virtual Rigidbody2D Body() {
			return _rigidBody2D;
		}

		public bool IsNear(AvatarMain avatar, double range) {
			double distance = GameUtil.AvatarDistance(avatar.Area().Scale(), avatar.Position(), Position());
			//Debug.LogWarningFormat("IsNear {0}, {1}, {2}", avatar.name, range, distance);
			return distance <= range;
		}

		public void PopupText(string value, SkillDamageType type) {
			GameObject popupTextObj = (GameObject) Resources.Load("Asgla/Prefabs/PopupText", typeof(GameObject));

			Vector3 vector3 = new Vector3
				{x = transform.position.x, y = transform.position.y + 2f, z = transform.position.z};

			PopupText popupText = Instantiate(popupTextObj, vector3, Quaternion.identity, transform)
				.GetComponent<PopupText>();

			popupText.Setup(value, type, _popTextSortingOrder);

			_popTextSortingOrder++;
		}

		public virtual void Unselect() {
			UnitFrameSmall unit = Utility().SmallUnitFrameOne; //Main.Singleton.Game.UnitFrameTarget;

			unit.gameObject.SetActive(false);
		}

		public AvatarUtility Utility() {
			return _utility;
		}

	}
}