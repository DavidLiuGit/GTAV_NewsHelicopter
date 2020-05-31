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
		private static Random rng = new Random();

		private static bool showingStatic = false;
		#endregion



		#region constants
		public static string defaultTitleText = "Live Breaking News";
		public static string[] subtitleText = new string[] {
			"Local cows lose jobs as milk price drops",
			"Safety meeting ends after 5 injured",
			"China admits to using ocean to hide submarines",
			"Drugs win War on Drugs",
			"Prisoner expected to be raped more often",
			"Pope forgives molested children",
			"Gay teen worried he might be Christian",
			"Poll: majority of Americans approve of sending Congress to Syria",
		};
		#endregion



		static CameraControl()
		{
			_newsScaleform = new Scaleform("breaking_news");
		}



		#region overlay
		public static void enableBreakingNewsOverlay(bool enable = true)
		{
			if (enable)
			{
				_newsScaleform.Render2D();
			}
		}


		public static void updateNewsText(string title, string subtitle)
		{
			if (title == null || title.Trim() == "")
				title = defaultTitleText;
			if (subtitle == null || subtitle.Trim() == "")
				subtitle = subtitleText[rng.Next(0, subtitleText.Length)];

			_newsScaleform.CallFunction("SET_TEXT", title, subtitle);
		}


		public static void showStatic(int type = 1) {
			if (type == -1)
			{
				showingStatic = false;
				_newsScaleform.CallFunction("SHOW_STATIC", -1);
			}

			else if (!showingStatic)
			{
				showingStatic = true;
				_newsScaleform.CallFunction("SHOW_STATIC", type);
			}

		}
		#endregion
	}
}
	