using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Executors
{
    public class ProcessExecutor
    {
        /// <summary>
        /// Executes a command asynchronously with specified arguments, and captures both standard output and error streams.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="arguments">The arguments for the command.</param>
        /// <param name="timeoutMilliseconds">Optional timeout for the command execution.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the command execution.</param>
        /// <returns>Returns a tuple containing output, error message, and exit code.</returns>
        public async Task<(string Output, string Error, int ExitCode)> ExecuteAsync(
            string command,
            string arguments = "",
            int timeoutMilliseconds = 10000,
            CancellationToken cancellationToken = default)
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.OutputDataReceived += (sender, args) => outputBuilder.AppendLine(args.Data);
            process.ErrorDataReceived += (sender, args) => errorBuilder.AppendLine(args.Data);

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                using var timeoutCts = new CancellationTokenSource(timeoutMilliseconds);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                await Task.Run(() => process.WaitForExit(), linkedCts.Token);

                // if process didnt exit within timeout or token was requested
                if (!process.HasExited)
                {
                    process.Kill();
                    return (Output: outputBuilder.ToString(), Error: "Process killed due to timeout or cancellation.", ExitCode: -1);
                }

                return (Output: outputBuilder.ToString(), Error: errorBuilder.ToString(), ExitCode: process.ExitCode);
            }
            catch (Exception ex)
            {
                return (Output: "", Error: $"Execution failed: {ex.Message}", ExitCode: -1);
            }
        }
    }
}




