#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Defaulter
{
	#region EditorGUIPlus
	public static class EditorGUIPlus
	{
		public static void Solid(Rect rect, Color color)
		{
			Color oldColor = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(rect, EditorTextures.White);
			GUI.color = oldColor;
		}

		public static void BorderlessBox(Rect rect)
		{
			GUI.DrawTexture(rect, EditorTextures.Background); 
		}

		public static void BorderlessShadowBox(Rect rect, float power = 0.02f)
		{
			Color oldColor = GUI.color;
			Color newColor = oldColor;
			newColor.a = power;
			GUI.color = newColor;
			GUI.DrawTexture(rect, EditorTextures.Black);
			GUI.color = oldColor;
		}

		public static void Box(Rect rect)
		{
			BorderlessBox(rect);
			LineBox(rect, EditorColors.SHADOW);
		}

		public static void InsetBox(Rect rect)
		{
			BorderlessShadowBox(rect);
			InsetLineBox(rect);
		}

		public static void OutsetBox(Rect rect)
		{
			BorderlessBox(rect);
			LineBox(rect, EditorColors.SHADOW);
			LineBox(InsetRect(rect, 1), EditorColors.HIGHLIGHT);
		}

		public static void Checkerboard(Rect rect) 
		{
			Texture2D image = EditorTextures.Checkerboard;
			GUI.DrawTextureWithTexCoords(rect, image, new Rect(0, 0, (float)(rect.width / image.width), (float)(rect.height / image.height)));
		}

		public static void Line(Vector2 a, Vector2 b, Color c)
		{
			Color oldColor = Handles.color;
			Handles.color = c;
			Handles.DrawLine(a, b);
			Handles.color = oldColor;
		}

		[Obsolete("Use BetterEditorGuiLayout.Line instead", true)]
		public static void LayoutLine(Color c) { LayoutLine(c, 5); }

		[Obsolete("Use BetterEditorGuiLayout.Line instead", true)]
		public static void LayoutLine(Color c, int padding)
		{
			EditorGUILayoutPlus.Line(c, padding);
		}

		public static void LinePath(Vector2[] path, Color color)
		{
			for (int i = 0; i < path.Length; i++)
			{
				if (i + 1 < path.Length)
				{
					Line(path[i], path[i + 1], color);
				}
			}
		}

		public static void LineBox(Rect rect, Color color)
		{
			Color oldColor = Handles.color;
			Handles.color = color;
			Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.xMax, rect.y));
			Handles.DrawLine(new Vector2(rect.xMax, rect.y), new Vector2(rect.xMax, rect.yMax - 1));
			Handles.DrawLine(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMin, rect.yMax));
			Handles.DrawLine(new Vector2(rect.x, rect.yMax), new Vector2(rect.x, rect.y - 1));
			Handles.color = oldColor;
		}

		public static void InsetLineBox(Rect rect)
		{
			LineBox(rect, EditorColors.HIGHLIGHT);
			LineBox(InsetRect(rect, 1), EditorColors.SHADOW);
		}

		public static void OutsetLineBox(Rect rect)
		{
			LineBox(rect, EditorColors.SHADOW);
			LineBox(InsetRect(rect, 1), EditorColors.HIGHLIGHT);
		}

		public static void DrawSprite(Rect rect, Sprite sprite)
		{
			Texture t = sprite.texture;
			Rect tr = sprite.textureRect;
			Rect r = new Rect(tr.x / t.width, tr.y / t.height, tr.width / t.width, tr.height / t.height);

			GUI.DrawTextureWithTexCoords(new Rect(rect.x, rect.y, tr.width, tr.height), t, r);
		}

		#region GUI
		public static string OutlinedTextField(Rect r, string s)
		{
			GUIStyle style = new GUIStyle(GUI.skin.textField);
			style.alignment = TextAnchor.MiddleLeft;
			style.padding = new RectOffset(5, 5, 0, 0);
			style.fontSize = (int)r.height - 9;

			s = EditorGUI.TextField(r, s, style);

			Rect outlineRect = r;
			outlineRect.xMin++;
			outlineRect.yMin++;
			EditorGUIPlus.LineBox(outlineRect, EditorColors.SHADOW);
			EditorGUIPlus.LineBox(OutsetRect(outlineRect, 1), EditorColors.HIGHLIGHT);

			return s;
		}

		public static string OutlinedTextArea(Rect r, string s)
		{
			GUIStyle style = new GUIStyle(GUI.skin.textArea);
			style.alignment = TextAnchor.UpperLeft;
			style.padding = new RectOffset(5, 5, 5, 5);

			s = EditorGUI.TextArea(r, s, style);

			Rect outlineRect = r;
			outlineRect.xMin++;
			outlineRect.yMin++;
			EditorGUIPlus.LineBox(outlineRect, EditorColors.SHADOW);
			EditorGUIPlus.LineBox(OutsetRect(outlineRect, 1), EditorColors.HIGHLIGHT);

			return s;
		}

		#region Custom Float Field
		private static Type FLOAT_FIELD_RECYCLED_TEXT_EDITOR_TYPE = Assembly.GetAssembly(typeof(EditorGUI)).GetType("UnityEditor.EditorGUI+RecycledTextEditor");
		private static Type[] FLOAT_FIELD_ARGUMENT_TYPES = new Type[] { FLOAT_FIELD_RECYCLED_TEXT_EDITOR_TYPE, typeof(Rect), typeof(Rect), typeof(int), typeof(float), typeof(string), typeof(GUIStyle), typeof(bool) };
		private static MethodInfo FLOAT_FIELD_METHOD = typeof(EditorGUI).GetMethod("DoFloatField", BindingFlags.NonPublic | BindingFlags.Static, null, FLOAT_FIELD_ARGUMENT_TYPES, null);
		private static FieldInfo FLOAT_FIELD_FIELD_INFO = typeof(EditorGUI).GetField("s_RecycledEditor", BindingFlags.NonPublic | BindingFlags.Static);

		public static float FloatField(Rect position, Rect dragHotZone, float value)
		{
			int controlID = GUIUtility.GetControlID("EditorTextField".GetHashCode(), FocusType.Keyboard, position);
			object recycledEditor = FLOAT_FIELD_FIELD_INFO.GetValue(null);
			object[] parameters = new object[] { recycledEditor, position, dragHotZone, controlID, value, "g7", EditorStyles.numberField, true };
			return (float)FLOAT_FIELD_METHOD.Invoke(null, parameters);
		}
		#endregion
		#endregion

		#region Rect
		public static Rect OutsetRect(Rect rect, int outset)
		{
			Rect output = rect;
			output.xMin -= outset;
			output.xMax += outset;
			output.yMin -= outset;
			output.yMax += outset;
			return output;
		}

		public static Rect InsetRect(Rect rect, int inset)
		{
			return OutsetRect(rect, -inset);
		}
		#endregion
	}
	#endregion

	#region EditorGUILayoutPlus
	public static class EditorGUILayoutPlus
	{
		public static string OutlinedTextField(string text, int fontSize)
		{
			int height = fontSize + 9;
			Rect r;

			Color c = GUI.color;
			GUI.color = new Color(1, 1, 1, 0);
			GUILayout.Box("", GUILayout.Height(height), GUILayout.ExpandWidth(true));
			GUI.color = c;

			r = GUILayoutUtility.GetLastRect();
			r.xMax -= 2;

			return EditorGUIPlus.OutlinedTextField(r, text);
		}

		public static string OutlinedTextField(string text, string label)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(label);
			text = OutlinedTextField(text, 12);
			GUILayout.EndHorizontal();
			return text;
		}

		public static string OutlinedTextArea(string text, int height)
		{
			Rect r;
			bool expandHeight = height < 0;
			Color c = GUI.color;
			GUI.color = new Color(1, 1, 1, 0);
			GUILayout.Box("", EditorStyles.textArea, GUILayout.Height(height), GUILayout.ExpandHeight(expandHeight), GUILayout.ExpandWidth(true));
			GUI.color = c;

			r = GUILayoutUtility.GetLastRect();
			r.xMax -= 2;

			return EditorGUIPlus.OutlinedTextArea(r, text);
		}

		public static void HorizontalLine() { HorizontalLine(EditorColors.HIGHLIGHT); }
		public static void HorizontalLine(Color color)
		{
			Rect r;

			Color c = GUI.color;
			GUI.color = new Color(1, 1, 1, 0);
			GUILayout.Box("", GUILayout.Height(3), GUILayout.ExpandWidth(true));
			GUI.color = c;

			r = GUILayoutUtility.GetLastRect();

			EditorGUIPlus.Line(new Vector2(r.x, r.center.y), new Vector2(r.xMax, r.center.y), color);
		}

		public static void LineBreak(int space)
		{
			Rect r;

			Color c = GUI.color;
			GUI.color = new Color(1, 1, 1, 0);
			GUILayout.Box("", GUILayout.Height(space), GUILayout.ExpandWidth(true));
			GUI.color = c;

			r = GUILayoutUtility.GetLastRect();

			EditorGUIPlus.Line(new Vector2(r.x, r.center.y), new Vector2(r.xMax, r.center.y), EditorColors.HIGHLIGHT);
			EditorGUIPlus.Line(new Vector2(r.x, r.center.y + 1), new Vector2(r.xMax, r.center.y + 1), EditorColors.SHADOW);
		}

		public static void Line(Color c) { Line(c, 5); }
		public static void Line(Color c, int padding)
		{
			GUILayout.Space(1);
			GUILayout.BeginHorizontal();
			GUILayout.Space(0);
			GUILayout.FlexibleSpace();
			Rect r = GUILayoutUtility.GetLastRect();
			GUILayout.EndHorizontal();
			EditorGUIPlus.Line(new Vector2(r.xMin + padding, r.y), new Vector2(r.xMax - padding, r.y), c);
		}

		public static void GroupLabel(string label)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(5);
			GUI.color = GUI.contentColor * new Color(1, 1, 1, 0.5f);
			GUILayout.Label(label, EditorStyles.miniLabel);
			GUI.color = GUI.contentColor;
			GUILayout.Space(-5);
			GUILayout.EndVertical();
		}
	}

	#endregion

	#region EditorColors
	public static class EditorColors
	{
		public static readonly Color DARK_SELECTION = new Color(0.2431372549019608f, 0.3725490196078431f, 0.5882352941176471f, 1);
		public static readonly Color LIGHT_SELECTION = new Color(0.243f, 0.490f, 0.905f, 1);
		public static Color SELECTION
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return DARK_SELECTION;
				}
				else
				{
					return LIGHT_SELECTION;
				}
			}
		}

		public static readonly Color DARK_BG = new Color(0.21960f, 0.21960f, 0.21960f, 1);
		public static readonly Color LIGHT_BG = new Color(0.76078f, 0.76078f, 0.76078f, 1);

		public static Color BG
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return DARK_BG;
				}
				else
				{
					return LIGHT_BG;
				}
			}
		}

		public static readonly Color DARK_HIGHLIGHT = new Color(0.29804f, 0.29804f, 0.29804f, 1);
		public static readonly Color LIGHT_HIGHLIGHT = new Color(0.88235f, 0.88235f, 0.88235f, 1);

		public static Color HIGHLIGHT
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return DARK_HIGHLIGHT;
				}
				else
				{
					return LIGHT_HIGHLIGHT;
				}
			}
		}

		public static readonly Color DARK_SHADOW = new Color(0.10980f, 0.10980f, 0.10980f, 1);
		public static readonly Color LIGHT_SHADOW = new Color(0.52941f, 0.52941f, 0.52941f, 1);

		public static Color SHADOW
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return DARK_SHADOW;
				}
				else
				{
					return LIGHT_SHADOW;
				}
			}
		}

		public static readonly Color DARK_TOOLBAR = new Color(0.16f, 0.16f, 0.16f, 1);
		public static readonly Color LIGHT_TOOLBAR = new Color(0.63f, 0.63f, 0.63f, 1);

		public static Color TOOLBAR
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return DARK_TOOLBAR;
				}
				else
				{
					return LIGHT_TOOLBAR;
				}
			}
		}

		public static readonly Color DARK_CHECKER_DARK = new Color(0.14f, 0.14f, 0.14f, 1);
		public static readonly Color LIGHT_CHECKER_DARK = new Color(0.80392f, 0.80392f, 0.80392f, 1);

		public static Color CHECKER_DARK
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return DARK_CHECKER_DARK;
				}
				else
				{
					return LIGHT_CHECKER_DARK;
				}
			}
		}

		public static readonly Color DARK_CHECKER_LIGHT = new Color(0.18f, 0.18f, 0.18f, 1);
		public static readonly Color LIGHT_CHECKER_LIGHT = new Color(1, 1, 1, 1);

		public static Color CHECKER_LIGHT
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return DARK_CHECKER_LIGHT;
				}
				else
				{
					return LIGHT_CHECKER_LIGHT;
				}
			}
		}

		public static readonly Color SHADOW_BOX = new Color(0, 0, 0, 0.02f);
	}
	#endregion

	#region EditorTextures
	public static class EditorTextures
	{
		public static Texture2D White { get { return GetTexture(new Color32(255, 255, 255, 255)); } }
		public static Texture2D Black { get { return GetTexture(new Color32(0, 0, 0, 255)); } }

		public static Texture2D ShadowBox { get { return GetTexture(EditorColors.SHADOW_BOX); } }
		public static Texture2D Background { get { return GetTexture(EditorColors.BG); } }

		private static Texture2D _checkerboard;
		public static Texture2D Checkerboard
		{
			get
			{
				if (_checkerboard == null)
				{
					_checkerboard = new Texture2D(32, 32, TextureFormat.ARGB32, false, true);

					Rect r;
					Color[] pixels;

					r = new Rect(0, 0, 16, 16);
					pixels = _checkerboard.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
					for (int i = 0; i < pixels.Length; i++) { pixels[i] = EditorColors.CHECKER_DARK; }
					_checkerboard.SetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height, pixels);

					r = new Rect(16, 0, 16, 16);
					pixels = _checkerboard.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
					for (int i = 0; i < pixels.Length; i++) { pixels[i] = EditorColors.CHECKER_LIGHT; }
					_checkerboard.SetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height, pixels);

					r = new Rect(0, 16, 16, 16);
					pixels = _checkerboard.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
					for (int i = 0; i < pixels.Length; i++) { pixels[i] = EditorColors.CHECKER_LIGHT; }
					_checkerboard.SetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height, pixels);

					r = new Rect(16, 16, 16, 16);
					pixels = _checkerboard.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
					for (int i = 0; i < pixels.Length; i++) { pixels[i] = EditorColors.CHECKER_DARK; }
					_checkerboard.SetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height, pixels);

					_checkerboard.wrapMode = TextureWrapMode.Repeat;
					_checkerboard.filterMode = FilterMode.Point;
					_checkerboard.Apply();
				}
				return _checkerboard;
			}
		}

		private static Dictionary<Color32, Texture2D> _textures;
		public static Texture2D GetTexture(Color32 color)
		{
			if (_textures == null) { _textures = new Dictionary<Color32, Texture2D>(); }
			if (!_textures.ContainsKey(color) || _textures[color] == null)
			{
				_textures[color] = CreateTexture(color);
			}

			return _textures[color];
		}

		private static Texture2D CreateTexture(Color32 color)
		{
			Texture2D t = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
			t.SetPixel(1, 1, color);
			t.Apply();
			return t;
		}
	}
	#endregion
}
#endif