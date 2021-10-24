using Asgla.Data.Avatar;
using Asgla.Data.Entity;
using Asgla.Data.Skill;
using Asgla.Map;
using UnityEngine;

namespace Asgla.Avatar {
	public interface IAvatar {

		void Stats(AvatarStats stats);

		void State(AvatarState state);

		void Area(MapArea area);

		void Move(Vector2 vector);

		void Position(Vector2 vector2);

		void Scale(float scale);

		void OnDeath();

		void Attack(AvatarMain avatar, int damage);

		void Damaged(int damage, SkillDamageType type);

		void PlayAnimation(bool play);

		void Flip(bool flip);

		//void Target(AvatarMain target);

		//void TargetReset();

		//void TargetAdd(AvatarMain target);

		//void TargetRemove();

		int Id();

		int DatabaseId();

		string Name();

		int Level();

		AvatarStats Stats();

		AvatarState State();

		EntityType Type();

		MapArea Area();

		Vector2 Position();

		float Scale();

		GameObject Avatar();

		Animator Animator();

		Rigidbody2D Body();

		//AvatarMain Target();

		//HashSet<AvatarMain> Targets();

		bool IsNear(AvatarMain avatar, double range);

	}
}