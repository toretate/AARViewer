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


namespace AARViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private MemoryStream m_bufStream;
        private Bitmap m_canvas;

        private String m_fileName;
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                m_fileName = dialog.FileName;

                Stream picStream = new MemoryStream();

                using (Stream reader = dialog.OpenFile())
                {
                    m_bufStream = new MemoryStream();
                    reader.CopyTo(m_bufStream);
                    m_bufStream.Position = 0;

                    m_bufStream.CopyTo(picStream);
                    picStream.Position = 0;
                }
                readAndShow(m_bufStream);

                // Picture
                int width = 1024;
                byte[] rgba = new byte[4];
                int height = (int)(picStream.Length / (width * rgba.Length)) + 100;
                m_canvas = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(m_canvas);

                long i = 0, y = 0;
                while (i < picStream.Length)
                {
                    for (long x = 0; x < width; x++)
                    {
                        rgba[2] = (byte)picStream.ReadByte();
                        rgba[1] = (byte)picStream.ReadByte();
                        rgba[0] = (byte)picStream.ReadByte();
                        rgba[3] = (byte)picStream.ReadByte();
                        Color c = Color.FromArgb(rgba[0], rgba[1], rgba[2]);
                        m_canvas.SetPixel((int)x, (int)y, c);
                    }
                    i += width *rgba.Length;
                    y++;
                }

                g.Dispose();
                pictureBox1.Location = new Point(0, 0);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize; 
                pictureBox1.Image = m_canvas;
            }
        }

        private void readAndShow(Stream stream)
        {
            hexBox1.ByteProvider = new Be.Windows.Forms.DynamicFileByteProvider( stream );
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(new Point(m_canvas.Width, m_canvas.Height));
        }

        private void ダンプToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ALBridge bridge = new ALBridge.Class1();
            bridge.dump();
        }
    }
}
