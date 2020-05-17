using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.UI;
using GTA.Math;


namespace NewsHeli
{
	public class CameraControl
	{
		#region properties
		private static Scaleform _newsScaleform;
		#endregion

		static CameraControl()
		{
			_newsScaleform = new Scaleform("breaking_news");
			_newsScaleform.CallFunction("SHOW_STATIC", 1);
		}


		public static void enableBreakingNewsOverlay(bool enable = true)
		{
			if (enable)
			{
				_newsScaleform.Render2D();
			}
			else
			{
				_newsScaleform.Dispose();
			}
		}
	}
}
