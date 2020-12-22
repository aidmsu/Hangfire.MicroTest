using System;

namespace Hangfire.MicroTest.NewsletterService
{
    [Queue("newsletter")]
    public sealed class NewsletterSender
    {
        public static void Execute(long campaignId)
        {
            Console.WriteLine($"Processing newsletter '{campaignId}'");
        }

        public void ExecuteInstance(long campaignId)
        {
            Console.WriteLine($"Processing newsletter '{campaignId}'");
        }
    }
}