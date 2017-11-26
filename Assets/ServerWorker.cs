using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

public class ServerWorker : MonoBehaviour {

	private SceletonRenderer renderer;

	float[] parseJson(string s) {
		string[] tokens = s.Replace("[", "")
			.Replace("]", "")
			.Split(' ');
		float[] f = new float[12];
		for (int i = 0; i < 12; i++) {
			f[i] = float.Parse(
				tokens[i]
			);
		}
		return f;
	}


	// Use this for initialization

	void Start () {
		renderer = GetComponent<SceletonRenderer> ();
		Socket socket = new Socket(
			AddressFamily.InterNetwork, 
			SocketType.Stream, 
			ProtocolType.IP
		);
		bool noConnection = true;
		while (noConnection) {
			try {
				socket.Connect(new IPEndPoint(
					IPAddress.Parse("127.0.0.1"), 
					6999
				));
				noConnection = false;
			} catch (Exception e) {
				noConnection = true;
			}
		}
		byte[] b = new byte[1024];
        int k = socket.Receive(b);
        var result = "";
        for (int i = 0; i < k; i++) {
            result += Convert.ToChar(b[i]);
        }
        Debug.Log(result);
        var f = parseJson(result);
        renderer.jointToAngle["LEFT_SHOWDER"]["LEFT_ELBOW"] = new Vector3(f[0], f[1], f[2]);
        renderer.jointToAngle["LEFT_ELBOW"]["LEFT_HAND"] = new Vector3(f[3], f[4], f[5]);
        renderer.jointToAngle["RIGHT_SHOWDER"]["RIGHT_ELBOW"] = new Vector3(f[6], f[7], f[8]);
        renderer.jointToAngle["RIGHT_ELBOW"]["RIGHT_HAND"] = new Vector3(f[9], f[10], f[11]);
        renderer.updateJointPositions();
	}
	
	// Update is called once per frame
	void Update () {
		/*using (var webClient = new System.Net.WebClient()) {
			var json = webClient.DownloadString("https://api.vk.com/method/users.get?user_ids=210700286&fields=bdate&v=5.69");
			Debug.Log (json);
		}*/
	}
}
