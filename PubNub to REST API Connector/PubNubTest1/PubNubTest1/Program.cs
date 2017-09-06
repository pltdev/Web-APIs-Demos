using System;
using PubnubApi;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Configuration;
using System.Collections.Specialized;
using RestSharp;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace PubNubTest1
{
    /// <summary>
    /// This Program demo illustrates how to consume the Plantronics Web Services API realtime stream.
    /// It also illustrates how to forward those events (as JSON) onto another REST Service API for 
    /// example to connect to another system.
    /// The events it will look for are QD (quick disconnect) connected/disconnected and Mute (muted/unmuted)
    /// NOTE: In order to work you must edit the .config file in this project (or alongside the EXE) and add
    /// your settings, as follows:
    /// - PubNub_SubscribeKey - this is the PubNub Subscribe key provided for your Plantronics tenant
    /// - PubNub_PublishKey - this is the PubNub Publish key provided for your Plantronics tenant
    /// - PubNub_SubscribeChannel1 - this is the PubNub subscribe channel id 1 provided for your Plantronics tenant
    /// - PubNub_SubscribeChannel2 - this is the PubNub subscribe channel id 2 provided for your Plantronics tenant
    /// - PubNub_SubscribeChannel3 - this is the PubNub subscribe channel id 3 provided for your Plantronics tenant
    /// - PubNub_SubscribeChannel4 - this is the PubNub subscribe channel id 4 provided for your Plantronics tenant
    /// - REST_API_Endpoint - this is URL of a REST Service API to forward the Plantronics realtime event to
    /// 
    /// Author: Lewis.Collins @Plantronics.com, 6th September 2017
    /// </summary>
    class Program
    {
        static public Pubnub pubnub;

        static RestClient client = new RestClient(ConfigurationManager.AppSettings.Get("REST_API_Endpoint"));

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
                    ConfigurationManager.AppSettings.Get("PubNub_SubscribeChannel1"),
                    ConfigurationManager.AppSettings.Get("PubNub_SubscribeChannel2"),
                    ConfigurationManager.AppSettings.Get("PubNub_SubscribeChannel3"),
                    ConfigurationManager.AppSettings.Get("PubNub_SubscribeChannel4"),
                })
                .Execute();

            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            // New code:
            //client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("REST_API_BaseAddress"));
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            System.Console.WriteLine("press enter to quit...");
            System.Console.ReadLine();
        }

        /// <summary>
        /// Forward the message to your own REST Service API...
        /// NOTE: Will echo to console the response received from REST Service API
        /// or if an error occured.
        /// </summary>
        /// <param name="message">The raw JSON object that was received from PubNub library</param>
        /// <returns></returns>
        static async Task ForwardMessageToHTTPService(object message)
        {
            RootObject JSONObj = null;
            try {
                JSONObj = new JavaScriptSerializer().Deserialize<RootObject>(message.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Console.WriteLine("ForwardMessageToHTTPService:" + message);

            if (JSONObj != null)
            {
                //Format: { "SMSNumber":"a mobile number", "Message":"a message body"}
                StringBuilder sb = new StringBuilder();
                sb.Append("{ \"SMSNumber\":\"");
                sb.Append(ConfigurationManager.AppSettings.Get("SMSNumber"));
                sb.Append("\",\"Message\":\"");
                sb.Append(JSONObj.eventType);
                sb.Append(", ");
                sb.Append(JSONObj.timeStamp);
                sb.Append(", ");
                sb.Append(JSONObj.productCode.@base);
                sb.Append(", ");
                sb.Append(JSONObj.productCode.headset);
                sb.Append("\"}");
                try
                {
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("postman-token", "64e44b93-628a-eeb8-2efe-ae1ace92187a");
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
                    request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"family\"\r\n\r\nTest\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"type\"\r\n\r\nSMS\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"version\"\r\n\r\n1.0\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"eventBody\"\r\n\r\n" 
                        + sb.ToString() 
                        + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"\"\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    //HttpResponseMessage response = await client.PostAsJsonAsync(ConfigurationManager.AppSettings.Get("REST_API_RequestUri"), message);
                    //response.EnsureSuccessStatusCode();

                    Console.WriteLine("response: " + response.StatusDescription); // await response.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    Console.WriteLine("error: " + e.ToString());
                }
            }
            else
            {
                Console.WriteLine("could not parse json from pubnub event: "+message.ToString());
            }
        }
    }

    public class BuildCode
    {
        public string @base { get; set; }
        public string headset { get; set; }
    }

    public class GenesSerialNumber
    {
        public string @base { get; set; }
        public string headset { get; set; }
    }

    public class ProductCode
    {
        public string @base { get; set; }
        public string headset { get; set; }
    }

    public class RootObject
    {
        public BuildCode buildCode { get; set; }
        public string deviceId { get; set; }
        public string eventType { get; set; }
        public GenesSerialNumber genesSerialNumber { get; set; }
        public string originTime { get; set; }
        public ProductCode productCode { get; set; }
        public string tenantId { get; set; }
        public string timeStamp { get; set; }
        public string version { get; set; }
    }
}
