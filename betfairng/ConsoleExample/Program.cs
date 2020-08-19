using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Configuration;
using BetfairNG;
using BetfairNG.Data;
//using Betfair.ESAClient.Cache;
//using Betfair.ESAClient.Protocol;

// This example pulls the latest horse races in the UK markets
// and displays them to the console.
public class ConsoleExample
{
    public static ConcurrentQueue<MarketCatalogue> Markets = new ConcurrentQueue<MarketCatalogue>();

    public static void Main()
    {
        // TODO:// replace with your app details and Betfair username/password
        BetfairClient client = new BetfairClient("UhwmsL3EqCwjEKwH");
        client.Login(@"/etc/client-2048.p12", "REDsky.123", "garethreid123@gmail.com", "REDsky.123");

        // Exchange Streaming API example, see: http://docs.developer.betfair.com/docs/display/1smk3cen4v3lu3yomq5qye0ni/Exchange+Stream+API
        // TODO:// replace with your app deatils and Betfair username/password, and enable streaming support on your Betfair account
        //StreamingBetfairClient streamingClient = new StreamingBetfairClient("stream-api-integration.betfair.com", "APPKEY");
        //streamingClient.Login("username", "password");

        /*
         * OriginalExample runs the code originally in here, using the standard MarketListener
         * PeriodicExample runs a version of MarketListener (MarketListenerPeriodic), using an RX interval, specified in seconds
         * MultiPeriodExample runs a version of MarketListenerPeriodic (MarketListenerMultiPeriod), using potentially differing poll intervals per market book
         */

        var example = new OriginalExample(client); // This example blocks within GO
        //var example = new StreamingExample(client, streamingClient); // Betfair Exchange Streaming API example
        //var example = new PeriodicExample(client, 0.5);
        //var example = new MultiPeriodExample(client);
        example.Test();

        if(!example.IsBlocking) Thread.Sleep(TimeSpan.FromMinutes(20));

        Console.WriteLine("done.");
        Console.ReadLine();
    }
}
