﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;
using GTA.UI;


namespace NewsHeli
{
	class HeliController
	{
		#region properties
		// vehicle
		public bool isActive;
		public Vehicle activeHeli;
		private string _modelName;
		private Model _model;
		private float _altitude;
		private float _radius;
		private int _respawnDelay;		// in milliseconds. Specify in seconds in .ini
		private int _lastAliveTime;
		private const float _spawnRadiusMultiplier = 8.0f;
		private const float _spawnHeightMultiplier = 3.0f;
		private const int _staticFeedDuration = 6500;	// (in milliseconds) duration to show static if heli inoperable

		// crew
		public Ped activePilot;
		private const PedHash _defaultPilotHash = PedHash.ReporterCutscene;
		private RelationshipGroup _newsRG;

		// tasking
		private const int _chaseRetaskTicks = 2000;
		private int _tickCount = 0;

		// Camera
		private float _defaultFov;
		private float _currentFov;
		public Camera heliCam;
		public bool isRenderingFromHeliCam;
		private float _zoomFactor;
		private bool _showScaleformOverlay;
		private string _title;
		private string _subtitle;

		// debug
		private bool _verbose = false;
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

			// relationships
			_newsRG = new RelationshipGroup(Game.GenerateHash("news_team")); //RelationshipGroup("news_team");
			//_newsRG.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, Relationship.Companion, true);
			Game.Player.Character.RelationshipGroup.SetRelationshipBetweenGroups(_newsRG, Relationship.Like, true);

			// default settings & flags
			_lastAliveTime = int.MinValue;
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

			// show breaking news Scaleform overlay when camera is active
			if (this.isRenderingFromHeliCam && this._showScaleformOverlay)
				CameraControl.enableBreakingNewsOverlay();

			// if the heli is no longer operable, destroy gracefully
			if (!isHeliOperable(activeHeli))
			{
				//instanceDestructor(false);
				heliDestroyedHandler();
				return;
			}

			// retask the pilot's chase if tickCount is a multiple of _chaseRetaskTicks constant
			if (_tickCount % _chaseRetaskTicks == 0)
				taskPilotChasePlayer(activePilot);

			// increment tickCount
			_tickCount++;

			// record last active time
			_lastAliveTime = Game.GameTime;
		}



		/// <summary>
		/// Spawn a News Heli and its crew
		/// </summary>
		/// <returns>instance of <c>Vehicle</c></returns>
		public Vehicle spawnMannedHeliInPursuit()
		{
			// determine a spawn position
			float spawnRadius = Math.Min(_radius * _spawnRadiusMultiplier, 400f);
			float spawnHeight = Math.Min(_altitude * _spawnHeightMultiplier, 170f);
			Vector3 spawnPos = Game.Player.Character.Position.Around(spawnRadius);
			spawnPos.Z += spawnHeight;

			// spawn the heli & activePilot
			activeHeli = World.CreateVehicle(_model, spawnPos);

			// error checking
			if (activeHeli == null)
			{
				Notification.Show("News Heli: ~r~Failed to spawn heli " + _modelName);
				return activeHeli;
			}

			// configure heli
			activeHeli.IsEngineRunning = true;
			activeHeli.HeliBladesSpeed = 1.0f;
			spawnAndConfigureHeliCrew();

			// task activePilot with chasing the player
			taskPilotChasePlayer(activePilot);

			// create the camera attached to the heli
			heliCam = initializeHeliCamera(activeHeli);

			// update news overlay
			if (_showScaleformOverlay)
			{
				CameraControl.updateNewsText(_title, _subtitle);	// if no subtitle set, a random subtitle is chosen
				CameraControl.showStatic(-1);						// disable static feed
			}

			isActive = true;
			return activeHeli;
		}



		/// <summary>
		/// Destroy the active heli and its crew and assets
		/// </summary>
		/// <param name="force">If destroying by force, assets are deleted immediately</param>
		public void instanceDestructor(bool force = true)
		{
			_lastAliveTime = Game.GameTime;

			// if destroying by force, delete everything right away
			if (force)
			{
				activePilot.Delete();
				heliCam.Delete();
				activeHeli.Delete();
			}

			// otherwise, mark as ready for deletion (except for camera)
			else
			{
				heliCam.Delete();
				activePilot.Task.FleeFrom(Game.Player.Character);
				activePilot.MarkAsNoLongerNeeded();
				activeHeli.MarkAsNoLongerNeeded();
			}

			isRenderingFromHeliCam = false;
			World.RenderingCamera = null;		// reset rendering cam to gameplay cam
			isActive = false;
		}



