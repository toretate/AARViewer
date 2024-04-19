using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

// AIGファイルの処理

namespace AigViewer
{
    public partial class AigViewer : UserControl
    {
        private enum ColorType { RGBA, BGRA, RGBQuad, UNKNOWN, }

        private ColorType m_colorType;
        private Bitmap m_canvas;

        public AigViewer()
        {
            InitializeComponent();
        }

        // AIGStreamかどうかを調べる
        public static bool IsAig( Stream aigStream ) {
            bool result;
            long position = aigStream.Position;
            aigStream.Position = 0;

            BinaryReader reader = new BinaryReader(aigStream);
            {
                // ALIG で始まるかどうか調査
                byte[] magic = reader.ReadBytes(4);
                byte[] alig = new byte[] { (byte)'A', (byte)'L', (byte)'I', (byte)'G' };
                result = alig.SequenceEqual(magic);
            }
            aigStream.Position = position;

            return result;
        }

        public void LoadPicture(BinaryReader reader)
        {
            if (IsAig(reader.BaseStream) == false) return;
            reader.BaseStream.Position = 0;
            reader.ReadBytes(4);    // "ALIG"

            {
                // 4byte読み飛ばす(何のフラグか不明)
                byte[] flags1 = reader.ReadBytes(4);

                // RGBA/BGRA を判定
                {
                    byte[] type = reader.ReadBytes(4);
                    byte[] rgba = new byte[] { (byte)'R', (byte)'G', (byte)'B', (byte)'A' };
                    byte[] bgra = new byte[] { (byte)'B', (byte)'G', (byte)'R', (byte)'A' };
                    byte[] abg4 = new byte[] { (byte)'A', (byte)'B', (byte)'G', (byte)'4' };
                    if (type.SequenceEqual(rgba))
                    {
                        m_colorType = ColorType.RGBA;
                    }
                    else if (type.SequenceEqual(bgra))
                    {
                        m_colorType = ColorType.BGRA;
                    }
                    else if (type.SequenceEqual(abg4))
                    {
                        m_colorType = ColorType.RGBQuad;
                    }
                    else
                    {
                        m_colorType = ColorType.UNKNOWN;
                        return;
                    }
                }

                // 4byte読み飛ばす(何のフラグか不明)
                byte[] flags2 = reader.ReadBytes(4);

                // 謎の16byte読み飛ばす
                byte[] flags3 = reader.ReadBytes(16);

                // 画像をロード
                {
                    int width = 1024;
                    byte[] data = new byte[4];
                    Stream stream = reader.BaseStream;
                    int height = (int)(stream.Length / (width * data.Length)) + 100;
                    m_canvas = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(m_canvas);

                    long i = 0, y = 0;
                    while (i < stream.Length)
                    {
                        for (long x = 0; x < width; x++)
                        {
                            data = reader.ReadBytes(4);
                            if (data.Length < 4) break;

                            if (m_colorType == ColorType.RGBA)
                            {
                                Color c = Color.FromArgb(data[3], data[0], data[1], data[2]);
                                m_canvas.SetPixel((int)x, (int)y, c);
                            }
                            else if( m_colorType == ColorType.BGRA)
                            {
                                Color c = Color.FromArgb(data[3], data[2], data[1], data[0]);
                                m_canvas.SetPixel((int)x, (int)y, c);
                            }
                            else if (m_colorType == ColorType.RGBQuad)
                            {
                                Color c = Color.FromArgb(255, data[2], data[1], data[0]);
                                m_canvas.SetPixel((int)x, (int)y, c);
                            }
                        }
                        if (data.Length < 4) break;
                        i += width * data.Length;
                        y++;
                    }

                    g.Dispose();
                    pictureBox1.Location = new Point(0, 0);
                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox1.Image = m_canvas;

                }
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(new Point(m_canvas.Width, m_canvas.Height));
        }
    }
}
