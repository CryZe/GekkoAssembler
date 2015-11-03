using System.Collections.Generic;

namespace GekkoAssembler
{
    public class CodeBuilder
    {
        public List<string> Lines { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Errors { get; set; }

        public CodeBuilder()
        {
            Lines = new List<string>();
            Warnings = new List<string>();
            Errors = new List<string>();
        }

        public void WriteLine(string line)
        {
            Lines.Add(line);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddCode(Code code)
        {
            Lines.AddRange(code.Lines);
            Warnings.AddRange(code.Warnings);
            Errors.AddRange(code.Errors);
        }

        public Code Build()
        {
            return new Code(Lines, Warnings, Errors);
        }
    }
}
