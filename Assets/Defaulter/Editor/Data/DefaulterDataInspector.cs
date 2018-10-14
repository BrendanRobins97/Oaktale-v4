using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

namespace Defaulter
{
	[CustomEditor(typeof(DefaulterData))]
	public class DefaulterDataInspector : Editor
	{
		protected override void OnHeaderGUI() { /* do nothing */ }

		public override void OnInspectorGUI()
		{
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Open Defaulter")) { DefaulterWindow.Open(); }
			GUILayout.FlexibleSpace();
		}

	}
}