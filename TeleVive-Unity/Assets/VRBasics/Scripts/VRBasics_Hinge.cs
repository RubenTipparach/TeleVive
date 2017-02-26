
//========================== VRBasics_Hinge ===================================
//
// draws a gizmo that indicates an arc of degrees of rotation for a a hinge joint
// this child of a lever can be used for any object that rotates around a single axis
// examples: doors, knobs, wheels, dials
// use the angle property to get the rotation of the hinge between -180 and 180
// use the percentage property to get the position of rotation of the hinge between 0.0 and 1.0
//
//=========================== by Zac Zidik ====================================

using UnityEditor;
using UnityEngine;
using System.Collections;


[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(HingeJoint))]
public class VRBasics_Hinge : MonoBehaviour {

	//options to where a spring can move to
	public enum SpringTo{noLimit,none,max,mid,min}

	//the true angle of the hinge between -180 and 180
	public float angle;
	//how much did the angle change from the last frame
	public float angleFrameChange;
	//use limts on joint
	public bool useLimits = true;
	//the lowest degree of rotation
	public float limitMin = 0.0f;
	//the highest degree of rotation
	public float limitMax = 90.0f;
	//the percentage of the total amount of movement (0.0 - 1.0)
	public float percentage;
	//used to check is the angle has change from frame to frame
	private float prevAngle;
	//where is the joint currently springing to
	private SpringTo springTo;
	//how many degrees has the angle rotated since start
	public float totalRotation = 0.0f;
	//how many revolutions has the hinge made since the start
	public float totalRevolutions = 0.0f;
	//spring a joint to the max limit
	public bool useSpringToMax = false;
	//spring amount to max
	public float springToMax;
	//damper amount to max
	public float damperToMax;
	//spring a joint to the middle of the total possible rotation
	public bool useSpringToMid = false;
	//spring amount to middle
	public float springToMid;
	//damper amount to middle
	public float damperToMid;
	//spring a joint to the min limit
	public bool useSpringToMin = false;
	//spring amount to min
	public float springToMin;
	//damper amount to min
	public float damperToMin;


	//the percentage of the total amount of movement (0.0 - 1.0)
	void CalcPercentage(){
		//how far is the hinge capable of swinging
		float degreesOfSwing = limitMax - limitMin;
		//percentage
		if (angle >= 0) {
			percentage = (((angle + Mathf.Abs(limitMin)) * 100) / degreesOfSwing) / 100;
		}else if(angle < 0){
			percentage = ((Mathf.Abs(limitMin - angle) * 100) / degreesOfSwing) / 100;
		}
	}

	//get the total amount of degrees of rotation from start angle
	//and the total amount of revolutions since start
	void CalcTotalRotAndRev(){
		//has the angle changed
		if (prevAngle != angle) {
			//calculate the amount of change since last frame
			angleFrameChange = Mathf.DeltaAngle (prevAngle, angle);
			//add in the amount of change to the total
			totalRotation += angleFrameChange;
			//divide the total to get the amount of revolutions
			totalRevolutions = totalRotation / 360;
			//set the past angle
			prevAngle = angle;
		} else {
			//there has been no change
			angleFrameChange = 0.0f;
		}
	}

	//since the hinge joint angle only returns a positive value between 0 and 180
	//and yet allows you to set the limits between -180 and 180
	//this returns the true angle between -180 and 180
	float CalcTrueAngle(){		

		//store a single euler angle
		float trueAngle = transform.localEulerAngles.x;
		//when this flips from 0 to 180
		if (transform.localEulerAngles.y >= 180.0f) {
			//adjust the euler angle to be between 0 and 360
			if (trueAngle < 90) {
				trueAngle = 180 - trueAngle;
			} else if (trueAngle > 270) {
				trueAngle = 270 - Mathf.Abs (270 - trueAngle);
			}
		}

		//convert a value between 0 and 360 to a value between -180 and 180
		if (trueAngle > 180) {
			trueAngle -= 360;  
		}

		return trueAngle;
	}

