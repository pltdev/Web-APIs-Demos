<?php
$qd_state = file_get_contents("qdstate.txt");
$mute_state = file_get_contents("mutestate.txt");
print "<p>".$qd_state."</p>";
print "<p>".$mute_state."</p>";
?>