using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MelonLoader
{
    internal static class CommandLine
    {
        internal static bool IsCMD = false;
        internal static bool IsSilent = false;
        internal static int CmdMode = 0;
        internal static string ExePath = null;
        internal static string ZipPath = null;
        internal static string RequestedVersion = null;
        internal static bool AutoDetectArch = true;
        internal static bool Requested32Bit = false;

        internal static string[] __args;

        internal static bool Run(string[] args, ref int returnval)
        {
            __args = args;
            if (args.Length <= 0)
                return false;
            if (string.IsNullOrEmpty(args[0]))
                return false;
            ExePath = string.Copy(args[0]);
            if (args.Length == 1)
                return false;

            return true;
        }

        public static void Install()
        {
            if (!Program.ValidateUnityGamePath(ref ExePath))
            {
                // Output Error
                return;
            }
            Program.GetCurrentInstallVersion(Path.GetDirectoryName(ExePath));
            if (!string.IsNullOrEmpty(ZipPath))
            {
                InstallFromZip();
                return;
            }

            if (Releases.All.Count <= 0)
            {
                // Output Error
                return;
            }

            // Pull Latest Version

            if (Program.CurrentInstalledVersion == null)
                OperationHandler.CurrentOperation = OperationHandler.Operations.INSTALL;
            else
            {
                Version selected_ver = new Version(Releases.Official[0]);
                int compare_ver = selected_ver.CompareTo(Program.CurrentInstalledVersion);
                if (compare_ver < 0)
                    OperationHandler.CurrentOperation = OperationHandler.Operations.DOWNGRADE;
                else if (compare_ver > 0)
                    OperationHandler.CurrentOperation = OperationHandler.Operations.UPDATE;
                else
                    OperationHandler.CurrentOperation = OperationHandler.Operations.REINSTALL;
            }
            OperationHandler.Automated_Install(Path.GetDirectoryName(ExePath), Releases.Official[0], Requested32Bit, (Releases.Official[0].StartsWith("v0.2") || Releases.Official[0].StartsWith("v0.1")));
        }

        public static void InstallFromZip()
        {
            if (!Program.ValidateZipPath(ZipPath))
            {
                // Output Error
                return;
            }
            OperationHandler.ManualZip_Install(ZipPath, Path.GetDirectoryName(ExePath));
        }


        public static void Uninstall()
        {
            if (!Program.ValidateUnityGamePath(ref ExePath))
            {
                // Output Error
                return;
            }
            string folderpath = Path.GetDirectoryName(ExePath);
            Program.GetCurrentInstallVersion(folderpath);
            if (Program.CurrentInstalledVersion == null)
            {
                // Output Error
                return;
            }
            OperationHandler.CurrentOperation = OperationHandler.Operations.UNINSTALL;
            OperationHandler.Uninstall(folderpath);
        }
    }
}