	public void DrawGizmo(){
		//reference to the parent class
		VRBasics_Lever lever = transform.parent.gameObject.GetComponent<VRBasics_Lever> ();

		//reference to the hinge joint the gizmo displays
		HingeJoint hingeJoint = GetComponent<HingeJoint> ();
		//reference to the limits of the hinge joint
		JointLimits limits = hingeJoint.limits;

		//color of gizmo
		Handles.color = Color.cyan;

		//an empty game object used to aid in positioning
		GameObject dummyTrans = GetDummy();

		//DRAW HINGE
		if (useLimits) {
			//draw an arc representing the movement of the lever
			Handles.DrawWireArc (lever.transform.position, lever.transform.right, lever.transform.up, limits.min, lever.length);
			Handles.DrawWireArc (lever.transform.position, lever.transform.right, lever.transform.up, limits.max, lever.length);

			//draw a empty dot at min end of the arc
			dummyTrans.transform.position = lever.transform.position;
			dummyTrans.transform.rotation = lever.transform.rotation;
			dummyTrans.transform.position += dummyTrans.transform.up * lever.length;
			dummyTrans.transform.RotateAround (lever.transform.position, lever.transform.right, limits.min);
			Handles.DrawWireDisc (dummyTrans.transform.position, lever.transform.right, 0.015f);

			//draw a empty dot at max end of the arc
			dummyTrans.transform.position = lever.transform.position;
			dummyTrans.transform.rotation = lever.transform.rotation;
			dummyTrans.transform.position += dummyTrans.transform.up * lever.length;
			dummyTrans.transform.RotateAround (lever.transform.position, lever.transform.right, limits.max);
			Handles.DrawWireDisc (dummyTrans.transform.position, lever.transform.right, 0.015f);
		} else {
			//draw an arc representing the movement of the lever
			Handles.DrawWireArc (lever.transform.position, lever.transform.right, lever.transform.up, 360, lever.length);
			//draw a empty dot at min end of the arc
			dummyTrans.transform.position = lever.transform.position;
			dummyTrans.transform.rotation = lever.transform.rotation;
			dummyTrans.transform.position += dummyTrans.transform.up * lever.length;
			dummyTrans.transform.RotateAround (lever.transform.position, lever.transform.right, 0);
			Handles.DrawWireDisc (dummyTrans.transform.position, lever.transform.right, 0.015f);
		}

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

	//use this to set the angle of the hingejoint with code
	//the angle must be within the limits of the joint
	public void SetAngle(float angle){
		if (angle >= limitMin && angle <= limitMax) {
			Vector3 newAngle = new Vector3 (angle, 0, 0);
			//before rotating the hinge with script, best make it kinematic
			GetComponent<Rigidbody> ().isKinematic = true;
			transform.localEulerAngles = newAngle;
			//after rotating the hinge with script, best make it not kinematic
			GetComponent<Rigidbody> ().isKinematic = false;
		}
	}

	public void SetLimits(){
		GetComponent<HingeJoint> ().useLimits = useLimits;
		JointLimits limits = GetComponent<HingeJoint> ().limits;
		limits.min = limitMin;
		limits.max = limitMax;
		GetComponent<HingeJoint> ().limits = limits;
	}

	public void SetSpring(){

		//if using limits
		if (useLimits) {
			
			//if using any of the springs
			if (useSpringToMax || useSpringToMid || useSpringToMin) {

				//get the spring settings from the joint
				JointSpring spring = GetComponent<HingeJoint> ().spring;

				//if using spring to maximum, middle and minimum
				if (useSpringToMax && useSpringToMid && useSpringToMin) {

					//use the percentage to set where the hinge springs towards
					if (percentage >= 0.75f) {

						//set spring settings to maximum
						if (springTo != SpringTo.max || spring.spring != springToMax || spring.damper != damperToMax || spring.targetPosition != limitMax) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMax ();
						}

					} else if (percentage >= 0.25f && percentage < 0.75f) {

						//how far is the hinge capable of swinging
						float degreesOfSwing = limitMax - limitMin;
						//the angle in the middle of the entire range
						float middleAngle = limitMax - (degreesOfSwing * 0.5f);

						//set spring settings to middle
						if (springTo != SpringTo.mid || spring.spring != springToMid || spring.damper != damperToMid || spring.targetPosition != middleAngle) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMid ();
						}

					} else {

						//set spring settings to minimum
						if (springTo != SpringTo.min || spring.spring != springToMin || spring.damper != damperToMin || spring.targetPosition != limitMin) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMin ();
						}
					}

				//if using spring to maximum and middle but not to minimum
				} else if (useSpringToMax && useSpringToMid && !useSpringToMin) {

					//use the percentage to set where the hinge springs towards
					if (percentage >= 0.75f) {

						//set spring settings to maximum
						if (springTo != SpringTo.max || spring.spring != springToMax || spring.damper != damperToMax || spring.targetPosition != limitMax) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMax ();
						}

					} else {

						//how far is the hinge capable of swinging
						float degreesOfSwing = limitMax - limitMin;
						//the angle in the middle of the entire range
						float middleAngle = limitMax - (degreesOfSwing * 0.5f);

						//set spring settings to middle
						if (springTo != SpringTo.mid || spring.spring != springToMid || spring.damper != damperToMid || spring.targetPosition != middleAngle) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMid ();
						}
					}

