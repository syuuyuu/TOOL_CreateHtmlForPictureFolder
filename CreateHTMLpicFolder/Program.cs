using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CreateHTMLpicFolder
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var arg_len = args.Length;
            bool dostuff;
            dostuff = ExtractArgPara(args, arg_len);
            if (dostuff)
            {
                DoStuff();
            }
        }

        private static bool ExtractArgPara(string[] args, int arg_len)
        {
            var dostuff = true;
            for (int arg_index = 0; arg_index < arg_len; ++arg_index)
            {
                string arg = args[arg_index];
                if (arg == "-h" || arg == "-?" || arg == "/?" || arg == "/h")
                {
                    dostuff = ShowHelpToConsole();
                    break;
                }
                else if (arg_index + 1 < arg_len)
                {
                    var arg_para = args[arg_index + 1];
                    SetParameter(arg, arg_para);
                }
                else
                {                   
                    //do nothing
                }
            }

            return dostuff;
        }

        private static void SetParameter(string arg, string arg_para)
        {
            if (arg == "-r")
            {
                FolderWithFileAndFolder.ParaBackgroundColor = arg_para;
            }
            else
            {
                try
                {
                    var para = int.Parse(arg_para);
                    if (para > 1)
                    {
                        switch (arg)
                        {
                            case "-col":
                                FolderWithFileAndFolder.ParaMaxColumn = para;
                                break;
                            case "-w":
                                FolderWithFileAndFolder.ParaPicWidth = para;
                                break;
                            case "-v":
                                FolderWithFileAndFolder.ParaPicHeight = para;
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    //                Console.WriteLine(ex.ToString());
                }
            }
        }

        private static bool ShowHelpToConsole()
        {
            bool dostuff;
            WriteAppInfoToConsole();
            Console.WriteLine("-col NN : Max Column Num");
            Console.WriteLine("-w NNN: Pic Width");
            Console.WriteLine("-v NNN : Pic Height");
            Console.WriteLine("-r RRGGBB : Background color");

            dostuff = false;
            return dostuff;
        }

        private static void DoStuff()
        {
            string root_path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            FolderWithFileAndFolder root_folder = FolderWithFileAndFolder.GetNewInstance().SetFolderPath(root_path);
            WriteAppInfoToConsole();
            Console.WriteLine("ROOTPATH: " + root_folder.GetFolderPath());
            Console.WriteLine("ROOTNAME: " + root_folder.GetFolderName());
            //root_folder.GetFoldersUnder();
            //root_folder.GetFilesUnder();
            //root_folder.GetContentsFromFileList();
            //root_folder.WriteFoldersLinkToHTML(null);
            //root_folder.WriteContentsToHTML(null);
            root_folder.SearchAll(0, null);
            WriteAppInfoToConsole();
            Console.WriteLine("..... Press Any Key to exit.");
            Console.ReadLine();
        }

        private static void WriteAppInfoToConsole()
        {
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine("\nContent with .jpg preview Searcher v1.0.200624.1");
            Console.WriteLine("                              by syuu 2020\n");
            Console.WriteLine("---------------------------------------------------------");
        }
    }
}
