namespace GCop.ErrorHandling.Core.Attributes
{
    using System;

    public class DelayAttribute:Attribute
    {
        public DelayAttribute(int minute)
        {
            Minute = minute;
        }

        public int Minute { get; set; }
    }
}
