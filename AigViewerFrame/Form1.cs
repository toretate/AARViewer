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

namespace AigViewerFrame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void フォルダToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 指定ディレクトリから atx ファイルを読み込んでファイル一覧に出す

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                String dirPath = dialog.SelectedPath;
                String[] files = System.IO.Directory.GetFiles( dirPath, "*.atx", System.IO.SearchOption.TopDirectoryOnly);
                listBox1.Items.AddRange(files);
            }
            dialog.Dispose();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 選択肢が変更されたとき、AIG Viewer に出すリソースを変える
            String path = listBox1.SelectedItem as String;
            if (path == null) return;

            using( Stream stream = new FileStream( path, FileMode.Open ) ) {
                aigViewer1.LoadPicture(new BinaryReader(stream));
            }
        }
    }
}
