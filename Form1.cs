using System;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace VAG_FRF_Extractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button_OpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "FRF Files |*.frf";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                if (File.Exists(ofd.FileName) == false) return;
                var key = Key.GetKey();
                var crypted_file = File.ReadAllBytes(ofd.FileName);
                var decrypted_file = new byte[crypted_file.Length];
                int first_seed = 0;
                int second_seed = 1;
                for (int i = 0; i < crypted_file.Length; i++)
                {
                    var key_byte = key[i % key.Length];
                    first_seed = ((first_seed + key_byte) * 3) & 0xFF;
                    decrypted_file[i] = (byte)(crypted_file[i] ^ (first_seed ^ 0xFF ^ second_seed ^ key_byte));
                    second_seed = ((second_seed + 1) * first_seed) & 0xFF;
                }
                var fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() != DialogResult.OK) return;

                File.WriteAllBytes("decrypted.zip", decrypted_file);
                ZipFile.ExtractToDirectory("decrypted.zip", fbd.SelectedPath);
                File.Delete("decrypted.zip");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
