using System.Collections.Generic;
using UnityEngine;

namespace AsglaUI.UI {
	public class UISceneRegistry {

		private static UISceneRegistry m_Instance;

		private List<UIScene> m_Scenes;

		protected UISceneRegistry() {
			m_Scenes = new List<UIScene>();
		}

		public static UISceneRegistry instance {
			get{
				if (m_Instance == null)
					m_Instance = new UISceneRegistry();
				return m_Instance;
			}
		}

		/// <summary>
		///     Get all the registered scenes.
		/// </summary>
		public UIScene[] scenes => m_Scenes.ToArray();

		/// <summary>
		///     Get the last active scene before the current one.
		/// </summary>
		public UIScene lastScene { get; private set; }

		/// <summary>
		///     Register a scene.
		/// </summary>
		/// <param name="scene"></param>
		public void RegisterScene(UIScene scene) {
			// Make sure we have the list set
			if (m_Scenes == null)
				m_Scenes = new List<UIScene>();

			// Check if already registered
			if (m_Scenes.Contains(scene)) {
				Debug.LogWarning("Trying to register a UIScene multiple times.");
				return;
			}

			// Store in the list
			m_Scenes.Add(scene);
		}

		/// <summary>
		///     Unregister a scene.
		/// </summary>
		/// <param name="scene"></param>
		public void UnregisterScene(UIScene scene) {
			if (m_Scenes != null)
				m_Scenes.Remove(scene);
		}

		/// <summary>
		///     Get all the active scenes.
		/// </summary>
		/// <returns></returns>
		public UIScene[] GetActiveScenes() {
			List<UIScene> activeScenes = m_Scenes.FindAll(x => x.isActivated);

			return activeScenes.ToArray();
		}

		/// <summary>
		///     Get the scene with the specified id. Returns null if not found.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public UIScene GetScene(int id) {
			if (m_Scenes == null || m_Scenes.Count == 0)
				return null;

			return m_Scenes.Find(x => x.id == id);
		}

		/// <summary>
		///     Get the next available scene id;
		/// </summary>
		/// <returns></returns>
		public int GetAvailableSceneId() {
			if (m_Scenes.Count == 0)
				return 0;

			int id = 0;

			foreach (UIScene scene in m_Scenes)
				if (scene.id > id)
					id = scene.id;

			return id + 1;
		}

		/// <summary>
		///     Transitions out of the active scene and in to the new one.
		/// </summary>
		/// <param name="scene"></param>
		public void TransitionToScene(UIScene scene) {
			// Transition out of the current scenes
			UIScene[] activeScenes = GetActiveScenes();

			foreach (UIScene activeScene in activeScenes) {
				// Transition the scene out
				activeScene.TransitionOut();

				// Set as last scene
				lastScene = activeScene;
			}

			// Transition in the new scene
			scene.TransitionIn();
		}

	}
}