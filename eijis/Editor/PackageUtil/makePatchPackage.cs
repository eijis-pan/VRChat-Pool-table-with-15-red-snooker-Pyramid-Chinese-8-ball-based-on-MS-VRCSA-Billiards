using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace EijisMsVrcsaTableUtil
{
	public class Package
	{
		private static readonly string exportPackageFilePath = "CBCTable1391_rotation20250805.unityPackage";
		static readonly string[] exportFilePaths = 
		{
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/TestPoolSence_rotation.unity",
			
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Cheese/Translate/TextJson/en.Json",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Cheese/Translate/TextJson/ja.Json",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Cheese/Translate/TextJson/zh.Json",
			
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/ht8b_materials/tdesktop_stuff_call_safety.png",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/ht8b_materials/tdesktop_stuff_safety_called.png",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/CallShotOrb_Blue.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/CallShotOrb_Gray.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/CallShotOrb_Orange.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/CallShotOrb_White.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/DesktopAssets_safety.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/DesktopAssets_safetycalled.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/InfoBoard_Rotation_EN.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/InfoBoard_Rotation_JP.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/PushOut.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/PushOutDoing.mat",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Materials/SkipTurnButton.mat",
			
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Textures/GameModesUI4Extra.png",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Textures/InfoBoard_rotation_en.png",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/Textures/InfoBoard_rotation_jp.png",

			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/AdvancedPhysicsManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/BilliardsModule.cs",
			
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCallSafety.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCallSafety.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCallShotLock.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCallShotLock.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCueBallInKitchen.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonCueBallInKitchen.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonNextBallOnSpot.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonNextBallOnSpot.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonPushOut.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonPushOut.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonRequestBreak.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonRequestBreak.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonSkipTurn.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/ButtonSkipTurn.cs",

			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/DesktopManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/GraphicsManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/LegacyPhysicsManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/MenuManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/NetworkingManager.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Modules/BilliardsModule/UdonScripts/StandardPhysicsManager.cs",
			
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Prefab/InfoBoard_Rotation_EN.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Prefab/InfoBoard_Rotation_JP.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Prefab/snooker&pyramid&cn8&3c&10b.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/Prefab/snooker&pyramid&cn8&3c&10b_rotation.prefab",

			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Fbx/Billiards/woodframerack.fbx",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Fbx/【Free】ビリヤードVer.1.0【3Dモデル】（木製ラックフレーム改変） booth_items5248046.txt",
			
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModule/intl.menu/MenuAnchor/GoalPointMenu.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/BilliardsModule/intl.menu/MenuAnchor/SkillLevelMenu.prefab",
			
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreen/PlayerRow.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreen/PointCell.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreen/ScoreScreen.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreen/ScoreScreenRotation.prefab",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/Prefab/ScoreScreen/TeamPlayers.prefab",

			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/BilliardsScoreScreen.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/BilliardsScoreScreen.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/PlayerRow.asset",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/PlayerRow.cs",
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/TeamPlayers.asset", 
			"Assets/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards/eijis/UdonScripts/ScoreScreen/TeamPlayers.cs"
		};
		
		[MenuItem("MS-VRCSA/eijis fork util/ExportPatchPackage_rotation20250805", false, Int32.MaxValue)]
		private static void ExportPackage_Menu(MenuCommand command)
		{
			try
			{
				Debug.Log("ExportPackage");

				var sb = new StringBuilder();
				foreach (var exportFilePath in exportFilePaths)
				{
					if (!File.Exists(exportFilePath))
					{
						sb.AppendLine(exportFilePath);
						Debug.LogWarning("ファイルが見つかりません。 " + exportFilePath);
					}
				}

				bool cancel = false;
				if (0 < sb.Length)
				{
					cancel = EditorUtility.DisplayDialog("Custom Script Warning",
						"Export file(s) nod found.\n" + sb.ToString(), "Ignore", "Cancel");
				}

				if (!cancel)
				{
					AssetDatabase.ExportPackage(exportFilePaths, exportPackageFilePath, ExportPackageOptions.Default);
				}

				EditorUtility.DisplayDialog ("Custom Script Result", "ExportPackage end", "OK");
			}
			catch (Exception ex)
			{
				EditorUtility.DisplayDialog ("Custom Script Exception", ex.ToString(), "OK");
			}
		}
	}
}
