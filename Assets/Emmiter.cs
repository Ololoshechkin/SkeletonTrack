using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Emmiter")]
public class Emmiter : MonoBehaviour {

	public Transform self;
	public Transform target;
	private Vector3 dir;

	void emitBall() {
		var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		ball.transform.position = self.transform.position;
		ball.AddComponent<Rigidbody>();
		ball.GetComponent<Rigidbody>().velocity = 2.0f * dir;
	}

	// Use this for initialization
	void Start () {
		dir = target.transform.position - self.transform.position;
		InvokeRepeating("emitBall", 0.2f, 1.0f);
		Physics.gravity = new Vector3(0, -0.5F, 0);
	}
	
	// Update is called once per frame
	void Update () {
			
	}
}