				//if using spring to middle and minimum but not to maximum
				} else if (!useSpringToMax && useSpringToMid && useSpringToMin) {

					//use the percentage to set where the hinge springs towards
					if (percentage >= 0.25f) {

						//how far is the hinge capable of swinging
						float degreesOfSwing = limitMax - limitMin;
						//the angle in the middle of the entire range
						float middleAngle = limitMax - (degreesOfSwing * 0.5f);

						//set spring settings to middle
						if (springTo != SpringTo.mid || spring.spring != springToMid || spring.damper != damperToMid || spring.targetPosition != middleAngle) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMid ();
						}

					} else {

						//set spring settings to minimum
						if (springTo != SpringTo.min || spring.spring != springToMin || spring.damper != damperToMin || spring.targetPosition != limitMin) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMin ();
						}
					}

				//if using spring to maximum and minimum but not to middle
				} else if (useSpringToMax && !useSpringToMid && useSpringToMin) {

					//use the percentage to set where the hinge springs towards
					if (percentage >= 0.5f) {
					
						//set spring settings to maximum
						if (springTo != SpringTo.max || spring.spring != springToMax || spring.damper != damperToMax || spring.targetPosition != limitMax) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMax ();
						}

					} else {
					
						//set spring settings to minimum
						if (springTo != SpringTo.min || spring.spring != springToMin || spring.damper != damperToMin || spring.targetPosition != limitMin) {
							//set the values to the hinge's spring
							GetComponent<HingeJoint> ().spring = SetSpringToMin ();
						}

					}

				//if only using spring to maximum
				} else if (useSpringToMax && !useSpringToMid && !useSpringToMin) {
				
					//set spring settings to maximum
					if (springTo != SpringTo.max || spring.spring != springToMax || spring.damper != damperToMax || spring.targetPosition != limitMax) {
						//set the values to the hinge's spring
						GetComponent<HingeJoint> ().spring = SetSpringToMax ();
					}

				//if only using spring to middle
				} else if (!useSpringToMax && useSpringToMid && !useSpringToMin) {

					//how far is the hinge capable of swinging
					float degreesOfSwing = limitMax - limitMin;
					//the angle in the middle of the entire range
					float middleAngle = limitMax - (degreesOfSwing * 0.5f);

					//set spring settings to middle
					if (springTo != SpringTo.mid || spring.spring != springToMid || spring.damper != damperToMid || spring.targetPosition != middleAngle) {
						//set the values to the hinge's spring
						GetComponent<HingeJoint> ().spring = SetSpringToMid ();
					}

				//if only using spring to minimum
				} else if (!useSpringToMax && !useSpringToMid && useSpringToMin) {

					//set spring settings to minimum
					if (springTo != SpringTo.min || spring.spring != springToMin || spring.damper != damperToMin || spring.targetPosition != limitMin) {
						//set the values to the hinge's spring
						GetComponent<HingeJoint> ().spring = SetSpringToMin ();
					}
				}
		
			//using none of the springs
			} else {

				//get the spring settings from the joint
				JointSpring spring = GetComponent<HingeJoint> ().spring;
			
				//set spring settings to none
				if (springTo != SpringTo.none || spring.spring != 0.0f || spring.damper != 0.0f || spring.targetPosition != 0.0f) {
					//set the values to the hinge's spring
					GetComponent<HingeJoint> ().spring = SetSpringToNone ();
				}
			}
		
		//not using limits
		} else {
			
			//get the spring settings from the joint
			JointSpring spring = GetComponent<HingeJoint> ().spring;

			//use the max spring to control hinges with no limits
			if (useSpringToMax) {
				
				//set spring settings to none
				if (springTo != SpringTo.noLimit || spring.spring != springToMax || spring.damper != damperToMax|| spring.targetPosition != 0.0f) {
					//set the values to the hinge's spring
					GetComponent<HingeJoint> ().spring = SetSpringToNoLimit ();
				}

			//using none of the springs
			} else {

				//set spring settings to none
				if (springTo != SpringTo.none || spring.spring != 0.0f || spring.damper != 0.0f || spring.targetPosition != 0.0f) {
					//set the values to the hinge's spring
					GetComponent<HingeJoint> ().spring = SetSpringToNone ();
				}
			}
		}
	}

	JointSpring SetSpringToNoLimit(){

		//set to not using spring
		GetComponent<HingeJoint> ().useSpring = true;

		//get the spring settings from the joint
		JointSpring spring = GetComponent<HingeJoint> ().spring;

		//set spring settings
		spring.spring = springToMax;			
		spring.damper = damperToMax;
		spring.targetPosition = 0.0f;

		//set where the spring is currently springing to
		springTo = SpringTo.noLimit;

		return spring;
	}

	JointSpring SetSpringToNone(){

		//set to not using spring
		GetComponent<HingeJoint> ().useSpring = false;

		//get the spring settings from the joint
		JointSpring spring = GetComponent<HingeJoint> ().spring;

		//set spring settings
		spring.spring = 0.0f;	
		spring.damper = 0.0f;
		spring.targetPosition = 0.0f;

		//set where the spring is currently springing to
		springTo = SpringTo.none;

		return spring;
	}

	JointSpring SetSpringToMax(){

		//set to using spring
		GetComponent<HingeJoint> ().useSpring = true;

		//get the spring settings from the joint
		JointSpring spring = GetComponent<HingeJoint> ().spring;

		//set spring settings to maximum
		spring.spring = springToMax;			
		spring.damper = damperToMax;			
		spring.targetPosition = limitMax;

		//set where the spring is currently springing to
		springTo = SpringTo.max;

		return spring;
	}

	JointSpring SetSpringToMid(){

		//set to using spring
		GetComponent<HingeJoint> ().useSpring = true;

		//get the spring settings from the joint
		JointSpring spring = GetComponent<HingeJoint> ().spring;

		//how far is the hinge capable of swinging
		float degreesOfSwing = limitMax - limitMin;
		//the angle in the middle of the entire range
		float middleAngle = limitMax - (degreesOfSwing * 0.5f);

		//set spring settings to middle
		spring.spring = springToMid;			
		spring.damper = damperToMid;			
		spring.targetPosition = middleAngle;

		//set where the spring is currently springing to
		springTo = SpringTo.mid;

		return spring;
	}

	JointSpring SetSpringToMin(){

		//set to using spring
		GetComponent<HingeJoint> ().useSpring = true;

		//get the spring settings from the joint
		JointSpring spring = GetComponent<HingeJoint> ().spring;

		//set spring settings to minimum
		spring.spring = springToMin;				
		spring.damper = damperToMin;				
		spring.targetPosition = limitMin;

		//set where the spring is currently springing to
		springTo = SpringTo.min;

		return spring;
	}

	void Update(){
		
		//get the true angle of the hinge between -180 and 180 degrees
		angle = CalcTrueAngle();
		//keep track of movement of the hinge
		CalcTotalRotAndRev ();
		//keeps track if the hinge is closer to minimum or the maximum limit
		CalcPercentage();
		//keeps the spring at the proper settings
		SetSpring();
	}
}

