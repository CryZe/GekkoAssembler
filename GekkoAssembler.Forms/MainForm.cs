using System;
using System.IO;
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

            ICodeWriter writer;

            if (cmbType.SelectedItem.ToString() == "Gecko")
            {
                writer = new GeckoWriter();
            }
            else
            {
                writer = new ActionReplayWriter();
            }

            var code = writer.WriteCode(gekkoAssembly);

            txtOutput.Text = string.Join(Environment.NewLine, code.Lines);
        }
    }
}
