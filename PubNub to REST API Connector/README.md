This Program demo illustrates how to consume the Plantronics Web Services API realtime stream.
It also illustrates how to forward those events (as JSON) onto another REST Service API for 
example to connect to another system.
The events it will look for are QD (quick disconnect) connected/disconnected and Mute (muted/unmuted)
NOTE: In order to work you must edit the .config file in this project (or alongside the EXE) and add
your settings, as follows:
- PubNub_SubscribeKey - this is the PubNub Subscribe key provided for your Plantronics tenant
- PubNub_PublishKey - this is the PubNub Publish key provided for your Plantronics tenant
- PubNub_SubscribeChannel1 - this is the PubNub subscribe channel id 1 provided for your Plantronics tenant
- PubNub_SubscribeChannel2 - this is the PubNub subscribe channel id 2 provided for your Plantronics tenant
- PubNub_SubscribeChannel3 - this is the PubNub subscribe channel id 3 provided for your Plantronics tenant
- PubNub_SubscribeChannel4 - this is the PubNub subscribe channel id 4 provided for your Plantronics tenant
- Plantronics User_Device_ID - this is the unique device ID for your user's Plantronics device of interest (as defined in Plantronics Manager Pro tenant data)
- REST_API_Endpoint - ADD YOUR REST API ENDPOINT HERE TO FORWARD THE EVENT TO
- SMSNumber - (OPTIONAL) ADD YOUR MOBILE NUMBER HERE TO USE IN YOUR REST API FLOW LATER
    
Author: Lewis.Collins@Plantronics.com, 6th September 2017
