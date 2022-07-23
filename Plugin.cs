﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.IL2CPP;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;

namespace RF5NoHat {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin : BasePlugin {

		internal static new ManualLogSource Log;

		public static ConfigEntry<bool> eShowCrown;
		public static ConfigEntry<bool> eShowFullface;
		public static ConfigEntry<bool> eShowGlasses;
		public static ConfigEntry<bool> eShowHairpin;
		public static ConfigEntry<bool> eShowHat;
		public static ConfigEntry<bool> eShowHeadband;
		public static ConfigEntry<bool> eShowRibbon;
		public static ConfigEntry<bool> eShowShield;

		public static ConfigEntry<bool> oShowCap;
		public static ConfigEntry<bool> oShowHat;
		public static ConfigEntry<bool> oShowTophair;

		public static List<HumanAttachIDEnum> equipmentBlacklist;
		public static List<HumanJointIDEnum> outfitBlacklist;

		public override void Load() {
			// Plugin startup logic

			Plugin.Log = base.Log;
			Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			eShowCrown = Config.Bind("Equipment Parts", "ShowCrown", false, "Set true to show crown equipments");
			eShowFullface = Config.Bind("Equipment Parts", "ShowFullface", false, "Set true to show fullface equipments");
			eShowGlasses = Config.Bind("Equipment Parts", "ShowGlasses", false, "Set true to show glasses equipments");
			eShowHairpin = Config.Bind("Equipment Parts", "ShowHairpin", false, "Set true to show hairpin equipments");
			eShowHat = Config.Bind("Equipment Parts", "ShowHat", false, "Set true to show hat equipments");
			eShowHeadband = Config.Bind("Equipment Parts", "ShowHeadband", false, "Set true to show headband equipments");
			eShowRibbon = Config.Bind("Equipment Parts", "ShowRibbon", false, "Set true to show ribbon equipments");
			eShowShield = Config.Bind("Equipment Parts", "ShowShield", false, "Set true to show shield equipments");

			oShowCap = Config.Bind("Outfit/Costume Parts (NOT ALL HIDDEN)", "ShowCapOutfit", true, "Set false to hide cap from outfits");
			oShowHat = Config.Bind("Outfit/Costume Parts (NOT ALL HIDDEN)", "ShowHatOutfit", true, "Set false to hide hat from outfits");
			oShowTophair = Config.Bind("Outfit/Costume Parts (NOT ALL HIDDEN)", "ShowTophairOutfit", true, "Set false to hide cowlick from hairs (BEWARE OF BIG GAPING HOLE THERE)");

			equipmentBlacklist = new List<HumanAttachIDEnum>();

			if (!eShowCrown.Value) equipmentBlacklist.Add(HumanAttachIDEnum.Crown);
			if (!eShowFullface.Value) equipmentBlacklist.Add(HumanAttachIDEnum.Fullface);
			if (!eShowGlasses.Value) equipmentBlacklist.Add(HumanAttachIDEnum.Glasses);
			if (!eShowHairpin.Value) equipmentBlacklist.Add(HumanAttachIDEnum.Hairpin);
			if (!eShowHat.Value) equipmentBlacklist.Add(HumanAttachIDEnum.Hat);
			if (!eShowHeadband.Value) equipmentBlacklist.Add(HumanAttachIDEnum.Headband);
			if (!eShowRibbon.Value) equipmentBlacklist.Add(HumanAttachIDEnum.Shield);

			outfitBlacklist = new List<HumanJointIDEnum>();

			if (!oShowCap.Value) outfitBlacklist.Add(HumanJointIDEnum.cap);
			if (!oShowHat.Value) outfitBlacklist.Add(HumanJointIDEnum.hat);
			if (!oShowTophair.Value) outfitBlacklist.Add(HumanJointIDEnum.TopHair01);

			Harmony.CreateAndPatchAll(typeof(RF5NoHat));
		}

		[HarmonyPatch]
		public class RF5NoHat {
			[HarmonyPatch(typeof(HumanModelSocket), nameof(HumanModelSocket.Attach))]
			[HarmonyPrefix]
			public static void HideHat(HumanModelSocket __instance) {
				var attachId = __instance.AttachId;
				equipmentBlacklist.ForEach(x => { if (attachId == x) { __instance.IsVisible = false; Log.LogInfo($"{x} hidden"); } });
			}

			[HarmonyPatch(typeof(HumanModelSocket), nameof(HumanModelSocket.Attach))]
			[HarmonyPrefix]
			public static void HideHatCostume(UnhollowerBaseLib.Il2CppReferenceArray<HumanJoint> joints) {
				foreach (var joint in joints) {
					outfitBlacklist.ForEach(x => { if (joint.JointId == x) { joint.Active = false; Log.LogInfo($"{x} hidden"); } });
				}
			}

			[HarmonyPatch(typeof(HumanModel), nameof(HumanModel.Update))]
			[HarmonyPostfix]
			public static void HideOutfitatStart(HumanModel __instance) {
				outfitBlacklist.ForEach(x => { if (__instance.IsJointVisible(x)) { __instance.JointVisible(x, false); Log.LogInfo($"{x} hidden"); } });
			}
		}
	}
}
