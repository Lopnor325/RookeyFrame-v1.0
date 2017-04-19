namespace Rookey.Frame.QuartzClient
{
    using System;

    public class TriggerData : Activity
    {
        public TriggerData(string name, ActivityStatus status) : base(name, status)
        {
        }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public DateTimeOffset? NextFireDate { get; set; }

        public DateTimeOffset? PreviousFireDate { get; set; }

        public System.String Group { get; set; }

        public System.Int32 Priority { get; set; }

        public System.String Des { get; set; }
    }
}