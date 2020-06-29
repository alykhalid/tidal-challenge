using System;
using System.Collections.Generic;
using System.Threading;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class EventLimiter
 {
    Queue<DateTime> requestTimes;
    int maxRequests;
    TimeSpan timeSpan;
    NorTide nortide = new NorTide();

    public EventLimiter()
        : this(20, TimeSpan.FromSeconds(1))
    { }

    public EventLimiter(int maxRequests, TimeSpan timeSpan)
    {
        this.maxRequests = maxRequests;
        this.timeSpan = timeSpan;
        requestTimes = new Queue<DateTime>(maxRequests);
    }

    private void SynchronizeQueue()
    {
        while ((requestTimes.Count > 0) && (requestTimes.Peek().Add(timeSpan) < DateTime.UtcNow))
            requestTimes.Dequeue();
    }

    public bool CanRequestNow()
    {
        SynchronizeQueue();
        return requestTimes.Count < maxRequests;
    }

    public async Task EnqueueRequest(List<Measurement> Measurements)
    {
        for (int i = 0; i < Measurements.Count; i++)
        {
            while (!CanRequestNow())               
                Thread.Sleep(requestTimes.Peek().Add(timeSpan).Subtract(DateTime.UtcNow));
            
            await nortide.getWaterLevel(Measurements[i]);
            Console.WriteLine("Processing: " + (i + 1) + " of " + Measurements.Count);
            if(i > 10)
                break;
            requestTimes.Enqueue(DateTime.UtcNow);
        }
    }
}