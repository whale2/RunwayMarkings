using System;
using System.IO;
using UnityEngine;
using KerbalKonstructs.Core;

namespace BWStatics
{
	public class AutoKourseModule: StaticModule
	{
		public AutoKourseModule ()
		{
		}

		public void Awake() {

			setTextures();
		}

		public override void StaticObjectUpdate() {

			setTextures ();
		}

		private void setTextures() {

			int heading = getHeading ();
			if (heading % 10 > 5)
				heading += 10 - heading % 10;
			if (heading < 6)
				heading = 360; // There are no 00 runways, they all are 36!
				
			int dg0 = heading / 10 % 10;
			int dg1 = heading / 100 % 10;

			Debug.Log ("AutoKourseModule: heading = " + heading);
			try {
				paintDigits("digit0_obj", "digit1_obj", dg0, dg1);
			}
			catch {
				Debug.Log ("AutoKourseModule: Failed in setTextures() - does the static contain objects 'digit0_obj' and 'digit1_obj'?");
				return;
			}
			// If we've got so far, let's try to find reverse digits
			dg1 = 3 - dg1;
			dg1 = dg1 < 0 ? dg1 += 10 : dg1;
			dg0 = 6 - dg0;
			dg0 = dg0 < 0 ? dg0 += 10 : dg0;
			try {
				paintDigits("rev_digit0_obj", "rev_digit1_obj", dg0, dg1);
			}
			catch {
			}
		}

		private void paintDigits(String name0, String name1, int dg0, int dg1) {
			
			Transform digit0 = gameObject.transform.GetChild(0).FindChild(name0);
			Transform digit1 = gameObject.transform.GetChild(0).FindChild(name1);

			Renderer dg0renderer = digit0.GetComponent<Renderer>();
			Renderer dg1renderer = digit1.GetComponent<Renderer>();

			dg0renderer.material.SetTextureOffset("_MainTex",  new Vector2(dg0 * 0.1f,0));
			dg1renderer.material.SetTextureOffset("_MainTex",  new Vector2(dg1 * 0.1f,0));
		}

		private int getHeading() {
			
			CelestialBody body = FlightGlobals.ActiveVessel.mainBody;
			Vector3 upVector = body.GetSurfaceNVector(
				FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude).normalized;
			Vector3 north = Vector3.ProjectOnPlane(body.transform.up, upVector).normalized;
			Vector3 east = Vector3.Cross(upVector, north).normalized;
			Vector3 forward = Vector3.ProjectOnPlane(gameObject.transform.forward, upVector);
			float heading = Vector3.Angle (forward, north);
			if (Vector3.Dot (forward, east) < 0)
				heading = 360 - heading;
			return (int)heading;
		}
	}
}