		/// <summary>
		/// When the heli is rendered inoperable, gracefully destroy. 
		/// </summary>
		private void heliDestroyedHandler()
		{
			// determine if instance destructor should be called
			if (Game.GameTime > _staticFeedDuration + _lastAliveTime)
			{
				instanceDestructor(false);
				return;
			}

			// show static
			CameraControl.showStatic(1);
		}



		/// <summary>
		/// Toggle between the gameplay camera and the news heli camera
		/// </summary>
		/// <returns>whether the news heli camera is rendering</returns>
		public bool toggleHeliCam()
		{
			// sanity check: if no heli is active, stop execution
			if (!isActive)
			{
				instanceDestructor();
				if (_verbose) Notification.Show("while toggling heli cam, heli was NOT active");
				return false;
			}

			// if currently rendering from heli cam, then reset to gameplay cam
			if (isRenderingFromHeliCam){
				World.RenderingCamera = null;
				isRenderingFromHeliCam = false;
			}

			// if not currently rendering from heli cam, then set heli cam as active rendering cam
			else
			{
				// if heli cam does not exist (deleted for some reason), reinitialize it
				if (!heliCam.Exists())
				{
					heliCam = initializeHeliCamera(activeHeli);
					if (_verbose) Notification.Show("while toggling heli cam, heli cam did not exist! Initializing now");
				}

				CameraControl.enableBreakingNewsOverlay(true);
				World.RenderingCamera = heliCam;
				isRenderingFromHeliCam = true;
			}

			return isRenderingFromHeliCam;
		}



		/// <summary>
		/// Modify the field-of-view of the camera, effectively zooming the camera in or out.
		/// No action is taken if the camera is NOT currently rendering
		/// </summary>
		/// <param name="zoomIn">If <c>true</c>, zoom in. If false, zoom out</param>
		/// <returns>The updated field-of-view of the camera</returns>
		public float zoomCamera(bool zoomIn)
		{
			// if heli camera is not currently rendering, do nothing
			if (!isRenderingFromHeliCam)
				return _currentFov;

			// compute the new camera field-of-view
			if (zoomIn)
				_currentFov *= 1f - _zoomFactor;
			else
				_currentFov *= 1f + _zoomFactor;

			// constrain FOV
			if (_currentFov < 1.0f) _currentFov = 1.0f;
			else if (_currentFov > 360f) _currentFov = 360f;
			
			// apply new camera fov
			heliCam.FieldOfView = _currentFov;
			if (_verbose) GTA.UI.Screen.ShowHelpTextThisFrame("News Heli Cam FOV: " + _currentFov + " degrees");

			return _currentFov;
		}



		/// <summary>
		/// Determine whether enough time has elapsed since the heli was last active to spawn it again
		/// </summary>
		/// <returns><c>true</c> if enough time has elapsed</returns>
		public bool canAutoSpawn()
		{
			if (Game.GameTime > _lastAliveTime + _respawnDelay)
				return true;
			else return false;
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
			_radius = ss.GetValue<float>(section, "radius", 50f);
			_altitude = ss.GetValue<float>(section, "altitude", 50f);
			_respawnDelay = ss.GetValue<int>(section, "respawnDelay", 30) * 1000;

			section = "HeliCam";
			_defaultFov = ss.GetValue<float>(section, "defaultFov", 50f);
			_zoomFactor = ss.GetValue<float>(section, "zoomFactor", 10f) / 100f;
			_showScaleformOverlay = ss.GetValue<bool>(section, "showWeazelOverlay", true);
			_title = ss.GetValue<string>(section, "title", "");
			_subtitle = ss.GetValue<string>(section, "subtitle", "");

			section = "debug";
			_verbose = ss.GetValue<bool>(section, "verbose", false);
		}



		/// <summary>
		/// Spawn the crew of the heli, and apply configurations to the Peds
		/// </summary>
		private void spawnAndConfigureHeliCrew()
		{
			activePilot = activeHeli.CreatePedOnSeat(VehicleSeat.Driver, _defaultPilotHash);
			if (activePilot == null)
				Notification.Show("News Heli: ~r~Failed to spawn heli pilot " + _defaultPilotHash.ToString());

			activePilot.RelationshipGroup = _newsRG;
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
			// create camera
			Camera cam = World.CreateCamera(Vector3.Zero, Vector3.Zero, _defaultFov);
			_currentFov = _defaultFov;

			// error checking
			if (cam == null)
			{
				Notification.Show("News Heli: ~r~Failed to create camera for News Heli");
				return cam;
			}

			// determine the offset from center to mount the camera
			// Model.Dimensions: (Item1: rearBottomLeft, Item2: frontTopRight)
			ValueTuple<Vector3, Vector3> heliDimensions = heli.Model.Dimensions;
			Vector3 offset = new Vector3(0f, heliDimensions.Item2.Y / 2, heliDimensions.Item1.Z);

			// configure camera
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
