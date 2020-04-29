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
			Interval = 999;
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


			if (_heliCtrl.isActive)
				_heliCtrl.onTick();

			else if (Game.Player.WantedLevel >= 3 && _enabledOnWanted)
				_heliCtrl.spawnMannedHeliInPursuit();
		}


		private void onKeyDown(object sender, KeyEventArgs e)
		{
			if (_heliCtrl.isActive)
			{
				if (e.KeyCode == _toggleCamKey)
					_heliCtrl.toggleHeliCam();
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
