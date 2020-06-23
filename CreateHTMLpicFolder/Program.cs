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
            string root_path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            FolderWithFileAndFolder root_folder = FolderWithFileAndFolder.GetNewFolder().SetFolderPath(root_path);
            Console.WriteLine("Content with .jpg searcher v0.1b");
            Console.WriteLine("                              by syuu 2020");
            Console.WriteLine("ROOTPATH: "+root_folder.GetFolderPath());
            Console.WriteLine("ROOTNAME: "+root_folder.GetFolderName());
            //root_folder.GetFoldersUnder();
            //root_folder.GetFilesUnder();
            //root_folder.GetContentsFromFileList();
            //root_folder.WriteFoldersLinkToHTML(null);
            //root_folder.WriteContentsToHTML(null);
            root_folder.SearchAllContentUnder(0, null);
            Console.WriteLine("\nContent with .jpg searcher v0.1b");
            Console.WriteLine("                              by syuu 2020");
            Console.ReadLine();
        }
    }
}
