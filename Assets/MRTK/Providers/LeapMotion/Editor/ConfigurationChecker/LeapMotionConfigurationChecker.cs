// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace Microsoft.MixedReality.Toolkit.LeapMotion
{
    /// <summary>
    /// Class to perform checks for configuration checks for the UnityAR provider.
    /// </summary>
    [InitializeOnLoad]
    static class LeapMotionConfigurationChecker
    {
        private const string FileName = "LeapXRServiceProviderEditor.cs";
        private static readonly string[] definitions = { "LEAPMOTIONCORE_PRESENT" };

        static LeapMotionConfigurationChecker()
        {
            // Check if leap core is in the project
            bool isLeapInProj = ReconcileLeapMotionDefine();
            Debug.Log("Leap in Pro: " + isLeapInProj);

            if (isLeapInProj)
            {
                RemoveTestingFolders();
                AddAndUpdateAsmDef();
            }
           
        }


        private static void RemoveTestingFolders()
        {
            string[] pathsToDelete = new string[]
            {
                "LeapMotion/Core/Editor/Tests/",
                "LeapMotion/Core/Scripts/Algorithms/Editor/Tests/",
                "LeapMotion/Core/Scripts/DataStructures/Editor/Tests/",
                "LeapMotion/Core/Scripts/Encoding/Editor/",
                "LeapMotion/Core/Query/Editor/",
                "LeapMotion/Core/Scripts/Utils/Editor/BitConverterNonAllocTests.cs",
                "LeapMotion/Core/Scripts/Utils/Editor/ListAndArrayExtensionTests.cs",
                "LeapMotion/Core/Scripts/Utils/Editor/TransformUtilTests.cs",
                "LeapMotion/Core/Scripts/Utils/Editor/UtilsTests.cs",
                "LeapMotion/Core/Plugins/LeapCSharp/Editor/Tests/",
                "LeapMotion/Core/Scripts/Query/Editor/"
            };

            // What if leap is not imported to the root of assets?
            foreach(string path in pathsToDelete)
            {
                string fullPath = Path.Combine(Application.dataPath, path);

                // if we are deleting a specific file, then we also need to remove the meta associated with the file
                if (File.Exists(fullPath) && fullPath.Contains(".cs"))
                {
                    FileUtil.DeleteFileOrDirectory(fullPath);
                    // Also delete the meta files
                    FileUtil.DeleteFileOrDirectory(fullPath + ".meta");

                }

                if(Directory.Exists(fullPath) && !fullPath.Contains(".cs"))
                {
                    FileUtil.DeleteFileOrDirectory(fullPath);
                    FileUtil.DeleteFileOrDirectory(fullPath.TrimEnd('/') + ".meta");

                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the AR Foundation package.
        /// </summary>
        /// <returns>True if the define was added, false otherwise.</returns>
        private static bool ReconcileLeapMotionDefine()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(FileName);
            // If the leap asmdef is not in the assets then the core is not in assets
            if (files.Length > 0)
            {
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Standalone, definitions);
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.WSA, definitions);

                return true;
            }
            else
            {
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Standalone, definitions);
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.WSA, definitions);
                return false;
                // if leap is in there an then removed, we need to remove the references from the provider
            }
        }

        private static void AddAndUpdateAsmDef()
        {
            string leapPath = "C:/MRTK/MixedRealityToolkit-Unity/Assets/LeapMotion/LeapMotion.asmdef"; 

            if (!File.Exists(leapPath))
            {
                // File chosen in the leap sdk as an identifier that leap is in the project
                const string leapFileIdentifier = "LeapXRServiceProviderEditor.cs";
                FileInfo[] leapFiles = FileUtilities.FindFilesInAssets(leapFileIdentifier);

                if (leapFiles.Length == 0)
                {
                    Debug.LogWarning($"Unable to locate file: {leapFileIdentifier}");
                    return;
                }

                if (leapFiles.Length == 1)
                {
                    Debug.Log("File has been found and leap is in the project");
                }

                if (leapFiles.Length > 1)
                {
                    Debug.LogWarning($"Multiple ({leapFiles.Length}) {leapFileIdentifier} instances found. Modifying only the first.");
                }

                // Create a new asmdef if leap is in the project

                //string leapAsmDefPlacementPath = Path.Combine(Application.dataPath, "LeapMotion");



                // Get an existing asmdef to copy
                string asmdefToCopy = "C:/MRTK/MixedRealityToolkit-Unity/Assets/MRTK/Core/Microsoft.MixedReality.Toolkit.asmdef";
                //FileStream leapfile = File.Create(leapPath);

                File.Copy(asmdefToCopy, leapPath);

                //AssemblyDefinition leapAsmdef = new AssemblyDefinition();
                //leapAsmdef.Name = "leap";
                //leapAsmdef.IncludePlatforms = new string[] { "Editor", "WSA" };
                AssemblyDefinition asm = AssemblyDefinition.Load(leapPath);
                asm.Name = "LeapMotion";
                asm.References = new string[] { };
                asm.IncludePlatforms = new string[] { "Editor", "WSA" };
                asm.Save(leapPath);

                
                // leap motion provider asmde
                FileInfo[] leapproviderasmdef = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef");

                AssemblyDefinition leapprovideram = AssemblyDefinition.Load(leapproviderasmdef[0].FullName);
                leapprovideram.References = new string[] { "Microsoft.MixedReality.Toolkit",
            "Microsoft.MixedReality.Toolkit.Editor.Inspectors",
            "Microsoft.MixedReality.Toolkit.Editor.Utilities",
            "LeapMotion"};
                leapprovideram.Save(leapproviderasmdef[0].FullName);
                AssetDatabase.Refresh();
            }


        }
    }
}

