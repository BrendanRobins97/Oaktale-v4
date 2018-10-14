using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Defaulter
{
	public static class DefaulterMenuItems
	{
		private const string TOP = "Tools/Defaulter/";

		[MenuItem("Window/Defaulter", false, 200)]
		[MenuItem(TOP + "Open Defaulter Window", false, 200)]
		public static void OpenDefaulterWindow()
		{
			DefaulterWindow.Open();
		}

		[MenuItem(TOP + "Website", false, 2501)]
		internal static void OpenWebsiteLink()
		{
			OpenLink("http://defaulter.tonycoculuzzi.com");
		}

		[MenuItem(TOP + "Forum Thread", false, 2502)]
		internal static void OpenForumLink()
		{
			OpenLink("https://forum.unity3d.com/threads/defaulter-set-default-import-settings-easily.444852/");
		}

		[MenuItem(TOP + "Contact", false, 2503)]
		internal static void OpenEmailLink()
		{
			OpenLink("mailto:tonycoculuzzi+defaulter@gmail.com");
		}

		private static void OpenLink(string url)
		{
			Application.OpenURL(url);
		}
	}
}