using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace CreateHTMLpicFolder
{
    class FolderWithFileAndFolder
    {
        string MyFolderName = "FOLDER_NAME";
        string MyFolderPath = "FOLDER_PATH";
        int MyFolderLevel = 0;
        string LEVEL_MARK = "** ";
        string STEP_MARK = "-- ";

        private void WriteToConsole(int step, string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int counti = 0; counti < MyFolderLevel; counti++)
            {
                sb.Append(LEVEL_MARK);
            }
            for (int counti = 0; counti < step; counti++)
            {
                sb.Append(STEP_MARK);
            }

            Console.WriteLine(sb.ToString() + str);
        }


        public FolderWithFileAndFolder SetFolderName(string name)
        {
            this.MyFolderName = name;
            return this;
        }
        public FolderWithFileAndFolder SetFolderPath(string path)
        {
            this.MyFolderPath = path;
            this.MyFolderName = Path.GetFileName(path);
            return this;
        }

        List<FolderWithFileAndFolder> FolderList = new List<FolderWithFileAndFolder>();
        List<string> FileList = new List<string>();
        List<string> ContentList = new List<string>();

        public static FolderWithFileAndFolder GetNewFolder()
        {
            return new FolderWithFileAndFolder();
        }

        public string GetFolderName()
        {
            return MyFolderName;
        }
        public string GetFolderPath()
        {
            return MyFolderPath;
        }

        public void GetFoldersUnder()
        {
            FolderList.Clear();
            var folder_name_list = Directory.GetDirectories(MyFolderPath).ToList<string>();
            foreach (string folder_name in folder_name_list)
            {
                var folder = FolderWithFileAndFolder.GetNewFolder().SetFolderPath(folder_name);
                FolderList.Add(folder);
                WriteToConsole(4, "New folder: " + folder.GetFolderPath());
            }
        }


        public void GetFilesUnder()
        {
            FileList.Clear();
            List<String> file_path_list = Directory.GetFiles(MyFolderPath).ToList<string>();
            foreach (string file_path in file_path_list)
            {
                var file_name = Path.GetFileName(file_path);
                FileList.Add(file_name);
                WriteToConsole(1, "New file: " + file_name);
            }
        }
        public void GetContentsFromFileList()
        {
            List<string> jpg_list;
            jpg_list = ExtractJPGList();
            foreach (string jpg in jpg_list)
            {
                if (FileList.Contains(jpg))
                {
                    ContentList.Add(jpg);
                    WriteToConsole(3, "New Content: " + jpg);
                }
            }
        }


        const string HTML = ".html";

        string ParentFolderName = null;
        const string INDEX = "index";
        public void SearchAllContentUnder(int level, string parent_name)
        {
            if (level == 1)
            {
                ParentFolderName = INDEX;
            }
            else
            {
                ParentFolderName = parent_name;
            }
            this.MyFolderLevel = level;
            GetAllContentSelf();
            GetFoldersUnder();
            foreach (FolderWithFileAndFolder folder in FolderList)
            {
                try
                {
                    folder.SearchAllContentUnder(this.MyFolderLevel + 1, MyFolderName);
                } catch (Exception ex)
                {
                    WriteToConsole(0, ex.ToString());
                }
            }
            String root_name = null;
            if (MyFolderLevel == 0)
            {
                root_name = INDEX;
            }
            WriteFoldersLinkToHTML(root_name);
            WriteContentsToHTML(root_name);
        }

        public void WriteContentsToHTML(string name)
        {
            name = DecideNameForHTML(name);


            using (StreamWriter output_file = new StreamWriter(name, true))
            {
                int content_index = 0;
                output_file.WriteLine("****************<br>Content Num = " + ContentList.Count.ToString() + "<br>****************<br>");
                output_file.WriteLine("<table border = \"5\">");


                foreach (string content in ContentList)
                {
                    WriteImg(output_file, content_index, content);
                    content_index++;
                }
                if (content_index % MAX_COLUMN != 0)
                {
                    output_file.WriteLine("\t</tr>");
                }
                output_file.WriteLine("</table>");
                if (MyFolderLevel != 0)
                {
                    output_file.WriteLine("<a href=\"../" + ParentFolderName + ".html\">back</a><br>");
                }
            }
        }

        const int MAX_COLUMN = 3;
        private void WriteImg(StreamWriter output_file, int content_index, string content)
        {
            StringBuilder sb = new StringBuilder();

            if (content_index % MAX_COLUMN == 0)
            {
                sb.Append("\t<tr>");
                sb.Append("\n");
            }
            sb.Append("\t\t<td> <img src='");
            var content_name_for_img = content;
            if(content_name_for_img.Contains('\''))
            {
                content_name_for_img = content_name_for_img.Replace("'", "&#39;");
                WriteToConsole(5, content + "->" + content_name_for_img);
            }
            sb.Append(content_name_for_img);
            sb.Append(".jpg' height='384' width='364'></img> <br> <a href=\"");
            sb.Append(content);
            sb.Append("\">CLICK</a> </td>");

            if (content_index % MAX_COLUMN == (MAX_COLUMN - 1))
            {
                sb.Append("\n\t</tr>");
            }
            output_file.WriteLine(sb.ToString());
        }

        private string DecideNameForHTML(string name)
        {
            if (name == null)
            {
                name = MyFolderPath + "\\" + MyFolderName + HTML;
            }
            else
            {
                name = MyFolderPath + "\\" + name + HTML;
            }

            return name;
        }

        public void WriteFoldersLinkToHTML(string name)
        {
            name = DecideNameForHTML(name);
            using (StreamWriter output_file = new StreamWriter(name, false))
            {
                output_file.WriteLine("<head>\n<meta charset = \"UTF-8\"> </head>");

                if (MyFolderLevel != 0)
                {
                    output_file.WriteLine("<a href=\"../" + ParentFolderName + ".html\">back</a><br>");
                }
                foreach (FolderWithFileAndFolder folder in FolderList)
                {
                    WriteWithAHref(output_file, folder.GetFolderName());
                }
            }
        }

        private void WriteWithAHref(StreamWriter output_file, string str)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<a href =\"");
            sb.Append(str);
            sb.Append("/");
            sb.Append(str);
            sb.Append(".html");
            sb.Append("\">");
            sb.Append(str);
            sb.Append("</a>");
            sb.Append("<br>");
            output_file.WriteLine(sb.ToString());
        }



        private void GetAllContentSelf()
        {
            GetFilesUnder();
            GetContentsFromFileList();
        }

        private List<string> ExtractJPGList()
        {
            List<string> jpg_list = new List<string>();
            foreach (string file_name in FileList)
            {
                var file_name_length_without3 = file_name.Length - 4;
                var jpg_index = file_name_length_without3;
                if (jpg_index >= 0)
                {
                    var filename3 = file_name.Substring(jpg_index, 4);
                    if (filename3 == ".jpg")
                    {
                        var filename8 = file_name.Substring(0, file_name_length_without3);
                        if (!jpg_list.Contains(filename8))
                        {
                            jpg_list.Add(filename8);
                            WriteToConsole(2, "New JPG: " + filename8);
                        }
                    }
                }
            }
            return jpg_list;
        }
    }
}
