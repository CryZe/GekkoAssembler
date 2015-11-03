using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GekkoAssembler
{
    public class Code
    {
        public ReadOnlyCollection<string> Lines { get; }
        public ReadOnlyCollection<string> Warnings { get; }
        public ReadOnlyCollection<string> Errors { get; }

        public Code(IEnumerable<string> lines, IEnumerable<string> warnings, IEnumerable<string> errors)
        {
            Lines = lines.AsReadOnly();
            Warnings = warnings.AsReadOnly();
            Errors = errors.AsReadOnly();
        }
    }
}