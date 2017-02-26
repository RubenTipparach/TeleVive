
//======================== VRBasics_Lever ===================================
//
// draws a gizmo that indicates the position and length of a lever prefab
// levers provide a solid mount for the child hinge to rotate around a single axis
//
//=========================== by Zac Zidik ====================================

using UnityEditor;
using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class VRBasics_Lever : MonoBehaviour {

	public GameObject hinge;
	//the radius of the arc
	public float length = 1.0f;

	public void DrawGizmo(){
		//color of gizmo
		Handles.color = Color.magenta;
		//get the position of the end of the lever according to how long it is
		Vector3 endOfLever = transform.position + (hinge.transform.up.normalized * length);
		//draw a line from the hinge joint to the end of the lever
		Handles.DrawLine (transform.position, endOfLever);
		//draw a solid dot at the hinge end of lever
		Handles.DrawSolidDisc (transform.position, transform.right, 0.01f);
		//draw an solid dot at the end of the lever
		Handles.DrawSolidDisc (endOfLever, hinge.transform.right, 0.01f);
	}
}

//custom editor for VRBasics_Lever class
//automatic handling of multi-object handling, undo and prefab overrides
[CustomEditor(typeof(VRBasics_Lever))]
[CanEditMultipleObjects]
public class VRBasics_LeverEditor : Editor {

	SerializedProperty hingeProp;
	SerializedProperty lengthProp;

	void OnEnable(){
		//set up serialized properties
		hingeProp = serializedObject.FindProperty ("hinge"); 
		lengthProp = serializedObject.FindProperty ("length");
	}

	public override void OnInspectorGUI ()
	{
		//always update serialized properties at start of OnInspectorGUI
		serializedObject.Update ();

		//display serialized properties in inspector
		EditorGUILayout.PropertyField (hingeProp, new GUIContent ("Hinge Object"));
		EditorGUILayout.PropertyField (lengthProp, new GUIContent ("Length"));

		//always apply serialized properties at end of OnInspectorGUI
		serializedObject.ApplyModifiedProperties ();
	}

	void OnSceneGUI() {
		//reference to the class of object used to display gizmo
		VRBasics_Lever lever = (VRBasics_Lever) target;

		VRBasics_Hinge hinge = lever.hinge.GetComponent<VRBasics_Hinge> ();

		//DRAW LEVER
		lever.DrawGizmo();

		//DRAW HINGE
		hinge.DrawGizmo();
	}
}



