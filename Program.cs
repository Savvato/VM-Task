namespace VMTask
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            const int BaseWindowSize = 5 * 60; // 5 minutes
            const int AnalysedWindowSize = 2; // 2 seconds
            float sumFor5Minutes = 0;
            float averageFor5Minutes = 0;

            using PerformanceCounter cpuUsageCounter = new(
                categoryName: "Processor Information",
                counterName: "% Processor Utility",
                instanceName: "_Total",
                readOnly: true);

            Queue<float> measurements = new();
            LinkedList<float> latestValues = new();

            while (true)
            {
                await Task.Delay(1000);
                float value = cpuUsageCounter.NextValue();
                Console.WriteLine(value);

                measurements.Enqueue(value);
                sumFor5Minutes += value;

                latestValues.AddLast(value);

                if (latestValues.Count > AnalysedWindowSize)
                {
                    latestValues.RemoveFirst();
                }

                if (measurements.Count > BaseWindowSize)
                {
                    sumFor5Minutes -= measurements.Dequeue();
                };

                averageFor5Minutes = sumFor5Minutes / measurements.Count;

                if (latestValues.All(value => value / averageFor5Minutes > 1.2))
                    Console.WriteLine("ALERT");
            }
        }
    }
}
