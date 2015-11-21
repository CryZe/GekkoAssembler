using System;
using System.IO;
using System.Linq;
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

        private void assemble()
        {
            txtErrorsAndWarnings.Text = string.Empty;

            var assembler = new Assembler();
            var lines = txtInput.Text.Split('\n');
            try
            {
                var gekkoAssembly = assembler.AssembleAllLines(lines);

                ICodeWriter writer;

                if (cmbType.SelectedItem.ToString() == "Gecko")
                {
                    writer = new GeckoWriter();
                }
                else if (cmbType.SelectedItem.ToString() == "Action Replay")
                {
                    writer = new ActionReplayWriter();
                }
                else
                {
                    writer = new ByteCodeWriter();
                }

                var code = writer.WriteCode(gekkoAssembly);

                txtOutput.Text = string.Join(Environment.NewLine, code.Lines);
                txtErrorsAndWarnings.Text = string.Join(Environment.NewLine, code.Errors.Select(x => $"Error: {x}").Concat(code.Warnings.Select(x => $"Warning: {x}")));
            }
            catch (Exception ex)
            {
                txtErrorsAndWarnings.Text = $"Error: {ex.Message}";
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            assemble();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            assemble();
        }
    }
}
