using UnityEngine;

public class CameraResolutionManager : MonoBehaviour {

	//default is most common mobile device resolution
	[SerializeField] private int width = 720;
	[SerializeField] private int height = 1280;
	[SerializeField] private bool fullscreen = false;

	//Calls initializer function when attaching to GameObject
	void Start () {
		SetResolution();
	}
	
	//Function to initialize default width, height, and fullscreen values for screen
	private void SetResolution() {
		///Screen.SetResolution(int width, int height, bool fullscreen);
		Screen.SetResolution(width, height, fullscreen);
	}

	public void ChangeResolution(int newWidth, int newHeight, boolean isFullscreen)
	{
		width = newWidth;
		height = newHeight;
		fullscreen = isFullscreen;
		SetResolution();
	}
}