//custom editor for VRBasics_Hinge class
//automatic handling of multi-object handling, undo and prefab overrides
[CustomEditor(typeof(VRBasics_Hinge))]
[CanEditMultipleObjects]
public class VRBasics_HingeEditor : Editor {

	SerializedProperty useLimitsProp;
	SerializedProperty limitMinProp;
	SerializedProperty limitMaxProp;
	SerializedProperty useSpringToMaxProp;
	SerializedProperty springToMaxProp;
	SerializedProperty damperToMaxProp;
	SerializedProperty useSpringToMidProp;
	SerializedProperty springToMidProp;
	SerializedProperty damperToMidProp;
	SerializedProperty useSpringToMinProp;
	SerializedProperty springToMinProp;
	SerializedProperty damperToMinProp;

	protected static bool showSpringToMax = false;
	protected static bool showSpringToMid = false;
	protected static bool showSpringToMin = false;

	void OnEnable(){

		//set up serialized properties
		useLimitsProp = serializedObject.FindProperty ("useLimits");
		limitMinProp = serializedObject.FindProperty ("limitMin");
		limitMaxProp = serializedObject.FindProperty ("limitMax");
		useSpringToMaxProp = serializedObject.FindProperty ("useSpringToMax");
		springToMaxProp = serializedObject.FindProperty ("springToMax");
		damperToMaxProp = serializedObject.FindProperty ("damperToMax");
		useSpringToMidProp = serializedObject.FindProperty ("useSpringToMid");
		springToMidProp = serializedObject.FindProperty ("springToMid");
		damperToMidProp = serializedObject.FindProperty ("damperToMid");
		useSpringToMinProp = serializedObject.FindProperty ("useSpringToMin");
		springToMinProp = serializedObject.FindProperty ("springToMin");
		damperToMinProp = serializedObject.FindProperty ("damperToMin");
	}

