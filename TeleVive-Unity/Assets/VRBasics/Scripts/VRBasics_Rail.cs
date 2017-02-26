
//========================== VRBasics_Rail ====================================
//
// draws a gizmo that indicates the position and length of a rail prefab
// rails provide a solid mount for the child slider to move back and forth along a single axis
//
//=========================== by Zac Zidik ====================================

using UnityEditor;
using UnityEngine;
using System.Collections;

//typically a kinematic rigidbody
//serves as a stable object for a slider object to move across
[RequireComponent (typeof(Rigidbody))]
public class VRBasics_Rail : MonoBehaviour {

	//the gameobject the moves along the rail
	public GameObject slider;
	//the length of the rail
	public float length = 1.0f;
	//the location of the anchor along the rail between 0.0 and 1.0
	public float anchor = 0.5f;
	//the amount the anchor has moved from the orginal position
	public float anchorMove;

	public void DrawGizmo(){

		//an empty game object used to aid in positioning
		GameObject dummyTrans = GetDummy();

		//reference to the configurable joint the gizmo displays
		ConfigurableJoint configJoint = slider.GetComponent<ConfigurableJoint> ();

		//color of gizmo
		Handles.color = Color.cyan;

		dummyTrans.transform.position = transform.position;
		dummyTrans.transform.eulerAngles = transform.eulerAngles;
		dummyTrans.transform.position += dummyTrans.transform.up * anchorMove;

		Vector3 minLimitPos = dummyTrans.transform.position + (transform.up.normalized * configJoint.linearLimit.limit);
		Vector3 maxLimitPos = dummyTrans.transform.position - (transform.up.normalized * configJoint.linearLimit.limit);

		//draw a empty dot at lower limit
		Handles.DrawWireDisc (minLimitPos, transform.right, 0.015f);
		//draw a empty dot at upper limit
		Handles.DrawWireDisc (maxLimitPos, transform.right, 0.015f);
		//draw a line
		Handles.DrawLine (minLimitPos, maxLimitPos);

		//remove the dummy
		DestroyImmediate (dummyTrans);
	}

	public GameObject GetDummy(){
		GameObject dummyTrans;
		//if one exist
		if (GameObject.Find ("Dummy")) {
			//get the Dummy transform object
			dummyTrans = GameObject.Find ("Dummy");
			//if one doesnt exist
		} else {
			//create one
			dummyTrans = new GameObject ();
			dummyTrans.name = "Dummy";
		}
		return dummyTrans;
	}

	public void SetAnchorMove(float m){
		//store the value of the distance the achor has moved
		anchorMove = m;
		//reposition the connected anchor on the slider joint
		slider.GetComponent<VRBasics_Slider> ().SetConnectedAnchorPos ();
	}
}


//custom editor for VRBasics_Rail class
//automatic handling of multi-object handling, undo and prefab overrides
[CustomEditor(typeof(VRBasics_Rail))]
[CanEditMultipleObjects]
public class VRBasics_RailEditor : Editor {

	SerializedProperty sliderProp;
	SerializedProperty lengthProp;
	SerializedProperty anchorProp;

	void OnEnable(){
		//set up serialized properties
		sliderProp = serializedObject.FindProperty ("slider"); 
		lengthProp = serializedObject.FindProperty ("length");
		anchorProp = serializedObject.FindProperty ("anchor");

		VRBasics_Rail rail = (VRBasics_Rail) target;
		//move anchor if not in correct place
		float move = (rail.anchor * rail.length) - (rail.length * 0.5f);
		if (rail.anchorMove != move) {
			rail.SetAnchorMove (move);
		}
	}

	public override void OnInspectorGUI ()
	{
		//always update serialized properties at start of OnInspectorGUI
		serializedObject.Update ();

		//start listening for changes in inspector values
		EditorGUI.BeginChangeCheck ();

		//display serialized properties in inspector
		EditorGUILayout.PropertyField (sliderProp, new GUIContent ("Slider Object"));
		EditorGUILayout.PropertyField (lengthProp, new GUIContent ("Length"));
		EditorGUILayout.Slider (anchorProp, 0.0f, 1.0f, new GUIContent ("Anchor"));

		//if there were any changes in inspector values
		if (EditorGUI.EndChangeCheck ()) {

			//apply changes to serialized properties
			serializedObject.ApplyModifiedProperties();

			VRBasics_Rail rail = (VRBasics_Rail) target;
			//move anchor if not in correct place
			float move = (rail.anchor * rail.length) - (rail.length * 0.5f);
			if (rail.anchorMove != move) {
				rail.SetAnchorMove (move);
			}

			//reference to the slider
			VRBasics_Slider slider = rail.slider.GetComponent<VRBasics_Slider> ();
			//adjust linear limit to match the length of the rail
			slider.SetLinearLimit ();
		}
	}

	void OnSceneGUI() {

		//reference to the class of object used to display gizmo
		VRBasics_Rail rail = (VRBasics_Rail) target;

		//reference to the slider
		VRBasics_Slider slider = rail.slider.GetComponent<VRBasics_Slider> ();

		//use the inspector to postion the slider along the rail
		slider.EditorSetPosition ();

		//calculate the percentage of the slider along the rail
		slider.CalcPercentage ();

		//DRAW RAIl
		rail.DrawGizmo();

		//DRAW SLIDER
		slider.DrawGizmo();
	}
}