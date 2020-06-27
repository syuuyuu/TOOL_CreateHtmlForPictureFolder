using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CreateHTMLpicFolder
{
    internal class FolderWithFileAndFolder
    {
        public static int ParaMaxColumn = 4;
        public static int ParaPicHeight = 364;
        public static int ParaPicWidth = 384;
        public static string ParaBackgroundColor = "000000";
        private const string STR_HTML = ".html";
        private const string STR_INDEX = "index";
        private const string STR_LEVEL_MARK = "** ";
        private const string STR_STEP_MARK = "-- ";
        private List<string> ContentList = new List<string>();
        private List<string> FileList = new List<string>();
        private List<FolderWithFileAndFolder> FolderList = new List<FolderWithFileAndFolder>();
        private int MyFolderLevel = 0;
        private string MyFolderName = "FOLDER_NAME";
        private string MyFolderPath = "FOLDER_PATH";
        private string ParentFolderName = null;
        private int StepLevel = 0;

        public static FolderWithFileAndFolder GetNewInstance()
        {
            return new FolderWithFileAndFolder();
        }

        public void GetContentsFromFileList()
        {
            StepLevel++;
            List<string> jpg_list;
            jpg_list = ExtractJPGList();
            foreach (string jpg in jpg_list)
            {
                if (FileList.Contains(jpg))
                {
                    ContentList.Add(jpg);
                    ShowToConsole(StepLevel, "New Content: " + jpg);
                }
            }
        }

        public string GetFolderName()
        {
            return MyFolderName;
        }

        public string GetFolderPath()
        {
            return MyFolderPath;
        }

        public void SearchAll(int level, string parent_name)
        {
            StepLevel = 0;
            if (level == 1)
            {
                ParentFolderName = STR_INDEX;
            }
            else
            {
                ParentFolderName = parent_name;
            }
            this.MyFolderLevel = level;
            SearchContentSelf();
            SearchFoldersUnder();
            foreach (FolderWithFileAndFolder folder in FolderList)
            {
                try
                {
                    folder.SearchAll(this.MyFolderLevel + 1, MyFolderName);
                }
                catch (Exception ex)
                {
                    ShowToConsole(StepLevel, ex.ToString());
                }
            }
            String root_name = null;
            if (MyFolderLevel == 0)
            {
                root_name = STR_INDEX;
            }
            WriteFoldersLinkToHTML(root_name);
            WriteContentsToHTML(root_name);
        }

        public void SearchContentSelf()
        {
            SearchFilesUnder();
            GetContentsFromFileList();
        }
        public void SearchFilesUnder()
        {
            StepLevel++;
            FileList.Clear();
            List<String> file_path_list = Directory.GetFiles(MyFolderPath).ToList<string>();
            foreach (string file_path in file_path_list)
            {
                var file_name = Path.GetFileName(file_path);
                FileList.Add(file_name);
                ShowToConsole(StepLevel, "New file: " + file_name);
            }
        }

        public void SearchFoldersUnder()
        {
            StepLevel++;
            FolderList.Clear();
            var folder_name_list = Directory.GetDirectories(MyFolderPath).ToList<string>();
            foreach (string folder_name in folder_name_list)
            {
                var folder = FolderWithFileAndFolder.GetNewInstance().SetFolderPath(folder_name);
                FolderList.Add(folder);
                ShowToConsole(StepLevel, "New folder: " + folder.GetFolderPath());
            }
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

        public void WriteContentsToHTML(string name)
        {
            StepLevel++;
            name = DecideFileNameForHTML(name);

            using (StreamWriter output_file = new StreamWriter(name, true))
            {
                int content_index = 0;
                output_file.WriteLine("****************<br>Content Num = " + ContentList.Count.ToString() + "<br>****************<br>");
                output_file.WriteLine("<table border = \"5\">");

                foreach (string content in ContentList)
                {
                    DoWriteContentsToHTML(output_file, content_index, content);
                    content_index++;
                }
                if (content_index % ParaMaxColumn != 0)
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

        public void WriteFoldersLinkToHTML(string name_input)
        {
            StepLevel++;
            var filename = DecideFileNameForHTML(name_input);
            using (StreamWriter output_file = new StreamWriter(filename, false))
            {
                output_file.WriteLine("<head>\n<meta charset = \"UTF-8\"> </head>");
                output_file.WriteLine("<body style = \"background-color:#" + ParaBackgroundColor + ";\" >\n</body>");
                if (MyFolderLevel != 0)
                {
                    output_file.WriteLine("<a href=\"../" + ParentFolderName + ".html\">back</a><br>");
                }
                foreach (FolderWithFileAndFolder folder in FolderList)
                {
                    DoWriteFoldersLinkToHTML(output_file, folder.GetFolderName());
                }
            }
        }

        private string DecideFileNameForHTML(string name_input)
        {
            var filename = "";
            if (name_input == null)
            {
                filename = MyFolderPath + "\\" + MyFolderName + STR_HTML;
            }
            else
            {
                filename = MyFolderPath + "\\" + name_input + STR_HTML;
            }

            return filename;
        }

        private void DoWriteContentsToHTML(StreamWriter output_file, int content_index, string content)
        {
            StringBuilder sb = new StringBuilder();

            if (content_index % ParaMaxColumn == 0)
            {
                sb.Append("\t<tr>");
                sb.Append("\n");
            }
            sb.Append("\t\t<td> <img src='");
            var content_newname = content;
            if (content_newname.Contains('#') ||
                content_newname.Contains('$') ||
                content_newname.Contains(';') ||
                content_newname.Contains('!') ||
                content_newname.Contains('"') ||
                content_newname.Contains('~') ||
                content_newname.Contains('%') ||  
                content_newname.Contains('&') ||
                content_newname.Contains('@') ||
                content_newname.Contains('`') 
                )
            {
                content_newname = content_newname.Replace("#", "_");
                content_newname = content_newname.Replace("$", "_");
                content_newname = content_newname.Replace(";", "_");
                content_newname = content_newname.Replace("!", "_");
                content_newname = content_newname.Replace("'", "_");
                content_newname = content_newname.Replace("~", "_");
                content_newname = content_newname.Replace("%", "_");
                content_newname = content_newname.Replace("&", "_");
                content_newname = content_newname.Replace("@", "_");
                content_newname = content_newname.Replace("`", "_");

                System.IO.File.Move(MyFolderPath +"\\" +content, MyFolderPath + "\\"+content_newname);
                System.IO.File.Move(MyFolderPath + "\\" + content +".jpg", MyFolderPath + "\\" + content_newname +".jpg");

                ShowToConsole(StepLevel, content + "->" + content_newname);
            }
            sb.Append(content_newname);
            sb.Append(".jpg' height='" + ParaPicHeight.ToString() + "' width='" + ParaPicWidth.ToString() + "'></img> <br> <a href=\"");
            sb.Append(content_newname);
            sb.Append("\">CLICK</a> </td>");

            if (content_index % ParaMaxColumn == (ParaMaxColumn - 1))
            {
                sb.Append("\n\t</tr>");
            }
            output_file.WriteLine(sb.ToString());
        }

        private void DoWriteFoldersLinkToHTML(StreamWriter output_file, string str)
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
                            ShowToConsole(StepLevel, "New JPG: " + filename8);
                        }
                    }
                }
            }
            return jpg_list;
        }
        private void ShowToConsole(int step, string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int counti = 0; counti < MyFolderLevel; counti++)
            {
                sb.Append(STR_LEVEL_MARK);
            }
            for (int counti = 0; counti < step; counti++)
            {
                sb.Append(STR_STEP_MARK);
            }

            Console.WriteLine(sb.ToString() + str);
        }
    }
}