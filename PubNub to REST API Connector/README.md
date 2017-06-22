This Program demo illustrates how to consume the Plantronics Web Services API realtime stream.
It also illustrates how to forward those events (as JSON) onto another REST Service API for 
example to connect to another system.
The events it will look for are QD (quick disconnect) connected/disconnected and Mute (muted/unmuted)
NOTE: In order to work you must edit the .config file in this project (or alongside the EXE) and add
your settings, as follows:
- PubNub_SubscribeKey - this key is provided by your PubNub developer account
- PubNub_PublishKey - this key is provided by your PubNub developer account
- PubNub_SubscribeChannel - this channel id you create in your PubNub developer account
- REST_API_BaseAddress - this is the base URL of a REST Service API to forward the event to,
    e.g. http://localhost:8888/ (can also be a remote URL)
- REST_API_RequestUri - this is the script name of the REST Service API to forward the event to,
    i.e. the bit of the URL after the BaseURL, e.g. myscript.php
Author: Lewis.Collins@Plantronics.com, 22nd June 2017
