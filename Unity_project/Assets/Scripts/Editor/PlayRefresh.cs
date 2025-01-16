using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlayRefresh {
	[MenuItem ("Tools/Auto Refresh")]
	private static void AutoRefreshToggle () {
		var status = EditorPrefs.GetInt ("kAutoRefresh");

		EditorPrefs.SetInt ("kAutoRefresh", status == 1 ? 0 : 1);
	}

	[MenuItem ("Tools/Auto Refresh", true)]
	private static bool AutoRefreshToggleValidation () {
		var status = EditorPrefs.GetInt ("kAutoRefresh");

		Menu.SetChecked ("Tools/Auto Refresh", status == 1);

		return true;
	}

	[MenuItem ("Tools/Refresh %r")]
	private static void Refresh () {
		Debug.Log ("Request script reload.");

		EditorApplication.UnlockReloadAssemblies ();

		AssetDatabase.Refresh ();

		EditorUtility.RequestScriptReload ();
	}

	[InitializeOnLoadMethod]
	private static void Initialize () {
		Debug.Log ("Script reloaded!");

		AssetDatabase.SaveAssets ();

		EditorApplication.LockReloadAssemblies ();
	}
}
