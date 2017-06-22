using System;
using PubnubApi;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace PubNubTest1
{
    class Program
    {
        static public Pubnub pubnub;

        static HttpClient client = new HttpClient();

        static public void Main()
        {
            PNConfiguration config = new PNConfiguration();
            config.SubscribeKey = "sub-c-e0e48298-c1a7-11e6-8036-0619f8945a4f";
            config.PublishKey = "pub-c-36c67bb6-e1b8-4fc9-8017-040c9cc8c4ce";

            pubnub = new Pubnub(config);
            pubnub.AddListener(new SubscribeCallbackExt(
                (pubnubObj, message) => {
            // Handle new message stored in message.Message
            if (message != null)
                    {
                        if (message.Channel != null)
                        {
                            // Message has been received on channel group stored in
                            // message.Channel()
                            System.Console.WriteLine("Channel grp msg: "+message.Message.ToString());
                        }
                        else
                        {
                            // Message has been received on channel stored in
                            // message.Subscription()
                            System.Console.WriteLine("Channel sub msg: " + message.Message.ToString());
                        }

                        ForwardMessageToHTTPService(message.Message);
                        /*
                            log the following items with your favorite logger
                                - message.Message()
                                - message.Subscription()
                                - message.Timetoken()
                        */
                    }
                },
                (pubnubObj, presence) => { },
                (pubnubObj, status) => {
                    if (status.Category == PNStatusCategory.PNUnexpectedDisconnectCategory)
                    {
                // This event happens when radio / connectivity is lost
            }
                    else if (status.Category == PNStatusCategory.PNConnectedCategory)
                    {
                // Connect event. You can do stuff like publish, and know you'll get it.
                // Or just use the connected event to confirm you are subscribed for
                // UI / internal notifications, etc

                pubnub.Publish()
                            .Channel("f343e170-5f4e-43c2-bcbc-4dd5e7d0a99a_sub1")
                            .Message("hello!!")
                            .Async(new PNPublishResultExt((publishResult, publishStatus) => {
                        // Check whether request successfully completed or not.
                        if (!publishStatus.Error)
                                {
                            // Message successfully published to specified channel.
                        }
                                else
                                {
                            // Request processing failed.

                            // Handle message publish error. Check 'Category' property to find out possible issue
                            // because of which request did fail.
                        }
                            }));
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

            pubnub.Subscribe<string>()
                .Channels(new string[] {
            "f343e170-5f4e-43c2-bcbc-4dd5e7d0a99a_sub1"
                })
                .Execute();

            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            // New code:
            //client.BaseAddress = new Uri("http://localhost:8888/");
            client.BaseAddress = new Uri("http://plttest.weblogikk.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            System.Console.WriteLine("press enter to quit...");
            System.Console.ReadLine();
        }

        static async Task ForwardMessageToHTTPService(object message)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("plt.php", message);
                response.EnsureSuccessStatusCode();

                Console.WriteLine("response: " + await response.Content.ReadAsStringAsync());
            }
            catch(Exception e)
            {
                Console.WriteLine("error: " + e.ToString());
            }
        }
    }
}
