using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalBookingMonitoringSystemFunctionApp.Tests
{
    public class TestCollector<T> : ICollector<T>
    {
        public List<T> Collector => new List<T>();

        public int Count
        {
            get
            {
                return Collector.Count;
            }
        }

        public void Add(T item)
        {
            Collector.Add(item);
        }

        public List<T> GetCollector()
        {
            return Collector;
        }
    }
}
