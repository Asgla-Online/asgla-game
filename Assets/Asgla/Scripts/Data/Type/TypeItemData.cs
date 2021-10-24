namespace Asgla.Data.Type {

	public class TypeItemData {

		public Category Category;
		public Equipment Equipment;

		public string Icon;

		public string Name;

		public Weapon Weapon;

	}

	public enum Category {

		Armor,
		Boot,
		Ear,
		Eyebrow,
		Eye,
		FacialHair,
		Glove,
		Hair,
		Helmet,
		Mouth,
		Nose,
		Pant,
		SkinDetail,
		Weapon,
		Cape,
		Skirt,
		BodySkin,
		Class

	}

	public enum Equipment {

		Armor,
		Boot,
		Ear,
		Eyebrow,
		Eye,
		FacialHair,
		Glove,
		Hair,
		Helmet,
		Mouth,
		Nose,
		Pant,
		SkinDetail,
		MainHand,
		OffHand,
		Cape,
		Skirt,
		BodySkin

	}

	public enum Weapon {

		OneHanded = 1,
		TwoHanded = 2,
		Bow = 3,
		Shield = 4,
		Gun = 5,
		Rifle = 6

	}

}