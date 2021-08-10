using System.Diagnostics;
using System.Threading.Tasks;

namespace FFXIVMusicExporter.Core.Helpers
{
    public static class RunExternalProcess
    {
        private static string _output = "";
        public static async Task<string> LaunchAsync(Process process) => await Task.Run(() => Launch(process));

        public static string Launch(Process process)
        {
            _output = string.Empty;
            using (process)
            {
                //* Set your output and error (asynchronous) handlers
                process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);

                //* Start process and handlers
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            return _output;
        }

        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine) =>
            //* Do your stuff with the output (write to console/log/StringBuilder)
            //Console.WriteLine(outLine.Data);

            _output += $"{outLine.Data}\r\n";
    }
}