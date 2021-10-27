using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Bars/Bullet Bar")]
	[RequireComponent(typeof(RectTransform))]
	public class UIBulletBar : UIBehaviour, IUIProgressBar {

		public enum BarType {

			Horizontal,
			Vertical,
			Radial

		}

		/// <summary>
		///     Gets or sets value indicating whether the fill should be inverted.
		/// </summary>
		public bool invertFill {
			get => m_InvertFill;
			set{
				m_InvertFill = value;
				UpdateFill();
			}
		}

		/// <summary>
		///     Gets the rect transform.
		/// </summary>
		public RectTransform rectTransform => transform as RectTransform;

		protected override void Start() {
			base.Start();

			if (m_BulletSprite == null || m_BulletSpriteActive == null) {
				Debug.LogWarning("The Bullet Bar script on game object " + gameObject.name +
				                 " requires that both bullet sprites are assigned to work.");
				enabled = false;
				return;
			}

			// Check if the bullets are constructed
			if (m_BulletsContainer == null)
				ConstructBullets();
		}

#if UNITY_EDITOR
		protected override void OnValidate() {
			base.OnValidate();

			// Update the bar fill
			UpdateFill();
		}
#endif

		/// <summary>
		///     Gets or sets the fill amount (0 to 1).
		/// </summary>
		public float fillAmount {
			get => m_FillAmount;
			set{
				m_FillAmount = Mathf.Clamp01(value);
				UpdateFill();
			}
		}

		/// <summary>
		///     Updates the fill.
		/// </summary>
		public void UpdateFill() {
			if (!isActiveAndEnabled || m_FillBullets == null || m_FillBullets.Count == 0)
				return;

			GameObject[] list = m_FillBullets.ToArray();

			if (m_InvertFill)
				Array.Reverse(list);

			int index = 0;
			foreach (GameObject go in list) {
				float currentPct = index / (float) m_BulletCount;

				Image img = go.GetComponent<Image>();
				if (img != null)
					img.enabled = m_FillAmount > 0f && currentPct <= m_FillAmount;

				index++;
			}
		}

		/// <summary>
		///     Constructs the bullet game objects and components.
		/// </summary>
		public void ConstructBullets() {
			if (m_BulletSprite == null || m_BulletSpriteActive == null || !isActiveAndEnabled)
				return;

			// Destroy the old bullets
			DestroyBullets();

			// Create the container
			m_BulletsContainer = new GameObject("Bullets", typeof(RectTransform));
			m_BulletsContainer.transform.SetParent(transform);
			m_BulletsContainer.layer = gameObject.layer;

			RectTransform crt = m_BulletsContainer.transform as RectTransform;
			crt.localScale = new Vector3(1f, 1f, 1f);
			crt.sizeDelta = rectTransform.sizeDelta;
			crt.localPosition = Vector3.zero;
			crt.anchoredPosition = Vector2.zero;

			// Create new bullets
			for (int i = 0; i < m_BulletCount; i++) {
				float pct = i / (float) m_BulletCount;

				// Create the background
				GameObject obj = new GameObject("Bullet " + i, typeof(RectTransform));
				obj.transform.SetParent(m_BulletsContainer.transform);
				obj.layer = gameObject.layer;

				RectTransform rt = obj.transform as RectTransform;
				rt.localScale = new Vector3(1f, 1f, 1f);
				rt.localPosition = Vector3.zero;

				Image img = obj.AddComponent<Image>();
				img.sprite = m_BulletSprite;
				img.color = m_BulletSpriteColor;

				if (m_FixedSize)
					rt.sizeDelta = m_BulletSize;
				else
					img.SetNativeSize();

				// Position the bullet
				if (m_BarType == BarType.Radial) {
					float ang = m_AngleMin + pct * (m_AngleMax - m_AngleMin);

					Vector2 pos;
					pos.x = 0f + m_Distance * Mathf.Sin(ang * Mathf.Deg2Rad);
					pos.y = 0f + m_Distance * Mathf.Cos(ang * Mathf.Deg2Rad);

					rt.anchoredPosition = pos;
					rt.Rotate(new Vector3(0f, 0f, (m_SpriteRotation + ang) * -1f));
				} else if (m_BarType == BarType.Horizontal) {
					rt.pivot = new Vector2(0.5f, 0.5f);
					rt.anchorMin = new Vector2(1f, 0.5f);
					rt.anchorMax = new Vector2(1f, 0.5f);

					float occupiedSpace = rt.sizeDelta.x * m_BulletCount;
					float freeSpace = rectTransform.rect.width - occupiedSpace;
					float spacing = freeSpace / (m_BulletCount - 1);

					float offsetX = rt.sizeDelta.x * i + spacing * i;

					Vector2 pos;
					pos.x = (offsetX + rt.sizeDelta.x / 2f) * -1f;
					pos.y = 0f;

					rt.anchoredPosition = pos;
					rt.Rotate(new Vector3(0f, 0f, m_SpriteRotation));
				} else if (m_BarType == BarType.Vertical) {
					rt.pivot = new Vector2(0.5f, 0.5f);
					rt.anchorMin = new Vector2(0.5f, 1f);
					rt.anchorMax = new Vector2(0.5f, 1f);

					float occupiedSpace = rt.sizeDelta.y * m_BulletCount;
					float freeSpace = rectTransform.rect.height - occupiedSpace;
					float spacing = freeSpace / (m_BulletCount - 1);

					float offsetY = rt.sizeDelta.y * i + spacing * i;

					Vector2 pos;
					pos.x = 0f;
					pos.y = (offsetY + rt.sizeDelta.y / 2f) * -1f;

					rt.anchoredPosition = pos;
					rt.Rotate(new Vector3(0f, 0f, m_SpriteRotation));
				}

				// Create the fill
				GameObject objFill = new GameObject("Fill", typeof(RectTransform));
				objFill.transform.SetParent(obj.transform);
				objFill.layer = gameObject.layer;

				RectTransform rtFill = objFill.transform as RectTransform;
				rtFill.localScale = new Vector3(1f, 1f, 1f);
				rtFill.localPosition = Vector3.zero;
				rtFill.anchoredPosition = m_ActivePosition;
				rtFill.rotation = rt.rotation;

				Image imgFill = objFill.AddComponent<Image>();
				imgFill.sprite = m_BulletSpriteActive;
				imgFill.color = m_BulletSpriteActiveColor;

				if (m_FixedSize)
					rtFill.sizeDelta = m_BulletSize;
				else
					imgFill.SetNativeSize();

				// Add the fill bullet to the list
				m_FillBullets.Add(objFill);
			}

			// Update
			UpdateFill();
		}

		protected void DestroyBullets() {
			// Clear the list
			m_FillBullets.Clear();

			GameObject go = m_BulletsContainer;

			// Destroy bullets
			if (Application.isEditor) {
#if UNITY_EDITOR
				EditorApplication.delayCall += () => { DestroyImmediate(go); };
#endif
			} else {
				Destroy(go);
			}

			// Null the variable
			m_BulletsContainer = null;
		}

#pragma warning disable 0649
		[SerializeField] private BarType m_BarType = BarType.Horizontal;

		[SerializeField] private bool m_FixedSize;
		[SerializeField] private Vector2 m_BulletSize = Vector2.zero;

		[SerializeField] private Sprite m_BulletSprite;
		[SerializeField] private Color m_BulletSpriteColor = Color.white;
		[SerializeField] private Sprite m_BulletSpriteActive;
		[SerializeField] private Color m_BulletSpriteActiveColor = Color.white;

		[SerializeField] private float m_SpriteRotation;
		[SerializeField] private Vector2 m_ActivePosition = Vector2.zero;

		[SerializeField] [Range(0f, 360f)] private float m_AngleMin;
		[SerializeField] [Range(0f, 360f)] private float m_AngleMax = 360f;
		[SerializeField] private int m_BulletCount = 10;
		[SerializeField] private float m_Distance = 100f;

		[SerializeField] [Range(0f, 1f)] private float m_FillAmount = 1f;
		[SerializeField] private bool m_InvertFill = true;

		[SerializeField] [HideInInspector] private GameObject m_BulletsContainer;
		[SerializeField] [HideInInspector] private List<GameObject> m_FillBullets;
#pragma warning restore 0649

	}
}