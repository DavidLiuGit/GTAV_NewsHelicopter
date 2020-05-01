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
			Interval = 199;
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
				_enabledOnWanted = _ss.GetValue<bool>("NewsHeli", "onWanted", true);
				_heliCtrl = new HeliController(_ss);
				_toggleCamKey = _ss.GetValue<Keys>("HeliCam", "activateKey", Keys.Return);
			}

			
			// if a heli is already active:
			if (_heliCtrl.isActive)
			{
				if (_enabledOnWanted)
				{
					if (Game.Player.WantedLevel >= 3)
						_heliCtrl.onTick();
					else _heliCtrl.instanceDestructor(false);	// if player has less than 3 stars, dismiss active heli
				}
			}

			// if no heli is active
			else
			{
				if (_enabledOnWanted && Game.Player.WantedLevel >= 3)
					_heliCtrl.spawnMannedHeliInPursuit();
			}
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
					else if (e.KeyCode == Keys.OemMinus)
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
	}
}
