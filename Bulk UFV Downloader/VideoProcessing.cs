using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Model;

namespace Bulk_UFV_Downloader
{
    class VideoProcessing
    {
        //input location:
        private string path;
        private List<string> names = new List<string>();
        private string tempPath;
        public VideoProcessing(string path,string tempPath, List<string> inputNames)
        {
            this.path = path;
            this.tempPath = tempPath;
            foreach (string name in inputNames)
            {
                names.Add(path + name);
            }
        }
        public void Concatnate(string output)
        {
            string command = CommandBuilder(output);
            ExecuteCommand(command);
        }
        private string CommandBuilder(string output)
        {
            StringBuilder command = new StringBuilder("ffmpeg -f concat -safe 0 -i " + VideoListMaker() + " " + output);
            return command.ToString();
        }
        private void ExecuteCommand(object command)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                
                procStartInfo.CreateNoWindow = true;
                
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                
                string result = proc.StandardOutput.ReadToEnd();
                
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                Console.WriteLine(objException);
            }
        }
        private string VideoListMaker()
        {
            string path = tempPath + @"VideoList.txt";
            using (StreamWriter file = File.CreateText(path))
            {
                file.WriteLine("# Video File List:");
                foreach(string s in names)
                {
                    file.WriteLine(@"file '" + s + "'");
                }
            }
            return path;
        }

        //for testing purposes:
        public void Concatnate(string output, Boolean test)
        {
            IConversionResult result = Conversion.Concatenate(output, names.ToArray()).Result;
            Console.WriteLine(result.Arguments);
            //Console.WriteLine(result.MediaInfo);
        }
    }
}
