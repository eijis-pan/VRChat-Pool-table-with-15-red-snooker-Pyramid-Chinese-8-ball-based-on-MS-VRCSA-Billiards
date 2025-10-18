//#define MNBK_BACKOUT_PATCH

using System;
using UnityEngine;
using UnityEditor;

namespace EijisMsVrcsaTableUtil
{
	public class Package
	{
		private static readonly string exportPackageFilePath = "CBCTable1393_mnbk_307.unityPackage";
		static readonly string[] exportFilePaths = 
		{
			//"Assets/eijis/Editor/MsVrcsaTableUtil/MNBK_table_setup.cs",
			//"Assets/eijis/Editor/MsVrcsaTableUtil/MNBK_table_backout.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Materials/BilliardsModuleMnbk/DesktopAssets_mnbk.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Materials/BilliardsModuleMnbk/DesktopAssets_paused.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Materials/BilliardsModuleMnbk/DesktopAssets_safetycalled.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Materials/BilliardsModuleMnbk/SkipTurnButton.mat",
			// "Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/MS-VRCSA_Table_mnbk.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.controls.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.desktop/desktop/desktop_mnbk.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.desktop/desktop/desktop_paused.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.desktop/desktop/desktop_safetycalled.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.menu/MenuAnchor/SkillLevelMenu.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.menu/MenuAnchor/LobbyMenu/GameMode/GameModeMnbk.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.menu/MenuAnchor/LobbyMenu/GameMode/Buttons/Mode9BallUsa.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.menu/MenuAnchor/LobbyMenu/GameMode/Buttons/Mode9BallMnbk.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.menu/MenuAnchor/LobbyMenu/GameMode/SelectionPoints/9ballUsa.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/intl.menu/MenuAnchor/LobbyMenu/GameMode/SelectionPoints/9ballMnbk.prefab",
			/*
			"Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/BilliardsModule/intl.controls.meta",
			"Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/BilliardsModule/intl.desktop/desktop/desktop_mnbk.meta",
			"Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/BilliardsModule/intl.desktop/desktop/desktop_paused.meta",
			"Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/BilliardsModule/intl.desktop/desktop/desktop_safetycalled.meta",
			"Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/BilliardsModule/intl.menu/MenuAnchor/SkillLevelMenu.meta",
			"Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/BilliardsModule/intl.menu/MenuAnchor/LobbyMenu/GameMode/GameModeMnbk.meta",
			"Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/BilliardsModule/intl.menu/MenuAnchor/LobbyMenu/GameMode/Buttons/Mode9BallUsa.meta",
			"Assets/eijis/Prefab/MS-VRCSA-Billiards_mnbk/BilliardsModule/intl.menu/MenuAnchor/LobbyMenu/GameMode/Buttons/Mode9BallMnbk.meta",
			*/
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreen/PointCell.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreenMnbk/PlayerRow.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreenMnbk/ScoreScreenMnbk.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreenMnbk/ScoreScreenMnbkMBC.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreenMnbk/TeamPlayers.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/snooker&pyramid&cn8&3c&10b&mnbk.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/snooker&pyramid&cn8&3c&10b&mnbkMBC.prefab",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModuleMnbk/snooker&pyramid&cn8&3c&10b&mnbkMBC.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Textures/BilliardsModuleMnbk/GameModesUI_Mnbk.png",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Textures/BilliardsModuleMnbk/tdesktop_stuff_call_safety.png",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Textures/BilliardsModuleMnbk/tdesktop_stuff_paused.png",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Textures/BilliardsModuleMnbk/tdesktop_stuff_safety_called.png",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Cheese/Elo/RankingSystem.cs", // 1.393
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Cheese/TableHook/SettingLoader.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Cheese/Translate/TextJson/en.Json",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Cheese/Translate/TextJson/ja.Json",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Cheese/Translate/TextJson/zh.Json",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Prefab/old/snooker&pyramid&cn8&3c.prefab", // 1.393
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Prefab/snooker&pyramid&cn8&3c&10b.prefab", // include 1.391 to 1.393
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/ht8b_materials/procedural/tballs_3Cushion_YellowCueBall.png", // 1.393
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/SkipTurnButton.mat",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/PocketMarker_Blue.mat",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/PocketMarker_Gray.mat",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/PocketMarker_Orange.mat",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/PocketMarker_White.mat",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/UI_Additive_CushionTouch.mat",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/UI_Additive_CushionTouch2.mat",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Textures/UI_Additive.png",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Shaders/TransparentOrb_Quest.shader",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/AdvancedPhysicsManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/AdvancedPhysicsManager_V05M.asset", // for upgrade
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/AdvancedPhysicsManager_V05M.cs", // for upgrade
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/BilliardsModule.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCallSafety.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCallSafety.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonPause.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonPause.cs",
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/CueController.cs", // 1.393
			// "Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ColorDownload.cs", // 1.393
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/DesktopManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/GraphicsManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/LegacyPhysicsManager.cs", // for upgrade
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/MenuManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/NetworkingManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/StandardPhysicsManager.cs", // for upgrade
			// "Assets/TsFolder/CountButton/ScoreBoard_1.prefab",
			// "Assets/UDON Counter_UdonProgramSources/ScoreBoard Udon C# Program Asset.asset",
			// "Assets/UDON Counter_UdonProgramSources/ScoreBoard.cs",
			// "Assets/UDON Counter_UdonProgramSources/ValueText.cs"
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/BilliardsScoreScreen.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/BilliardsScoreScreen.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/PlayerRow.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/PlayerRow.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/TeamPlayers.asset", 
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/TeamPlayers.cs"
		};
		
		[MenuItem("GameObject/MNBK/ExportPackage307", false, 0)]
		private static void ExportPackage_Menu(MenuCommand command)
		{
			try
			{
				Debug.Log("ExportPackage");

				AssetDatabase.ExportPackage(exportFilePaths, exportPackageFilePath, ExportPackageOptions.Default);

				EditorUtility.DisplayDialog ("Custom Script Result", "ExportPackage end", "OK");
			}
			catch (Exception ex)
			{
				EditorUtility.DisplayDialog ("Custom Script Exception", ex.ToString(), "OK");
			}
		}

		private static readonly string exportPackageFilePath_backout = "CBC_mnbk_backout.unityPackage";
		static readonly string[] exportFilePaths_backout = 
		{
			"Assets/eijis/Editor/MsVrcsaTableUtil/MNBK_table_backout.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/AdvancedPhysicsManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/BilliardsModule.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCallSafety.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonPause.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/CueController.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/DesktopManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/GraphicsManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/MenuManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/NetworkingManager.cs",
		};
		
		[MenuItem("GameObject/MNBK/ExportPackage_backout", false, 3)]
		private static void ExportPackage_backout_Menu(MenuCommand command)
		{
			try
			{
				Debug.Log("ExportPackage(backout)");

				AssetDatabase.ExportPackage(exportFilePaths_backout, exportPackageFilePath_backout, ExportPackageOptions.Default);

				EditorUtility.DisplayDialog ("Custom Script Result", "ExportPackage(backout) end", "OK");
			}
			catch (Exception ex)
			{
				EditorUtility.DisplayDialog ("Custom Script Exception", ex.ToString(), "OK");
			}
		}
	}
}
