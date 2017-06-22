<html>
<body>
<h1>plt realtime demo</h1>
<?php
function isValidJSON($str) {
   json_decode($str);
   return json_last_error() == JSON_ERROR_NONE;
}

//print "<p>raw data:" . $HTTP_RAW_POST_DATA . "</p><p>";

$json_params = file_get_contents("php://input");

if (strlen($json_params) > 0 && isValidJSON($json_params))
{
    //print "<br\>JSON params = ".$json_params;
    $json_params = trim(stripslashes(html_entity_decode($json_params)),'"');
    //print "<br\>JSON params 2 = ".$json_params;
    $decoded_params = json_decode($json_params, TRUE);
    //print "JSON params: " . $json_params;    
    if ($decoded_params==NULL) print "WAS NULL";
    //print "JSON dump: " . var_dump($decoded_params);    
    //print "Event Type = ".$decoded_params["eventType"];
    switch ($decoded_params["eventType"])
    {
        case "isConnected":
            print "Is QD connected? ".$decoded_params["isConnected"];
            file_put_contents("qdstate.txt","Is QD connected? ".$decoded_params["isConnected"]);
            break;
        case "isMute":
            print "Is Muted? ".$decoded_params["isMute"];
            file_put_contents("mutestate.txt","Is Muted? ".$decoded_params["isMute"]);
            break;       
    }
}

print "</p>-------------------------";
?>
</body>
</html>