using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WindowsInput;
using WindowsInput.Native;

namespace GW2AutoLogin {

    class Program {
        private const string PropInstallDir = "installDir";
        private const string PropWaitTime = "waitTime";
        private const string PropAccount = "account";

        public static void Main(string[] args) {

            try {
                Config config = initConfig();

                Process firstProc = new Process();

                firstProc.StartInfo.UseShellExecute = true;
                firstProc.StartInfo.WorkingDirectory = config.installDir;
                firstProc.StartInfo.FileName = "Gw2-64";
                firstProc.EnableRaisingEvents = true;

                foreach (var credentials in config.credentials) {
                    runAutoLogin(firstProc, config, credentials);
                }

            } catch (Exception ex) {
                Console.WriteLine("An error occurred!!!: " + ex.Message);
                return;
            }
        }

        private static void runAutoLogin(Process process, Config config, KeyValuePair<string, string> credentials) {
            process.Start();
            System.Threading.Thread.Sleep(7 * 1000);
            InputSimulator s = new InputSimulator();

            s.Keyboard.TextEntry(credentials.Key);

            s.Keyboard.KeyPress(VirtualKeyCode.TAB);

            s.Keyboard.TextEntry(credentials.Value);

            s.Keyboard.KeyPress(VirtualKeyCode.RETURN);

            // skip auto login timer
            System.Threading.Thread.Sleep(2 * 1000);
            s.Keyboard.KeyPress(VirtualKeyCode.RETURN);

            System.Threading.Thread.Sleep(10 * 1000);
            s.Keyboard.KeyPress(VirtualKeyCode.RETURN);

            System.Threading.Thread.Sleep(10 * 1000);
            s.Keyboard.KeyPress(VirtualKeyCode.RETURN);

            System.Threading.Thread.Sleep(config.waitTime);

            process.Kill();
        }

        private static Config initConfig() {

            string[] lines = System.IO.File.ReadAllLines(Directory.GetCurrentDirectory() + @"\config.txt");
            Config config = new Config();

            for (int i = 0; i < lines.Length; i++) {
                if (lines[i].StartsWith(PropInstallDir)) {
                    config.installDir = lines[i].Substring(PropInstallDir.Length + 1);
                }
                if (lines[i].StartsWith(PropWaitTime)) {
                    config.waitTime = int.Parse(lines[i].Substring(PropWaitTime.Length + 1));
                }

                if (lines[i].StartsWith(PropAccount)) {
                    string[] accInfo = lines[i].Substring(PropAccount.Length + 1).Split(" ");
                    config.credentials.Add(accInfo[0], accInfo[1]);
                }
            }

            return config;
        }
    }

    class Config {
        public string installDir { get; set; }
        public int waitTime { get; set; }

        private IDictionary<string, string> _credentials = new Dictionary<string, string>();
        public IDictionary<string, string> credentials => _credentials;
    }
}
