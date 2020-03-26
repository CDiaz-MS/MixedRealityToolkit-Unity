// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LeapMotion
{
    /// <summary>
    /// Class that checks if the Leap Motion Core assets are present and configures the project if they are.
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
                AddAndUpdateAsmDefs();
            }
        }

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the Leap Motion Core Assets.
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

        /// <summary>
        /// The Leap Core Assets currently contain multiple folders with tests in them.  An issue has been filed in the Unity
        /// Modules repo: https://github.com/leapmotion/UnityModules/issues/1097.  The issue with the multiple test folders is when an 
        /// asmdef is placed at the root of the core assets, each folder containing tests needs another separate asmdef.  This method
        /// is used to avoid adding an additional 8 asmdefs to the project, by removing the folders and files that are tests in the 
        /// Leap Core Assets.
        /// </summary>
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

            // If one of the leap test directories exists then the rest have not been deleted
            if (Directory.Exists(Path.Combine(Application.dataPath, pathsToDelete[0])))
            {
                foreach (string path in pathsToDelete)
                {
                    // What if leap is not imported to the root of assets?
                    string fullPath = Path.Combine(Application.dataPath, path);

                    // If we are deleting a specific file, then we also need to remove the meta associated with the file
                    if (File.Exists(fullPath) && fullPath.Contains(".cs"))
                    {
                        // Delete the test files
                        FileUtil.DeleteFileOrDirectory(fullPath);

                        // Also delete the meta files
                        FileUtil.DeleteFileOrDirectory(fullPath + ".meta");
                    }

                    if (Directory.Exists(fullPath) && !fullPath.Contains(".cs"))
                    {
                        // Delete the test directories
                        FileUtil.DeleteFileOrDirectory(fullPath);

                        // Delete the test directories meta files
                        FileUtil.DeleteFileOrDirectory(fullPath.TrimEnd('/') + ".meta");
                    }
                }

                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Adds an asmdef at the root of the LeapMotion Core Assets once they are imported into the project and adds the newly created LeapMotion.asmdef
        /// as a reference for the existing leap data provider asmdef.
        /// </summary>
        private static void AddAndUpdateAsmDefs()
        {
            string leapAsmDefPath = Path.Combine(Application.dataPath, "LeapMotion/LeapMotion.asmdef");
            Debug.Log(leapAsmDefPath);

            // If the asmdef has already been created then do not create another one
            if (!File.Exists(leapAsmDefPath))
            {
                // File chosen in the leap sdk as an identifier that leap is in the project
                const string leapFileIdentifier = "LeapXRServiceProviderEditor.cs";
                FileInfo[] leapFiles = FileUtilities.FindFilesInAssets(leapFileIdentifier);

                // Create the asmdef that will be placed in the Leap Core Assets when they are imported
                // A new asmdef needs to be created in order to reference it in the MRTK/Providers/LeapMotion/Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef file
                AssemblyDefinition leapAsmDef = new AssemblyDefinition();
                leapAsmDef.Name = "LeapMotion";
                leapAsmDef.References = new string[] { };
                leapAsmDef.IncludePlatforms = new string[] { "Editor", "WSA" };
                leapAsmDef.Save(leapAsmDefPath);

                // Get the MRTK/Providers/LeapMotion/Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef
                FileInfo[] leapDataProviderAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef");

                // Add the newly created LeapMotion.asmdef to the refrences of the leap data provider asmdef
                AssemblyDefinition leapDataProviderAsmDef = AssemblyDefinition.Load(leapDataProviderAsmDefFile[0].FullName);

                leapDataProviderAsmDef.References = new string[] 
                { "Microsoft.MixedReality.Toolkit",
                  "Microsoft.MixedReality.Toolkit.Editor.Inspectors",
                  "Microsoft.MixedReality.Toolkit.Editor.Utilities",
                  "LeapMotion"
                };

                leapDataProviderAsmDef.Save(leapDataProviderAsmDefFile[0].FullName);

                // A new asset (LeapMotion.asmdef) was created, refresh the asset database
                AssetDatabase.Refresh();
            }
        }
    }
}

