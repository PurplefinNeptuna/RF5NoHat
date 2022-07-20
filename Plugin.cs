using BepInEx;
using BepInEx.Logging;
using BepInEx.IL2CPP;
using UnityEngine;
using HarmonyLib;

namespace RF5NoHat {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin : BasePlugin {

		internal static new ManualLogSource Log;
		public override void Load() {
			// Plugin startup logic

			Plugin.Log = base.Log;
			Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			Harmony.CreateAndPatchAll(typeof(RF5NoHat));
		}

		public enum SynthMode {
			NONE,
			CRAFT,
			UPGRADE
		}

		[HarmonyPatch]
		public class RF5NoHat {
			[HarmonyPatch(typeof(HumanModelSocket), nameof(HumanModelSocket.Attach))]
			[HarmonyPrefix]
			public static void HideHat(HumanModelSocket __instance) {
				var attachId = __instance.AttachId;
				if (attachId is HumanAttachIDEnum.Crown or HumanAttachIDEnum.Fullface or HumanAttachIDEnum.Glasses or HumanAttachIDEnum.Hairpin or HumanAttachIDEnum.Hat or HumanAttachIDEnum.Headband or HumanAttachIDEnum.Ribbon) {
					Log.LogInfo("Hat Missing?");
					//visible = false;
					__instance.IsVisible = false;
				}
			}
		}
	}
}
