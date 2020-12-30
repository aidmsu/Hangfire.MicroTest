using System;

namespace Hangfire.MicroTest.NewsletterService
{
    [Queue("newsletter")]
    public sealed class NewsletterSender
    {
        public static void Execute(long campaignId, [FromResult] int? prevCampaignId)
        {
            Console.WriteLine($"Processing newsletter '{campaignId}' after '{prevCampaignId}'");
        }

        public static int ExecuteGeneric<T>(T campaignId)
        {
            Console.WriteLine($"Processing newsletter '{campaignId}'");
            return 123; 
        }

        [DisableConcurrentExecution(1)]
        public void ExecuteInstance(long campaignId)
        {
            Console.WriteLine($"Processing newsletter '{campaignId}'");
        }
    }
}