	public override void OnInspectorGUI ()
	{
		//reference to the class of object used to display gizmo
		VRBasics_Hinge hinge = (VRBasics_Hinge) target;

		//always update serialized properties at start of OnInspectorGUI
		serializedObject.Update ();

		//start listening for changes in inspector values
		EditorGUI.BeginChangeCheck ();

		//display serialized properties in inspector
		EditorGUILayout.PropertyField (useLimitsProp, new GUIContent ("Use Limits"));
		if (hinge.useLimits) {
			//limited to -179.9 because -180 would be the same as 180, would effect spring negatively
			EditorGUILayout.Slider (limitMinProp, 0.0f, -179.9f, new GUIContent ("Limit Minimum"));
			EditorGUILayout.Slider (limitMaxProp, 0.0f, 180.0f, new GUIContent ("Limit Maximum"));
		}

		EditorGUILayout.PropertyField (useSpringToMaxProp, new GUIContent ("Use Spring To Max"));
		if (hinge.useSpringToMax) {
			showSpringToMax = EditorGUILayout.Foldout (showSpringToMax, "Spring To Max");
			if (showSpringToMax) {
				EditorGUILayout.PropertyField (springToMaxProp, new GUIContent ("Spring"));
				EditorGUILayout.PropertyField (damperToMaxProp, new GUIContent ("Damper"));
			}
		}

		EditorGUILayout.PropertyField (useSpringToMidProp, new GUIContent ("Use Spring To Middle"));
		if (hinge.useSpringToMid) {
			showSpringToMid = EditorGUILayout.Foldout (showSpringToMid, "Spring To Middle");
			if (showSpringToMid) {
				EditorGUILayout.PropertyField (springToMidProp, new GUIContent ("Spring"));
				EditorGUILayout.PropertyField (damperToMidProp, new GUIContent ("Damper"));
			}
		}

		EditorGUILayout.PropertyField (useSpringToMinProp, new GUIContent ("Use Spring To Min"));
		if (hinge.useSpringToMin) {
			showSpringToMin = EditorGUILayout.Foldout (showSpringToMin, "Spring To Min");
			if (showSpringToMin) {
				EditorGUILayout.PropertyField (springToMinProp, new GUIContent ("Spring"));
				EditorGUILayout.PropertyField (damperToMinProp, new GUIContent ("Damper"));
			}
		}


		//if there were any changes in inspector values
		if (EditorGUI.EndChangeCheck ()) {

			//apply changes to serialized properties
			serializedObject.ApplyModifiedProperties ();

			//adjust the limits of the hinge
			hinge.SetLimits ();

			//adjust the spring of the hinge
			hinge.SetSpring();
		}

		//only show the angle in editor when in play mode
		if (Application.isPlaying) {
			EditorGUILayout.LabelField ("Angle", hinge.angle.ToString ());
			EditorGUILayout.LabelField ("Percentage", hinge.percentage.ToString ());
			EditorGUILayout.LabelField ("Angle Frame Change", hinge.angleFrameChange.ToString ());
			EditorGUILayout.LabelField ("Total Rotation", hinge.totalRotation.ToString ());
			EditorGUILayout.LabelField ("Total Revolutions", hinge.totalRevolutions.ToString ());
		}
	}

