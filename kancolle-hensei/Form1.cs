using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace kancolle_hensei
{
    public partial class Form1 : Form
    {
        private Int32[,] henseiPosition = new Int32[,] 
        {
            {0,0}, {488,0},
            {0, 376 * 1}, {488, 376 * 1},
            {0, 376 * 2}, {488, 376 * 2},
            {0, 376 * 3}, {488, 376 * 3},
        };

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ファイルリストをたどります
        /// ディレクトリの場合は再帰的に下降します
        /// </summary>
        /// <param name="FSInfo"></param>
        /// <returns></returns>
        private List<String> ListDirectoriesAndFiles(FileSystemInfo[] FSInfo)
        {
            List<String> fileList = new List<String>();

            if (FSInfo == null)
                return fileList;

            foreach (FileSystemInfo i in FSInfo)
            {
                if (i is DirectoryInfo)
                {
                    DirectoryInfo dInfo = (DirectoryInfo)i;
                    List<String> l = ListDirectoriesAndFiles(dInfo.GetFileSystemInfos());
                    fileList.AddRange(l);
                }
                else if (i is FileInfo)
                {
                    fileList.Add(i.FullName);
                }
            }
            return fileList;
        }

        /// <summary>
        /// ファイルリストを読み込みます
        /// </summary>
        /// <param name="files"></param>
        private List<String> LoadFilePath(String[] files)
        {
            List<String> fileList = new List<String>();
            foreach (String f in files)
            {
                if (Directory.Exists(f))
                {
                    DirectoryInfo dir = new DirectoryInfo(f);
                    FileSystemInfo[] infos = dir.GetFileSystemInfos();
                    List<String> l = ListDirectoriesAndFiles(infos);
                    fileList.AddRange(l);
                }
                else
                {
                    fileList.Add(f);
                }
            }
            return fileList;
        }
        
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int fileCount = files.Count();
            int h = Convert.ToInt32(Math.Ceiling(fileCount / 2.0));
            Bitmap henseiBitmap = new Bitmap(488 * 2, 376 * h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics henseiGraphics = Graphics.FromImage(henseiBitmap);

            List<String> fl = LoadFilePath(files);
            fl.Sort();
            for (int i = 0; i < fileCount; i++)
            {
                Console.WriteLine(fl[i]);
                Bitmap orig = new Bitmap(fl[i].ToString());
                Bitmap copy = orig.Clone(new Rectangle(312, 94, 488, 376), orig.PixelFormat);
                henseiGraphics.DrawImage(copy, henseiPosition[i, 0], henseiPosition[i, 1], copy.Width, copy.Height);
                orig.Dispose();

            }
            henseiGraphics.Dispose();
            String outPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "hensei.png");
            henseiBitmap.Save(outPath);
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(outPath);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
