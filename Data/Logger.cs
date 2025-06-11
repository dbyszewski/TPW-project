using System.Diagnostics;
using System.Text.Json;
using System.Timers;
using Timer = System.Timers.Timer;

namespace TP.ConcurrentProgramming.Data
{
    internal class Logger
    {
        private Timer timer;
        private List<Ball> balls;

        private string filePath = Directory.GetCurrentDirectory() + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "-log.txt";

        public Logger(List<Ball> balls)
        {
            this.balls = balls;
            SetTimer();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine($"Log entry at {e.SignalTime}");
                int index = 0;
                foreach (Ball ball in balls)
                    writer.WriteLine($"{index++}: {JsonSerializer.Serialize(ball)}");

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
