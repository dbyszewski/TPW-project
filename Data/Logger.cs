using System.Text.Json;
using System.Timers;
using Timer = System.Timers.Timer;

namespace TP.ConcurrentProgramming.Data
{
    internal class Log
    {
        private string _message;
        private DateTime _logTime;

        public Log(string message)
        {
            _message = message;
            _logTime = DateTime.Now;
        }

        public string GetStringLog()
        {
            return $"Collision at: {_logTime} - {_message}";
        }
    }

    internal class Logger : ILogger
    {
        private Timer timer;
        private Queue<Log> logsToAdd;

        private string filePath = Directory.GetCurrentDirectory() + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "-log.txt";

        public Logger()
        {
            logsToAdd = new Queue<Log>();
            SetTimer();
        }

        public void AddLog(IBall ball)
        {
            var log = new Log($"WallCollision - ball: {JsonSerializer.Serialize(ball)}");
            logsToAdd.Enqueue(log);
        }

        public void AddLog(IBall b1, IBall b2)
        {
            var log = new Log($"BallsCollision - ball1: {JsonSerializer.Serialize(b1)}; ball2:{JsonSerializer.Serialize(b2)}");
            logsToAdd.Enqueue(log);
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                if (logsToAdd == null) return;
                while (logsToAdd.Count > 0)
                {
                    var log = logsToAdd.Dequeue();
                    writer.WriteLine(log.GetStringLog());
                }
            }
        }
        
        private void SetTimer()
        {
            timer = new Timer(1000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
    }
}
