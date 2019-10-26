using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class RunExecutable : Expression
    {
        private static readonly Expression higherOrder =
            Factories.FromType<string>();

        public readonly string Path;
        public readonly Expression Argument;

        internal RunExecutable(string path, Expression argument)
        {
            this.Path = path;
            this.Argument = argument;
        }

        public override Expression HigherOrder =>
            higherOrder;

        public override int GetHashCode() =>
            this.Path.GetHashCode();

        public bool Equals(RunExecutable? other) =>
            other?.Path.Equals(this.Path) ?? false;

        public override bool Equals(Expression? other) =>
            this.Equals(other as RunExecutable);

        public override string ToString() =>
           System.IO.Path.GetFileNameWithoutExtension(this.Path);

        public override Expression Run()
        {
            var buffer = new StringBuilder();

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(this.Path)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                process.OutputDataReceived += (s, e) =>
                    buffer.AppendLine(e.Data);

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                using (var tw = process.StandardInput)
                {
                    var input = ((Value)this.Argument.Run()).RawValue;

                    tw.Write(input);
                    tw.Flush();
                }

                process.WaitForExit();
            }

            return Factories.Value(buffer.ToString());
        }
    }
}
