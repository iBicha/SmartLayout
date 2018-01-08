using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System; 
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class SmartLayout : EditorWindow
{
	static Dictionary<Type,List<string>> Components = new Dictionary<Type,List<string>>();
	
	static List<string> EditLayouts = new List<string> ();
	static List<string> PlayLayouts = new List<string> ();
	static List<string> PauseLayouts = new List<string> ();
	
	static List<string> ActiveLayoutSelection = new List<string> ();
	
	static int TheIndex = 0;
	static SmartLayout window;
	
	static bool IsVisible {
		get {
			return isVisible;
		}
		set {
			
			window = (SmartLayout)EditorWindow.GetWindow (typeof(SmartLayout));
			window.titleContent.text = "Smart Layout";
			if (value)
				window.ShowUtility ();
			else
				window.Close ();
		}
	}
	
	[MenuItem("Window/Layouts/Smart Layout/Quick Switch %l")]
	static void QuickSwitch ()
	{
		TheIndex++;
		if (ActiveLayoutSelection.Count > 0) {
			LoadLayout (ActiveLayoutSelection [(TheIndex + ActiveLayoutSelection.Count) % ActiveLayoutSelection.Count]);
		}
	}
	
	[MenuItem ("Window/Layouts/Smart Layout/Quick Switch %l", true)]
	static bool ValidateQuickSwitch ()
	{
		return ActiveLayoutSelection.Count > 0;
	}
	
	[MenuItem("Window/Layouts/Smart Layout/Settings... %#&l")]
	static void SettingsWindow ()
	{
		IsVisible = !IsVisible;
	}
	
	void OnGUI ()
	{
		isEnabled = EditorGUILayout.BeginToggleGroup ("On Play Mode Changed", isEnabled);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("On Edit (" + EditLayouts.Count + ")", GUILayout.ExpandWidth (false), GUILayout.Width (80));
		EditorGUILayout.BeginVertical ();
		
		for (int i = 0; i<EditLayouts.Count; i++) {
			if (layouts.IndexOf (EditLayouts [i]) == -1) {
				EditLayouts.RemoveAt (i);
				i--;
				continue;
			}
			EditorGUILayout.BeginHorizontal ();
			EditLayouts [i] = layouts [EditorGUILayout.Popup (layouts.IndexOf (EditLayouts [i]), layoutsCopy, EditorStyles.popup)];
			if (GUILayout.Button ("-", GUILayout.ExpandWidth (false), GUILayout.Height (15))) {
				EditLayouts.RemoveAt (i);
			}
			EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("+", GUILayout.Width (30))) {
			EditLayouts.Add (layouts [0]);
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.Space ();
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("On Play (" + PlayLayouts.Count + ")", GUILayout.ExpandWidth (false), GUILayout.Width (80));
		EditorGUILayout.BeginVertical ();
		
		for (int i = 0; i<PlayLayouts.Count; i++) {
			if (layouts.IndexOf (PlayLayouts [i]) == -1) {
				PlayLayouts.RemoveAt (i);
				i--;
				continue;
			}
			EditorGUILayout.BeginHorizontal ();
			PlayLayouts [i] = layouts [EditorGUILayout.Popup (layouts.IndexOf (PlayLayouts [i]), layoutsCopy, EditorStyles.popup)];
			if (GUILayout.Button ("-", GUILayout.ExpandWidth (false), GUILayout.Height (15))) {
				PlayLayouts.RemoveAt (i);
			}
			EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("+", GUILayout.Width (30))) {
			PlayLayouts.Add (layouts [0]);
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.Space ();
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("On Pause (" + PauseLayouts.Count + ")", GUILayout.ExpandWidth (false), GUILayout.Width (80));
		EditorGUILayout.BeginVertical ();
		
		for (int i = 0; i<PauseLayouts.Count; i++) {
			if (layouts.IndexOf (PauseLayouts [i]) == -1) {
				PauseLayouts.RemoveAt (i);
				i--;
				continue;
			}
			EditorGUILayout.BeginHorizontal ();
			PauseLayouts [i] = layouts [EditorGUILayout.Popup (layouts.IndexOf (PauseLayouts [i]), layoutsCopy, EditorStyles.popup)];
			if (GUILayout.Button ("-", GUILayout.ExpandWidth (false), GUILayout.Height (15))) {
				PauseLayouts.RemoveAt (i);
			}
			EditorGUILayout.EndHorizontal ();
		}
		
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("+", GUILayout.Width (30))) {
			PauseLayouts.Add (layouts [0]);
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndToggleGroup (); 
		
		isEnabledSelection = EditorGUILayout.BeginToggleGroup ("On GameObject Selection", isEnabledSelection);
		
		Type MarkComponentForDeletion = null;
		foreach (Type compType in Components.Keys) {
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("If selected gameObject has : " + compType.Name +  " ("+ Components[compType].Count +")");
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Delete") && Components.ContainsKey(compType)) {
				MarkComponentForDeletion = compType;
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (" ", GUILayout.ExpandWidth (false), GUILayout.Width (80));
			EditorGUILayout.BeginVertical ();
			
			for (int i = 0; i<Components[compType].Count; i++) {
				if (layouts.IndexOf (Components[compType] [i]) == -1) {
					Components[compType].RemoveAt (i);
					i--;
					continue;
				}
				EditorGUILayout.BeginHorizontal ();
				Components[compType] [i] = layouts [EditorGUILayout.Popup (layouts.IndexOf (Components[compType] [i]), layoutsCopy, EditorStyles.popup)];
				if (GUILayout.Button ("-", GUILayout.ExpandWidth (false), GUILayout.Height (15))) {
					Components[compType].RemoveAt (i);
				}
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("+", GUILayout.Width (30))) {
				Components[compType].Add (layouts [0]);
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
		} 
		if(MarkComponentForDeletion!=null && Components.ContainsKey(MarkComponentForDeletion)){
			Components.Remove(MarkComponentForDeletion);
		}
		EditorGUILayout.Space ();
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("If selected gameObject has : ");
		Rect r = EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Select Components",GUILayout.ExpandWidth(true))) {
			ShowAddComponent(r);
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndToggleGroup (); 
		GUILayout.Label ("Currently used layouts arrangements :");
		GUILayout.Label ("[" + string.Join("][",ActiveLayoutSelection.ToArray()) + "]");
	}
	
	static GameObject dummyComponent;
	
	void ShowAddComponent (Rect rect)
	{
		Type tyAddComponentWindow = Type.GetType ("UnityEditor.AddComponentWindow,UnityEditor");
		if (tyAddComponentWindow != null) {
			if (dummyComponent == null) {
				dummyComponent = new GameObject ();
				dummyComponent.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | 
                    HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
			}
			
			MethodInfo showfunc = tyAddComponentWindow.GetMethod ("Show", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] {
				typeof(Rect),
				typeof(GameObject[])
			}, null);
			if (showfunc != null) {
				showfunc.Invoke (null, new System.Object[] {
					rect,
					new GameObject[]{dummyComponent}
				});

			}
		}
	}
	
	private static void OnComponentAdded (Type type)
	{
		if (type!=null && !Components.ContainsKey(type)) {
			Components.Add (type, new List<string> ());
			IsVisible = true;
			if(window) window.Focus();
		}
		
	}

    static string _layoutFolder;
    static string LayoutFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_layoutFolder))
            {
                _layoutFolder = InternalEditorUtility.unityPreferencesFolder + "/Layouts/";
            }
            return _layoutFolder;
        }
    }

    static List<string> layouts = new List<string> ();
	static string[] layoutsCopy;
	//static System.IO.FileSystemWatcher fsWatcher;
	
	/*static void SetupLayoutWatcher ()
	{
		fsWatcher = new System.IO.FileSystemWatcher (); 
		fsWatcher.Path = LayoutFolder;
		fsWatcher.Filter = "*.wlt";
		fsWatcher.NotifyFilter = System.IO.NotifyFilters.FileName;
		fsWatcher.Created += new System.IO.FileSystemEventHandler (OnLayoutsChanged);
		fsWatcher.Changed += new System.IO.FileSystemEventHandler (OnLayoutsChanged);
		fsWatcher.Deleted += new System.IO.FileSystemEventHandler (OnLayoutsChanged);
		fsWatcher.Renamed += new System.IO.RenamedEventHandler (OnLayoutsRenamed);
		fsWatcher.EnableRaisingEvents = true;
	}*/
	
	static void OnLayoutsChanged (object source, System.IO.FileSystemEventArgs e)
	{
		ReloadLayouts ();
	}
	
	static void OnLayoutsRenamed (object source, System.IO.RenamedEventArgs e)
	{
		ReloadLayouts ();
	}
	
	static void ReloadLayouts ()
	{
		layouts.Clear ();
		layouts.AddRange (System.IO.Directory.GetFiles (LayoutFolder, "*.wlt"));
		for (int i = 0; i<layouts.Count; i++) {
			layouts [i] = System.IO.Path.GetFileNameWithoutExtension ((string)layouts [i]);
		}
		layoutsCopy = new string[layouts.Count];
		layouts.CopyTo (layoutsCopy);
	}
	
	private static bool _isEnabled;
	private static bool _isEnabledSelection;
	static bool isEnabled {
		get {
			return _isEnabled;
		}
		set {
			_isEnabled = value;
		}
	} 
	static bool isEnabledSelection {
		get {
			return _isEnabledSelection;
		}
		set {
			_isEnabledSelection= value;
			if(value){
				oldSelection = Selection.activeGameObject;
			}
		}
	}

    [InitializeOnLoadMethod]
    static void InitSmartLayout ()
	{
		ReloadLayouts ();
		LoadPrefs ();
		EditorPlayMode.PlayModeChanged += OnPlayModeChanged;
		//SetupLayoutWatcher ();
		
		if (EditorPrefs.GetBool ("SmartLayoutFirstRun", true)) {
			EditorPrefs.SetBool ("SmartLayoutFirstRun", false);
			if (EditorUtility.DisplayDialog ("Smart Layout first time run",
			                                 "Would you like to configure Smart Layout?\n" +
			                                 "(You can configure it later using Window->Layouts->Smart Layout->Settings menu)", "Yes", "No")) {
				IsVisible = true; 
			} 
		}
		EditorApplication.update += new EditorApplication.CallbackFunction (UpdateLoop);
		AddLayoutsOfCurrentModeToActive ();
	}
	
	void OnDestroy ()
	{
		//SavePrefs(); 
	}
	
	static bool isVisible;
	
	void OnDisable ()
	{
		SavePrefs ();
		if (dummyComponent != null) {
			DestroyImmediate (dummyComponent);
			dummyComponent = null;
		}
		isVisible = false;
	}
	 
	void OnEnable ()
	{
		ReloadLayouts ();
		isVisible = true;
	} 
	
	void OnFocus ()
	{
		ReloadLayouts (); 
	}
	 
	static void LoadPrefs ()
	{
		_isEnabled = EditorPrefs.GetBool ("SmartLayoutEnabled", false);
		
		_isEnabledSelection = EditorPrefs.GetBool ("SmartLayoutEnabledSelection", false);
		
		EditLayouts.Clear ();
		string setting = EditorPrefs.GetString ("SmartLayoutsEdit", "");
		if (setting != "") {
			EditLayouts.AddRange (setting.Split (':'));
		}
		PlayLayouts.Clear ();
		setting = EditorPrefs.GetString ("SmartLayoutsPlay", "");
		if (setting != "") {
			PlayLayouts.AddRange (setting.Split (':'));
		}
		PauseLayouts.Clear ();
		setting = EditorPrefs.GetString ("SmartLayoutsPause", "");
		if (setting != "") {
			PauseLayouts.AddRange (setting.Split (':'));
		}
		
		Components.Clear ();
		setting = EditorPrefs.GetString ("SmartLayoutsComponents", "");
		if (setting != "") {
			string[] ComponentNames = setting.Split (':');
			foreach(string comp in ComponentNames){
				Type t = Type.GetType(comp);
				if(t!=null){
					Components.Add(t,new List<string>());
					setting = EditorPrefs.GetString ("SmartLayouts" + comp, "");
					if (setting != "") {
						Components[t].AddRange (setting.Split (':'));
					}
				}
				else
				{
					EditorPrefs.DeleteKey("SmartLayouts" + comp);
				}
			}
		}
	}
	
	static void SavePrefs ()
	{
		EditorPrefs.SetBool ("SmartLayoutEnabled", _isEnabled);
		
		EditorPrefs.SetBool ("SmartLayoutEnabledSelection", _isEnabledSelection);
		
		EditorPrefs.SetString ("SmartLayoutsEdit", string.Join (":", EditLayouts.ToArray ()));
		EditorPrefs.SetString ("SmartLayoutsPlay", string.Join (":", PlayLayouts.ToArray ()));
		EditorPrefs.SetString ("SmartLayoutsPause", string.Join (":", PauseLayouts.ToArray ()));
		
		string[] ComponentNames = new string[Components.Count];
		int i = 0;
		foreach (Type t in Components.Keys) {
			ComponentNames[i] = t.AssemblyQualifiedName;
			i++;
		}
		EditorPrefs.SetString ("SmartLayoutsComponents", string.Join (":", ComponentNames));
		foreach (Type t in Components.Keys) {
			EditorPrefs.SetString ("SmartLayouts" + t.AssemblyQualifiedName, string.Join (":", Components[t].ToArray ()));
		}
	} 
	
	private static void OnPlayModeChanged (PlayModeState oldState, PlayModeState newState)
	{
		if (newState == oldState || !isEnabled)
			return;
		ActiveLayoutSelection.Clear ();
		TheIndex = -1;
		AddLayoutsOfCurrentModeToActive ();
		EditorApplication.ExecuteMenuItem("Window/Layouts/Smart Layout/Quick Switch %l");
		
	}
	
	private static void LoadLayout (string layoutName)
	{ 
		if (System.IO.File.Exists (LayoutFolder + layoutName + ".wlt")) {
			EditorUtility.LoadWindowLayout (LayoutFolder + layoutName + ".wlt");
			
		} else {
			Debug.LogWarning ("Cannot load layout : \"" + layoutName + "\" does not exist. Smart Layouts probably needs configuration.\n" +
			                  "(You can configure it using Window->Layouts->Smart Layout->Settings menu)");
		}
	}
	
	
	// On Update
	
	static GameObject oldSelection = null;
	static int SkipFrames = 0;
	//Loop
	static void UpdateLoop ()
	{
		if (++SkipFrames % 20 != 0)
			return;
		if (dummyComponent != null) {
			Type t = GetComponentType ();
			if (t != null) {
				DestroyImmediate (dummyComponent);
				dummyComponent = null;
				OnComponentAdded (t); 
			}
		}
		
		if (isEnabledSelection && (oldSelection != Selection.activeGameObject)) {
			oldSelection = Selection.activeGameObject;
			SelectionChanged();
		}
	}
	
	static Type GetComponentType ()
	{
		Type ret = null;
		Component[] allcomps = dummyComponent.GetComponents (typeof(Component));
		foreach (Component comp in allcomps) {
			if (comp.GetType () != typeof(Transform)) {
				ret = comp.GetType ();
			}
		}
		return ret;
	}
	
	
	
	//On Selection Change
	static List<Type> DetectedTypes = new List<Type>();
	static bool HadZeroDetectedTypes = false;
	static bool WasNull = false;
	static void SelectionChanged(){
		if (oldSelection != null) {
			WasNull=false;
			DetectedTypes.Clear ();
			foreach (Type ty in Components.Keys) {
				if (oldSelection.GetComponent (ty) != null) {
					DetectedTypes.Add (ty);
				}
			}
			if (DetectedTypes.Count > 0) {
				HadZeroDetectedTypes = false;
				ActiveLayoutSelection.Clear ();
				AddLayoutsOfCurrentModeToActive ();
				foreach (Type t in DetectedTypes) {
					ActiveLayoutSelection.AddRange (Components [t]);
				}
				if(!isEnabled){
					TheIndex = -1;
					EditorApplication.ExecuteMenuItem("Window/Layouts/Smart Layout/Quick Switch %l");
				}
			} else if (!HadZeroDetectedTypes) {
				HadZeroDetectedTypes = true;
				ActiveLayoutSelection.Clear ();
				AddLayoutsOfCurrentModeToActive ();
				if(!isEnabled){
					TheIndex = -1;
					EditorApplication.ExecuteMenuItem("Window/Layouts/Smart Layout/Quick Switch %l");
				}
			}
		} else if (!WasNull) {
			WasNull = true;
			ActiveLayoutSelection.Clear ();
			AddLayoutsOfCurrentModeToActive ();
			if(!isEnabled){
				TheIndex = -1;
				EditorApplication.ExecuteMenuItem("Window/Layouts/Smart Layout/Quick Switch %l");
			}
		}
	}
	
	static void AddLayoutsOfCurrentModeToActive(){
		if (!isEnabled)
			return;
		switch (EditorPlayMode.GetPlayModeState) {
		case PlayModeState.Playing:
			ActiveLayoutSelection.AddRange(PlayLayouts);
			break;
		case PlayModeState.Paused:
			ActiveLayoutSelection.AddRange(PauseLayouts); 
			break;
		case PlayModeState.Stopped:
			ActiveLayoutSelection.AddRange(EditLayouts); 
			break;
		default:
			ActiveLayoutSelection.AddRange(PlayLayouts);
			break; 
		}
	}
	
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
			
			EditorApplication.playModeStateChanged += OnUnityPlayModeChanged;
			
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
		
		private static void OnUnityPlayModeChanged (PlayModeStateChange playModeStateChange)
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
	}
	
}