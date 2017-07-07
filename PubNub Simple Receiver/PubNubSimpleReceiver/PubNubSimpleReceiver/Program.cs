using System;
using PubnubApi;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Configuration;
using System.Collections.Specialized;

namespace PubNubSimpleReceiver
{
    /// <summary>
    /// This Program demo illustrates how to consume the Plantronics Web Services API realtime stream.
    /// It also illustrates how to forward those events (as JSON) onto another REST Service API for 
    /// example to connect to another system.
    /// The events it will look for are QD (quick disconnect) connected/disconnected and Mute (muted/unmuted)
    /// NOTE: In order to work you must edit the .config file in this project (or alongside the EXE) and add
    /// your settings, as follows:
    /// - PubNub_SubscribeKey - this key is provided by your PubNub developer account
    /// - PubNub_PublishKey - this key is provided by your PubNub developer account
    /// - PubNub_SubscribeChannel - this channel id you create in your PubNub developer account
    /// - REST_API_BaseAddress - this is the base URL of a REST Service API to forward the event to,
    ///     e.g. http://localhost:8888/ (can also be a remote URL)
    /// - REST_API_RequestUri - this is the script name of the REST Service API to forward the event to,
    ///     i.e. the bit of the URL after the BaseURL, e.g. myscript.php
    /// Author: Lewis.Collins@Plantronics.com, 22nd June 2017
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
                    // Handle new message stored in message.Message
                    if (message != null)
                    {
                        if (message.Channel != null)
                        {
                            // Message has been received on channel group stored in
                            // message.Channel()
                            System.Console.WriteLine("Channel grp msg: " + message.Message.ToString());
                        }
                        else
                        {
                            // Message has been received on channel stored in
                            // message.Subscription()
                            System.Console.WriteLine("Channel sub msg: " + message.Message.ToString());
                        }

                        // Forward the message to your own REST Service API...
                        DisplayMessageOnConsole(message.Message);

                        /*
                            log the following items with your favorite logger
                                - message.Message()
                                - message.Subscription()
                                - message.Timetoken()
                        */
                    }
                },
                (pubnubObj, presence) => { },
                (pubnubObj, status) =>
                {
                    if (status.Category == PNStatusCategory.PNUnexpectedDisconnectCategory)
                    {
                        // This event happens when radio / connectivity is lost
                    }
                    else if (status.Category == PNStatusCategory.PNConnectedCategory)
                    {
                        // Connect event. You can do stuff like publish, and know you'll get it.
                        // Or just use the connected event to confirm you are subscribed for
                        // UI / internal notifications, etc

                        // Commenting this out, this just shows how to send an event back to 
                        // the pubnub channel, should you wish to do that. As you are consuming
                        // events from Plantronics, you probably won't need to do it.
                        //pubnub.Publish()
                        //            .Channel(ConfigurationManager.AppSettings.Get("PubNub_SubscribeChannel"))
                        //            .Message("hello!!")
                        //            .Async(new PNPublishResultExt((publishResult, publishStatus) =>
                        //            {
                        //        // Check whether request successfully completed or not.
                        //        if (!publishStatus.Error)
                        //                {
                        //            // Message successfully published to specified channel.
                        //        }
                        //                else
                        //                {
                        //            // Request processing failed.

                        //            // Handle message publish error. Check 'Category' property to find out possible issue
                        //            // because of which request did fail.
                        //        }
                        //            }));
                    }
                    else if (status.Category == PNStatusCategory.PNReconnectedCategory)
                    {
                        // Happens as part of our regular operation. This event happens when
                        // radio / connectivity is lost, then regained.
                    }
                    else if (status.Category == PNStatusCategory.PNDecryptionErrorCategory)
                    {
                        // Handle messsage decryption error. Probably client configured to
                        // encrypt messages and on live data feed it received plain text.
                    }
                }
            ));

            // Subscripe to your PubNub channel and wait for events
            pubnub.Subscribe<string>()
                .Channels(new string[] {
                    ConfigurationManager.AppSettings.Get("PubNub_SubscribeChannel")
                })
                .Execute();

            RunAsync().Wait();
        }

        /// <summary>
        /// Display the received JSON message from pubnub to the console
        /// Note: instead of to console you could send this to a database, or other system, etc.
        /// </summary>
        /// <param name="message">The raw JSON object that was received from PubNub library</param>
        private static void DisplayMessageOnConsole(object message)
        {
            try
            {
                Console.WriteLine("Message received: " + message);
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.ToString());
            }
        }

        static async Task RunAsync()
        {
            System.Console.WriteLine("press enter to quit...");
            System.Console.ReadLine();
        }
    }
}
