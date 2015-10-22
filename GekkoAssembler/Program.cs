using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.IO.File;

namespace GekkoAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Assembly File as argument expected.");
                return;
            }

            var filePath = args[0];
            if (!Exists(filePath))
            {
                Console.WriteLine($"File { filePath } not found.");
                return;
            }
            var lines = ReadAllLines(filePath);
            var assembler = new Assembler();
            var gekkoAssembly = assembler.AssembleAllLines(lines);
            var cheat = gekkoAssembly.ToCheat();
            Console.WriteLine(cheat);
        }
    }
}
