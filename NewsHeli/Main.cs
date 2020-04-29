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
			Interval = 1;
		}


		private void onTick(object sender, EventArgs e)
		{
			if (firstTime) // if this is the users first time loading the mod, this information will appear
			{
				GTA.UI.Notification.Show(ModName + " by " + Developer + " Loaded");
				firstTime = false;
				_ss = base.Settings;
			}



		}


		private void onKeyDown(object sender, KeyEventArgs e)
		{

		}


		private ScriptSettings _ss;
		private HeliController _heliCtrl;

	}
}
