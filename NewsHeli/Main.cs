using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using GTA;
using GTA.Native;
using GTA.Math;



namespace NewsHeli
{
	public class Main : Script
	{
		// You can set your mod information below! Be sure to do this!
		bool firstTime = true;
		string ModName = "News Heli";
		string Developer = "iLike2Teabag";


		public Main()
		{
			Tick += onTick;
			KeyDown += onKeyDown;
			Interval = 5;
			Aborted += onAbort;
		}



		private void onTick(object sender, EventArgs e)
		{
			if (firstTime) // if this is the users first time loading the mod, this information will appear
			{
				GTA.UI.Notification.Show(ModName + " by " + Developer + " Loaded");
				firstTime = false;

				// initialization
				_ss = base.Settings;
				_minimumWantedLevel = _ss.GetValue<int>("Newsheli", "minWantedLevel", 3);
				_enabledOnWanted = _ss.GetValue<bool>("NewsHeli", "onWanted", true);
				_heliCtrl = new HeliController(_ss);
				_toggleCamKey = _ss.GetValue<Keys>("HeliCam", "activateKey", Keys.Return);
				new CameraControl();
			}

			
			// if a heli is already active:
			if (_heliCtrl.isActive)
			{
				if (_enabledOnWanted)
				{
					if (Game.Player.WantedLevel >= _minimumWantedLevel)
						_heliCtrl.onTick();
					else _heliCtrl.instanceDestructor(false);	// if player has less than 3 stars, dismiss active heli
				}
			}

			// if no heli is active
			else
			{
				// automatically spawn if player is wanted & heli can auto spawn
				if (_enabledOnWanted && Game.Player.WantedLevel >= _minimumWantedLevel && _heliCtrl.canAutoSpawn())
				{
					Vehicle heli = _heliCtrl.spawnMannedHeliInPursuit();
					if (heli == null) _enabledOnWanted = false;
				}
					
			}


			// detect gamepad input
			if (_heliCtrl.isActive && Game.IsControlJustPressed(_gamepadActivate))
			{
				// toggle camera
				if (!_justActivated && Game.IsControlPressed(_gamepadModifier))
				{
					_heliCtrl.toggleHeliCam();
					_justActivated = true;
				}

				// zoom control
				else if (Game.IsControlPressed(GTA.Control.LookUpOnly))
					_heliCtrl.zoomCamera(true);
				else if (Game.IsControlPressed(GTA.Control.LookDownOnly))
					_heliCtrl.zoomCamera(false);
			}
			else _justActivated = false;
		}


		private void onKeyDown(object sender, KeyEventArgs e)
		{
			if (_heliCtrl.isActive)
			{
				// if the toggle camera key was pressed:
				if (e.KeyCode == _toggleCamKey)
					_heliCtrl.toggleHeliCam();

				// if control key also pressed:
				else if (e.Modifiers == Keys.Control)
				{
					if (e.KeyCode == Keys.Oemplus)
						_heliCtrl.zoomCamera(true);
					else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Oem6)
						_heliCtrl.zoomCamera(false);
				}
			}
		}


		private void onAbort(object sender, EventArgs e)
		{
			_heliCtrl.instanceDestructor(true);
		}



		private bool _enabledOnWanted = true;
		private ScriptSettings _ss;
		private HeliController _heliCtrl;
		private Keys _toggleCamKey;
		private int _minimumWantedLevel = 3;

		// gamepad
		private GTA.Control _gamepadModifier = GTA.Control.CharacterWheel;
		private GTA.Control _gamepadActivate = GTA.Control.LookBehind;
		private bool _justActivated = false;			// prevents double-tapping
	}
}
