using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GekkoAssembler.Writers;

namespace GekkoAssembler.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            cmbType.SelectedIndex = 0;
        }

        private void bnOpen_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog() { Filter = "Assembly Files (*.asm)|*.asm|All Files (*.*)|*.*", Multiselect = false })
            {
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var name = openDialog.FileName;
                    txtInput.Text = File.ReadAllText(name);
                }
            }
        }

        private void btnAssemble_Click(object sender, EventArgs e)
        {
            var assembler = new Assembler();
            var lines = txtInput.Text.Split('\n');
            var gekkoAssembly = assembler.AssembleAllLines(lines);

            using (var stream = new MemoryStream())
            {
                if (cmbType.SelectedItem.ToString() == "Gecko")
                {
                    new GeckoWriter(stream).Visit(gekkoAssembly);
                }
                else
                {
                    new ActionReplayWriter(stream).Visit(gekkoAssembly);
                }

                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                var output = reader.ReadToEnd();
                txtOutput.Text = output;
            }
        }
    }
}
