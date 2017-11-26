using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SceletonRenderer")]
public class SceletonRenderer : MonoBehaviour {

	public GameObject rocketPrefab;

	private string[] jointNames = {
		"HEAD", "NECK", 
		"LEFT_SHOWDER", "RIGHT_SHOWDER",
		"LEFT_ELBOW", "RIGHT_ELBOW",
		"LEFT_WRIST", "RIGHT_WRIST",
		"LEFT_HAND", "RIGHT_HAND",
		"LEFT_KNEE", "RIGHT_KNEE",
		"SPINE", 
		"LEFT_TOE", "RIGHT_TOE"
	};
	private class Joint {
		public string name;
		public GameObject obj;
		public ArrayList children = new ArrayList ();
		private GameObject rocket = null;
		private GameObject rocketPref;
		public Joint(string name, GameObject pref = null) {
			this.name = name;
			this.rocketPref = pref;
			obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			if (pref != null) {
				rocket = Instantiate(
					rocketPref,
					new Vector3(
						0.0f, 
						0.0f, 
						0.0f
					),
					Quaternion.identity
				);
				rocket.AddComponent<Rigidbody>();
				rocket.GetComponent<Rigidbody>().useGravity = false;
			}
		}
		private float getLength(string name1, string name2) {
			return 1.0f;
		}
		private float dist(Vector3 a, Vector3 b) {
			return Mathf.Sqrt(
				(a.x - b.x) * (a.x - b.x) +
				(a.y - b.y) * (a.y - b.y) +
				(a.z - b.z) * (a.z - b.z)
			);
		}
		private void createBone(ArrayList bones, GameObject joint1, GameObject joint2) {
			var bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			var pos1 = joint1.transform.position;
			var pos2 = joint2.transform.position;
	        bone.transform.position = joint1.transform.position;
			bone.transform.LookAt(joint2.transform);
			bone.transform.localScale = new Vector3 (
				0.1f,
				0.1f,
				dist(
					joint1.transform.position,
					joint2.transform.position
				) * 0.75f
			);
			bone.transform.position = new Vector3(
	        	(pos1.x + pos2.x) / 2.0f, 
	        	(pos1.y + pos2.y) / 2.0f,
	        	(pos1.z + pos2.z) / 2.0f
	        );
			bones.Add(bone);
		}
		public void dfs(Dictionary<string, Dictionary<string, Vector3>> jointToAngle, ArrayList bones) {
			//Debug.Log (name);
			foreach (Joint j in children) {
				if (jointToAngle.ContainsKey (name) && jointToAngle [name].ContainsKey (j.name)) {
					var vector = jointToAngle [name] [j.name];
					j.obj.transform.position = obj.transform.position + getLength (name, j.name) * vector;
					createBone (bones, obj, j.obj);
					if (j.rocket != null) {
						var a = obj.transform.position;
						var b = j.obj.transform.position;
						var v = (b - a);
						j.rocket.transform.position = b;
						j.rocket.transform.rotation = Quaternion.Inverse(
							((GameObject)bones[bones.Count - 1])
							.transform
							.rotation
						);
						j.rocket.transform.localScale = new Vector3(
							0.0005f, 
							0.0005f,
							0.0005f
						);
					}
					// TODO ^^
					j.dfs (jointToAngle, bones);
				}
			}
		}
	}
	public Dictionary<string, Dictionary<string, Vector3>> jointToAngle = new Dictionary<
		string, 
		Dictionary<string, Vector3>
	> ();
	private Joint head;
	private ArrayList bones = new ArrayList ();

	public void updateJointPositions() {
		foreach (GameObject bone in bones) {
			Destroy (bone);
		}
		bones.Clear();
		head.dfs(jointToAngle, bones);
	}



	// Use this for initialization
	void Start () {

		//Debug.Log ("Start");

		foreach (string name in jointNames) {
			//Debug.Log ("init" + name);
			jointToAngle.Add (name, new Dictionary<string, Vector3>());
		}
		//Debug.Log ("all");

		jointToAngle ["HEAD"].Add ("NECK", new Vector3(0.0f, -0.5f, 0.0f));
		jointToAngle ["NECK"].Add ("LEFT_SHOWDER", new Vector3(1.0f, -1.0f, 0.0f));
		jointToAngle ["NECK"].Add ("RIGHT_SHOWDER", new Vector3(-1.0f, -1.0f, 0.0f));
		jointToAngle ["NECK"].Add ("SPINE", new Vector3(0.0f, -2.0f, 0.0f));
		jointToAngle ["LEFT_SHOWDER"].Add ("LEFT_ELBOW", new Vector3(0.5f, -0.5f, -1.0f));
		jointToAngle ["RIGHT_SHOWDER"].Add ("RIGHT_ELBOW", new Vector3(-0.5f, -0.5f, 0.0f));
		jointToAngle ["LEFT_ELBOW"].Add ("LEFT_HAND", new Vector3(0.0f, -0.35f, 0.0f));
		jointToAngle ["RIGHT_ELBOW"].Add ("RIGHT_HAND", new Vector3(0.0f, -0.35f, 0.0f));
		jointToAngle ["SPINE"].Add ("LEFT_KNEE", new Vector3(1.0f, -1.5f, 0.0f));
		jointToAngle ["SPINE"].Add ("RIGHT_KNEE", new Vector3(-1.0f, -1.5f, 0.0f));
		jointToAngle ["LEFT_KNEE"].Add ("LEFT_TOE", new Vector3(0.0f, -1.0f, 0.0f));
		jointToAngle ["RIGHT_KNEE"].Add ("RIGHT_TOE", new Vector3(0.0f, -1.0f, 0.0f));

		/*new Vector3 (1.0, 1.0, 0.0);
		jointToAngle [new KeyValuePair ("HEAD", "RIGHT_SHOWDER")] = new Vector3 (1.0, -1.0, 0.0);
		jointToAngle [new KeyValuePair ("HEAD", "SPINE")] = new Vector3 (1.0, 0.0, 0.0);
*/

		//Debug.Log ("head:");

		head = new Joint("HEAD");
		//Debug.Log ("other:");
		var leftShowder = new Joint("LEFT_SHOWDER");
		var rightShowder = new Joint("RIGHT_SHOWDER");
		var leftElbow = new Joint("LEFT_ELBOW");
		var rightElbow = new Joint("RIGHT_ELBOW");
		var leftHand = new Joint("LEFT_HAND");
		var rightHand = new Joint("RIGHT_HAND", rocketPrefab);
		var leftKnee = new Joint("LEFT_KNEE");
		var rightKnee = new Joint("RIGHT_KNEE");
		var spine = new Joint("SPINE");
		var neck = new Joint("NECK");
		var leftToe = new Joint("LEFT_TOE");
		var rightToe = new Joint("RIGHT_TOE");

		//Debug.Log ("children.add:");

		head.children.Add (neck);
		neck.children.Add(leftShowder);
		neck.children.Add(rightShowder);
		neck.children.Add(spine);
		leftShowder.children.Add(leftElbow);
		rightShowder.children.Add(rightElbow);
		leftElbow.children.Add(leftHand);
		rightElbow.children.Add(rightHand);
		spine.children.Add(leftKnee);
		spine.children.Add(rightKnee);
		leftKnee.children.Add(leftToe);
		rightKnee.children.Add(rightToe);
		//Debug.Log ("dfs:");
		updateJointPositions ();
	}
	
	// Update is called once per frame
	void Update () {
		//updateJointPositions();
	}

}
