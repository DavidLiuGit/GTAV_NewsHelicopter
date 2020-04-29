using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;


namespace NewsHeli
{
	class HeliController
	{
		#region properties
		// vehicle
		public bool isActive;
		public Vehicle activeHeli;
		public Ped activePilot;
		public Ped activePaparazzi;
		private string _modelName;
		private Model _model;
		private float _altitude;
		private float _radius;

		// Camera
		private float _defaultFov;
		private float _currentFov;
		public Camera heliCam;
		public bool isRenderingFromHeliCam;
		#endregion



		#region constructor
		/// <summary>
		/// Initialize Heli controller
		/// </summary>
		/// <param name="ss"></param>
		public HeliController(ScriptSettings ss)
		{
			readNewsHeliSettings(ss);
			_model = (Model)Game.GenerateHash(_modelName);
			isRenderingFromHeliCam = false;
		}
		#endregion




		#region publicMethods
		/// <summary>
		/// Invoke this method to control the helicopter
		/// </summary>
		public void onTick()
		{
			// if no heli is active, do nothing
			if (!isActive)
				return;

			// if the heli is no longer operable, destroy gracefully
			if (!isHeliOperable(activeHeli))
			{
				instanceDestructor(false);
				return;
			}

			taskPilotChasePlayer(activePilot);
		}



		/// <summary>
		/// Spawn a News Heli and its crew
		/// </summary>
		/// <returns>instance of <c>Vehicle</c></returns>
		public Vehicle spawnMannedHeliInPursuit()
		{
			// determine a spawn position
			Vector3 spawnPos = Game.Player.Character.Position.Around(_radius);
			spawnPos.Z += _altitude;

			// spawn the heli & activePilot
			activeHeli = World.CreateVehicle(_model, spawnPos);
			activeHeli.IsEngineRunning = true;
			activeHeli.HeliBladesSpeed = 1.0f;
			activePilot = activeHeli.CreatePedOnSeat(VehicleSeat.Driver, PedHash.ReporterCutscene);
			activePaparazzi = activeHeli.CreatePedOnSeat(VehicleSeat.Passenger, PedHash.Beverly);

			// task activePilot with chasing the player
			taskPilotChasePlayer(activePilot);

			// create the camera attached to the heli
			heliCam = initializeHeliCamera(activeHeli);

			isActive = true;
			return activeHeli;
		}



		/// <summary>
		/// Destroy the active heli and its crew and assets
		/// </summary>
		/// <param name="force">If destroying by force, assets are deleted immediately</param>
		public void instanceDestructor(bool force = true)
		{
			// if destroying by force, delete everything right away
			if (force)
			{
				activePaparazzi.Delete();
				activePilot.Delete();
				heliCam.Delete();
				activeHeli.Delete();
			}

			// otherwise, mark as ready for deletion (except for camera)
			else
			{
				heliCam.Delete();
				activePaparazzi.MarkAsNoLongerNeeded();
				activePilot.MarkAsNoLongerNeeded();
				activeHeli.MarkAsNoLongerNeeded();
			}

			isRenderingFromHeliCam = false;
			World.RenderingCamera = null;		// reset rendering cam to gameplay cam
			isActive = false;
		}



		/// <summary>
		/// Toggle between the gameplay camera and the news heli camera
		/// </summary>
		public void toggleHeliCam()
		{
			if (isRenderingFromHeliCam){
				World.RenderingCamera = null;
				isRenderingFromHeliCam = false;
			}
			else
			{
				World.RenderingCamera = heliCam;
				isRenderingFromHeliCam = true;
			}
		}
		#endregion





		#region helperMethods
		/// <summary>
		/// Read INI settings for News Heli
		/// </summary>
		/// <param name="ss">instance of <c>ScriptSettings</c> to read settings from</param>
		private void readNewsHeliSettings (ScriptSettings ss)
		{
			string section = "NewsHeli";
			_modelName = ss.GetValue<string>(section, "model", "frogger");
			_radius = ss.GetValue<float>(section, "radius", 40f);
			_altitude = ss.GetValue<float>(section, "altitude", 40f);

			section = "HeliCam";
			_defaultFov = ss.GetValue<float>(section, "defaultFov", 60f);
		}



		/// <summary>
		/// Task the specified pilot of a helicopter with chasing the Player
		/// </summary>
		/// <param name="pilot">Pilot to be tasked</param>
		private void taskPilotChasePlayer(Ped pilot)
		{
			// task the activePilot with chasing the player
			activePilot.Task.ChaseWithHelicopter(Game.Player.Character, Vector3.Zero.Around(_radius));
			activePilot.AlwaysKeepTask = true;
		}



		/// <summary>
		/// Initialize the News Heli camera
		/// </summary>
		/// <param name="heli">instance of <c>Vehicle</c> to attach the camera to</param>
		/// <returns>instance of <c>Camera</c></returns>
		private Camera initializeHeliCamera(Vehicle heli)
		{
			Camera cam = World.CreateCamera(Vector3.Zero, Vector3.Zero, _defaultFov);
			Vector3 offset = Vector3.Zero;
			offset.Z -= 1.0f;
			cam.AttachTo(heli, offset);
			cam.PointAt(Game.Player.Character);
			return cam;
		}



		/// <summary>
		/// Determine whether the heli is still in operable condition. That is, the heli must be driveable, 
		/// and must have a living pilot (as dark as that sounds)
		/// </summary>
		/// <param name="heli">instance of <c>Vehicle</c> to evaluate</param>
		private bool isHeliOperable(Vehicle heli)
		{
			// if heli no longer driveable, return false
			if (!heli.IsDriveable) return false;

			// check the pilot
			Ped pilot = heli.Driver;
			if (pilot == null || pilot.IsDead)
				return false;

			return true;
		}
		#endregion
	}
}
