/*using UnityEditor;
using System;

public enum PlayModeState
{
	Stopped,
	Playing,
	Paused
}

[InitializeOnLoad]
public class EditorPlayMode
{
	private static PlayModeState _oldState = GetPlayModeState;
	
	static EditorPlayMode ()
	{
		EditorApplication.playmodeStateChanged = OnUnityPlayModeChanged;

	}
	
	public static event Action<PlayModeState, PlayModeState> PlayModeChanged;
	
	public static void Play ()
	{
		EditorApplication.isPlaying = true;
	}
	
	public static void Pause ()
	{
		EditorApplication.isPaused = true;
	}
	
	public static void Stop ()
	{
		EditorApplication.isPlaying = false;
	}
	
	private static void OnPlayModeChanged (PlayModeState oldState, PlayModeState newState)
	{
		if (PlayModeChanged != null)
			PlayModeChanged (oldState, newState);
	}
	
	private static void OnUnityPlayModeChanged ()
	{
		var newState = GetPlayModeState;
		// Fire PlayModeChanged event.
		OnPlayModeChanged (_oldState, newState);
		// Set current state.
		_oldState = newState;
	}

	public static PlayModeState GetPlayModeState {
		get {
			if (EditorApplication.isPlaying) {
				if (EditorApplication.isPaused)
					return PlayModeState.Paused;
				else
					return PlayModeState.Playing;
			} else
				return PlayModeState.Stopped;
 		}
	}
}*/
