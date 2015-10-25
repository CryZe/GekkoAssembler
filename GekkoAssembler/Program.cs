using System;
using GekkoAssembler.ActionReplay;
using static System.IO.File;

namespace GekkoAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("An Assembly File was expected as the first argument.");
                return;
            }

            var filePath = args[0];
            if (!Exists(filePath))
            {
                Console.WriteLine($"File { filePath } was not found.");
                return;
            }

            var lines = ReadAllLines(filePath);
            var assembler = new Assembler();
            var gekkoAssembly = assembler.AssembleAllLines(lines);
            var stream = Console.OpenStandardOutput();
            var arWriter = new ActionReplayWriter(stream);
            gekkoAssembly.Accept(arWriter);
        }
    }
}
