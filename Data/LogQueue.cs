using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class LogQueue : IDisposable
    {
        private readonly BlockingCollection<string> _logQueue = new();
        private readonly string _logFilePath;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _loggingTask;

        public LogQueue()
        {
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            _logFilePath = $"log_{timestamp}.log";

            // Startuje osobny task do zapisu do pliku
            _loggingTask = Task.Run(() => ProcessQueue(_cts.Token));
        }

        public void Log(string message)
        {
            string timestampedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}";
            _logQueue.Add(timestampedMessage);
        }

        private void ProcessQueue(CancellationToken token)
        {
            using var writer = new StreamWriter(_logFilePath, append: true);

            foreach (var message in _logQueue.GetConsumingEnumerable(token))
            {
                writer.WriteLine(message);
                writer.Flush(); // natychmiastowy zapis
            }
        }

        public void Dispose()
        {
            _logQueue.CompleteAdding();
            _cts.Cancel();
            try
            {
                _loggingTask.Wait();
            }
            catch (AggregateException) { /* ignoruj jeśli task został anulowany */ }
            _cts.Dispose();
        }
    }

}
