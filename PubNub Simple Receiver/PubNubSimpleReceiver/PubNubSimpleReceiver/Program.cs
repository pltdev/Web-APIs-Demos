using System;
using PubnubApi;
using System.Configuration;

namespace PubNubSimpleReceiver
{
    /// <summary>
    /// This Program demo illustrates how to consume the Plantronics Web Services API realtime stream,
    /// to receive headset events such as QD (quick disconnect) connected/disconnected and Mute (muted/unmuted).
    /// NOTE: In order to work you must edit the .config file in this project (or alongside the EXE) and add
    /// your settings, as follows:
    /// - PubNub_SubscribeKey - this key is provided by your PubNub developer account
    /// - PubNub_PublishKey - this key is provided by your PubNub developer account
    /// - PubNub_SubscribeChannel - this channel id you create in your PubNub developer account
    /// Author: Lewis.Collins@Plantronics.com, 11th July 2017
    /// </summary>
    class Program
    {
        static public Pubnub pubnub;

        static public void Main()
        {
            PNConfiguration config = new PNConfiguration();
            config.SubscribeKey = ConfigurationManager.AppSettings.Get("PubNub_SubscribeKey");
            config.PublishKey = ConfigurationManager.AppSettings.Get("PubNub_PublishKey");

            pubnub = new Pubnub(config);
            pubnub.AddListener(new SubscribeCallbackExt(
                (pubnubObj, message) =>
                {
                    // Log the new message stored in message.Message to the console
                    if (message != null)
                    {
                        Console.WriteLine("Msg: " + message.Message +
                            (message.Channel != null ? ", On chan: " + message.Channel : "") );
                    }
                },
                (pubnubObj, presence) => { },
                (pubnubObj, status) =>
                {
                    Console.WriteLine("PubNub Status: " + status.Category);
                }
            ));

            // Subscripe to your PubNub channel and wait for events
            pubnub.Subscribe<string>()
                .Channels(new string[] {
                    ConfigurationManager.AppSettings.Get("PubNub_SubscribeChannel")
                })
                .Execute();

            System.Console.WriteLine("press enter to quit...");
            System.Console.ReadLine();
        }
    }
}
