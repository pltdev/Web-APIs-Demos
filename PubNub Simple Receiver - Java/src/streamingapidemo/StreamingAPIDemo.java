/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package streamingapidemo;

import com.pubnub.api.PNConfiguration;
import com.pubnub.api.PubNub;
import com.pubnub.api.callbacks.SubscribeCallback;
import com.pubnub.api.models.consumer.PNStatus;
import com.pubnub.api.models.consumer.pubsub.PNMessageResult;
import com.pubnub.api.models.consumer.pubsub.PNPresenceEventResult;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Arrays;
import java.util.Properties;
import java.util.Scanner;

/**
  * This Program demo illustrates how to consume the Plantronics Web Services API realtime stream,
  * to receive headset events such as QD (quick disconnect) connected/disconnected and Mute (muted/unmuted).
  * NOTE: In order to work you must edit the .properties file in this project (to end up in Working Folder) 
  * and add your settings, as follows:
  * - PubNub_SubscribeKey - this key is provided by your PubNub developer account
  * - PubNub_PublishKey - this key is provided by your PubNub developer account
  * - PubNub_SubscribeChannel - this channel id you create in your PubNub developer account
  * Author: Lewis.Collins@Plantronics.com, 26th July 2017
 * @author LCollins
 */
public class StreamingAPIDemo {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        // TODO code application logic here
        Properties prop = LoadAppConfig();
        
        PNConfiguration config = new PNConfiguration();
        config.setSubscribeKey(prop.getProperty("PubNub_SubscribeKey"));
        config.setPublishKey(prop.getProperty("PubNub_PublishKey"));

        PubNub pubnub = new PubNub(config);

        pubnub.addListener(new SubscribeCallback() {
            @Override
            public void message(PubNub pubnub, PNMessageResult message) {
                // Log the new message stored in message.Message to the console
                if (message != null)
                {
                    System.out.println("Msg: " + message.getMessage() +
                        (message.getChannel() != null ? ", On chan: " + message.getChannel() : "") );
                }
            }

            @Override
            public void status(PubNub pubnub, PNStatus status) {
                System.out.println("PubNub Status: " + status.getCategory());
            }

            @Override
            public void presence(PubNub pubnub, PNPresenceEventResult presence) {
            }
        });

        pubnub.subscribe().channels(Arrays.asList(prop.getProperty("PubNub_SubscribeChannel"))).execute();
        
        System.out.println("press enter to quit...");
        Scanner scan=new Scanner(System.in);
        scan.nextLine();
        
        System.out.println("shutting down...");
        pubnub.unsubscribeAll();
        pubnub.disconnect();
        pubnub.destroy();
        System.out.println("Bye.");
    }    

    private static Properties LoadAppConfig() {
        // thanks: http://www.mkyong.com/java/java-properties-file-examples/
	Properties prop = new Properties();
	InputStream input = null;

	try {
            System.out.println("Working Directory (looking for Appconfig.properties here) = " +
                System.getProperty("user.dir"));
            input = new FileInputStream("Appconfig.properties");

            // load a properties file
            prop.load(input);
        } catch (IOException ex) {
            System.out.println("ERROR loading Appconfig.properties. Is it in the Working Directory?");
            ex.printStackTrace();
	} finally {
            if (input != null) {
                try {
                    input.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
	}
        return prop;
    }
}
