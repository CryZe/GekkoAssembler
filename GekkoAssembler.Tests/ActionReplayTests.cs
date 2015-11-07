using System;
using System.Collections.Generic;
using System.IO;
using GekkoAssembler.Writers;
using NUnit.Framework;

namespace GekkoAssembler.Tests
{
    [TestFixture]
    public class ActionReplayTests
    {
        private void Test(string input, string output, string message)
        {
            var inputLines = input.Replace("\r", "").Split('\n');
            var outputLines = output.Replace("\r", "").Split('\n');

            var assembler = new Assembler();
            var gekkoAssembly = assembler.AssembleAllLines(inputLines);

            var writer = new ActionReplayWriter();
            var code = writer.WriteCode(gekkoAssembly);
            var actualLines = code.Lines;

            for (var i = 0; i < Math.Min(outputLines.Length, actualLines.Count); ++i)
            {
                Assert.AreEqual(outputLines[i], actualLines[i], $"Line {i + 1}: {message}");
            }
        }

        [Test(Description = "D-Pad Down for Storage")]
        public void TestStorage()
        {
            var input = ActionReplayResources.StorageInput;
            var output = ActionReplayResources.StorageOutput;
            var message = "D-Pad Down for Storage wasn't converted properly.";

            Test(input, output, message);
        }

        [Test(Description = "R + D-Pad Right to Load Earth Temple")]
        public void TestLoadEarthTemple()
        {
            var input = ActionReplayResources.LoadEarthTempleInput;
            var output = ActionReplayResources.LoadEarthTempleOutput;
            var message = "R + D-Pad Right to Load Earth Temple wasn't converted properly.";

            Test(input, output, message);
        }

        [Test(Description = "Show Stage Information")]
        public void TestShowStageInformation()
        {
            var input = ActionReplayResources.ShowStageInformationInput;
            var output = ActionReplayResources.ShowStageInformationOutput;
            var message = "Show Stage Information wasn't converted properly.";

            Test(input, output, message);
        }

        [Test(Description = "Show In-Game Timer")]
        public void TestShowIGT()
        {
            var input = ActionReplayResources.ShowIGTInput;
            var output = ActionReplayResources.ShowIGTOutput;
            var message = "Show In-Game Timer wasn't converted properly.";

            Test(input, output, message);
        }

        [Test(Description = "Write Pattern Optimizer")]
        public void TestWritePatternOptimizer()
        {
            var input = ActionReplayResources.WritePatternOptimizerInput;
            var output = ActionReplayResources.WritePatternOptimizerOutput;
            var message = "The Write Pattern Optimizer failed to optimize the Write Patterns properly.";

            Test(input, output, message);
        }
    }
}
