using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Demo_FixScrollRects : MonoBehaviour {

	protected void OnEnable() {
		SceneManager.sceneLoaded += OnLoaded;
	}

	protected void OnDisable() {
		SceneManager.sceneLoaded -= OnLoaded;
	}

	private void OnLoaded(Scene scene, LoadSceneMode mode) {
		ScrollRect[] rects = FindObjectsOfType<ScrollRect>();

		foreach (ScrollRect rect in rects)
			LayoutRebuilder.MarkLayoutForRebuild(rect.viewport);
	}

}