	void OnSceneGUI() {

		//reference to the class of object used to display gizmo
		VRBasics_Hinge hinge = (VRBasics_Hinge) target;

		//reference to the hinge joint the gizmo displays
		HingeJoint hingeJoint = hinge.GetComponent<HingeJoint> ();

		if (hingeJoint.useLimits != hinge.useLimits) {
			hingeJoint.useLimits = hinge.useLimits;
		}

		if (hingeJoint.limits.min < -179.9f) {
			hinge.limitMin = -179.9f;
			//adjust the limits of the hinge
			hinge.SetLimits ();
		}

		if (hingeJoint.limits.max < 0.0f) {
			hinge.limitMax = 0.0f;
			//adjust the limits of the hinge
			hinge.SetLimits ();
		}

		//this checks if the limits of the hinge joint were changed on the hinge joint itself
		if (hingeJoint.limits.max != hinge.limitMax || hingeJoint.limits.min != hinge.limitMin) {
			hinge.limitMax = hingeJoint.limits.max;
			hinge.limitMin = hingeJoint.limits.min;
			//adjust the limits of the hinge
			hinge.SetLimits ();
		}

		//in edit mode
		if (!Application.isPlaying) {
			//at runtime this is taken care of by the physics engine
			//prevent the hinge from moving away from the fulcrum of the lever during edit mode
			hinge.transform.localPosition = Vector3.zero;
			//prevents the hinge from rotating away from the fulcrum of the parent
			hinge.transform.localEulerAngles = Vector3.zero;
		}

		//reference to the parent class
		VRBasics_Lever lever = hinge.transform.parent.gameObject.GetComponent<VRBasics_Lever> ();

		//DRAW HINGE
		hinge.DrawGizmo();

		//DRAW LEVER
		lever.DrawGizmo();
	}
}