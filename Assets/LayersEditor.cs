using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


public class LayersEditor : ScriptableWizard
{
	public Renderer renderer;
	public Renderer[] rendererList;
	public SortingLayer sortingLayer;
	public int sortingOrder = 2;

	[MenuItem("GameObject/Set Renderer SortingLayer")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard<LayersEditor>("Create Light", "Update");
	}


	void OnWizardUpdate()
	{
		if (renderer == null && (rendererList == null || rendererList.Length == 0))
		{
			helpString = "";
			errorString = "Select gameObject with renderer on it \n or drag couple of these objects in to the rendererList";
			isValid = false;
		}
		else
		{
			if (renderer != null)
				helpString = "Current sortingLayerName = " + renderer.sortingLayerID + "\n" +
								"sortingOrder =" + renderer.sortingOrder;
			errorString = "";
			isValid = true;
		}
	}

	void OnWizardCreate()
	{
		if (renderer != null)
			updateRenderer(renderer, sortingOrder, sortingLayer.id);
		if (rendererList != null && rendererList.Length > 0)
			foreach (Renderer r in rendererList)
				updateRenderer(r, sortingOrder, sortingLayer.id);
	}

	void updateRenderer(Renderer renderer, int sortingOrder, int layerId)
	{
		renderer.sortingOrder = sortingOrder;
		renderer.gameObject.layer = layerId;
		EditorUtility.SetDirty(renderer);
	}
}

[Serializable]
public class SortingLayer
{
	public int id;
}

[CustomPropertyDrawer(typeof(SortingLayer))]
public class SortingLayerIdPropDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		float offset = 4.0f;
		position.x += offset;
		position.width -= offset * 2;

		int origId = property.FindPropertyRelative("id").intValue;
		string origString = LayerMask.LayerToName(origId);
		int selectionId = 0;
		List<String> layerNames = new List<string>();
		for (int i = 0; i < 32; i++)
		{
			string layerName = LayerMask.LayerToName(i);
			if (layerName != string.Empty)
			{
				layerNames.Add(layerName);
				if (layerName.Equals(origString))
					selectionId = layerNames.Count - 1;
			}
		}
		Rect labelRect = position;
		labelRect.width = EditorGUIUtility.labelWidth;
		position.x += EditorGUIUtility.labelWidth;
		position.width -= EditorGUIUtility.labelWidth;

		EditorGUI.indentLevel = 0;

		EditorGUI.LabelField(labelRect, label);
		int newId = EditorGUI.Popup(position, selectionId, layerNames.ToArray());
		if (newId != selectionId)
		{
			int newLayerId = LayerMask.NameToLayer(layerNames[newId]);
			property.FindPropertyRelative("id").intValue = newLayerId;
		}
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight;
	}
}