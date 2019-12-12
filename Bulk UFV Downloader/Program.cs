using System;
using System.Collections.Generic;
using System.IO;
using WinSCP;

namespace Bulk_UFV_Downloader
{
    class Program
    {
        private static List<RemoteFileInfo> DownloadList = new List<RemoteFileInfo>();
        static void Main(string[] args)
        {
            Console.WriteLine("UFV Bulk File Downloader");
            Console.Write("Enter Camera UUID:");
            string UUID = Console.ReadLine();
            Console.WriteLine("Note: Dates are in UTC (7PM EST = 12AM UTC)");
            Console.Write("Enter year:");
            string Year = Console.ReadLine();
            Console.Write("Enter Month:");
            string Month = Console.ReadLine();
            Console.Write("Enter Day:");
            string Day = Console.ReadLine();

            Console.WriteLine("Note: Times are in Local time zone in UNIX format. https://www.epochconverter.com/");
            Console.Write("Start Time:");
            long StartUnix;
            if (long.TryParse(Console.ReadLine(), out long x))
            {
                StartUnix = x;
            }
            else
            {
                Console.WriteLine("Couldn't parse input.");
                return;
            }

            Console.Write("End Time:");
            long EndUnix;
            if (long.TryParse(Console.ReadLine(), out x))
            {
                EndUnix = x;
            }
            else
            {
                Console.WriteLine("Couldn't parse input.");
                return;
            }

            try
            {
                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Scp,
                    HostName = args[0],
                    UserName = args[1],
                    Password = args[2],
                    SshHostKeyFingerprint = args[3] + " " + args[4] + " " + args[5],
                };
                sessionOptions.AddRawSettings("Shell", "sudo%20-s");
                using (Session session = new Session())
                {
                    Console.WriteLine("Connecting...");
                    // Connect
                    session.Open(sessionOptions);

                    Console.WriteLine("Connected.");
                    // Example: /mnt/raid0/recordings/735682ef-c7d2-31c0-b217-5fae42fe52a4/2019/12/07/
                    RemoteDirectoryInfo directory = session.ListDirectory(args[6]+"/"+UUID+"/"+Year+"/"+Month+"/"+Day+"/");
                    long TotalBytes = 0;
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        System.DateTime StartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        System.DateTime EndTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        if (fileInfo.LastWriteTime > StartTime.AddSeconds(StartUnix-2).ToLocalTime()&&fileInfo.LastWriteTime < EndTime.AddSeconds(EndUnix).ToLocalTime()&&fileInfo.FileType=='-')
                        {
                            TotalBytes += fileInfo.Length;
                            DownloadList.Add(fileInfo);
                        }
                    }
                    //Sort list:
                    DownloadList.Sort((z, y) => z.LastWriteTime.CompareTo(y.LastWriteTime));
                    Console.WriteLine("Total of " + DownloadList.Count + " files for a total time of about "+DownloadList.Count/30+" minutes.");
                    Console.WriteLine("Combined file size: " + TotalBytes + " Bytes.");

                    Console.Write("Would you like to continue with the download? [y/n] ");
                    string key = Console.ReadLine();
                    if (key != "y" && key != "Y")
                    {
                        Console.WriteLine("Exiting.");
                        return;
                    }
                    Console.Write("\nEnter Output:");
                    string output = Console.ReadLine();
                    //Downloader:
                    Console.WriteLine("Initializing Downloads...");
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    TransferOperationResult transferResult;
                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    Console.WriteLine("Downloading...");
                    string FolderName = Path.GetTempPath() + @"VideoFiles" + unixTimestamp + @"\";
                    System.IO.Directory.CreateDirectory(FolderName);
                    Console.WriteLine(FolderName);
                    int DownloadCount = 0;

                    //list for splicing:
                    List<string> fileLocations = new List<string>();
                    foreach (RemoteFileInfo fileInfo in DownloadList)
                    {
                        transferResult = session.GetFiles(fileInfo.FullName, FolderName, false, transferOptions);
                        transferResult.Check();
                        fileLocations.Add(fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf('/') + 1));
                        DownloadCount++;
                        Console.Write("\rDownloaded: {0} Files.",DownloadCount);
                    }
                    //Complile videos:
                    VideoProcessing processor = new VideoProcessing(FolderName, FolderName, fileLocations);
                    Console.WriteLine("Splicing files...");
                    processor.Concatnate(output);
                    Console.WriteLine("Complete.");
                }

                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return;
            }
        }
    }
}
