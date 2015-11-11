<?php

// slightly altered function xml_highlight from comments at: http://php.net/manual/en/function.highlight-string.php by Dmitry S.
function xml_highlight($s) {        
    $s = htmlspecialchars($s);
    $s = preg_replace("#&lt;([/]*?)(.*)([\s]*?)&gt;#sU",
        "<font color=\"#0000FF\">&lt;\\1\\2\\3&gt;</font>",$s);
    $s = preg_replace("#&lt;([\?])(.*)([\?])&gt;#sU",
        "<font color=\"#800000\">&lt;\\1\\2\\3&gt;</font>",$s);
    $s = preg_replace("#&lt;([^\s\?/=])(.*)([\[\s/]|&gt;)#iU", //the xml opening elements
        "&lt;<font color=\"#0000CC\"><strong>\\1\\2</strong></font>\\3",$s);
    $s = preg_replace("#&lt;([/])([^\s]*?)([\s\]]*?)&gt;#iU", //the xml closing elements
        "&lt;\\1<font color=\"#0000CC\"><strong>\\2</strong></font>\\3&gt;",$s);
    $s = preg_replace("#([^\s]*?)\=(&quot;|')(.*)(&quot;|')#isU", //the xml attributes and their values
        "<font color=\"#880000\"><strong>\\1</strong></font>=<font color=\"#006622\"><em>\\2\\3\\4</em></font>",$s);
    $s = preg_replace("#&lt;(.*)(\[)(.*)(\])&gt;#isU",
        "&lt;\\1<font color=\"#800080\">\\2\\3\\4</font>&gt;",$s);
    return nl2br($s);
	}
?>

<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8" />
<title>Polyhedron (and alike) examples</title>

<!-- showhide-content code from: http://www.cssnewbie.com/example/showhide-content/ -->
<script language="javascript" type="text/javascript">
function showHide(shID) {
   if (document.getElementById(shID)) {
      if (document.getElementById(shID+'-show').style.display != 'none') {
         document.getElementById(shID+'-show').style.display = 'none';
         document.getElementById(shID).style.display = 'block';
      	} 
      else {
         document.getElementById(shID+'-show').style.display = 'inline';
         document.getElementById(shID).style.display = 'none';
      	}}}
</script>

<style type="text/css">

td {vertical-align: top; }

   /* This CSS is used for the Show/Hide functionality, source: http://www.cssnewbie.com/example/showhide-content/ */
   .more {
      display: none;
      border-top: 1px solid #666;
      border-bottom: 1px solid #666; }
   a.showLink::before { content: "↓" }
   a.showLink::after{ content: "↓" }
   a.hideLink::before { content: "↑" }
   a.hideLink::after { content: "↑" }
   a.showLink, a.hideLink { 
      text-decoration: none;
      color: #36f;
      padding-left: 2px;
      background: transparent no-repeat left; }
   a.hideLink {
      background: transparent no-repeat left; }
   a.showLink:hover, a.hideLink:hover {
      border-bottom: 1px dotted #36f; }
</style>

</head>
<body>



<table>
	<tr><td>Filename:</td>
		<td>
		cube.scx
		<a href="#" id="cube-show" class="showLink" onclick="showHide('cube');return false;" >view source</a> 		
		<div id="cube" class="more">
			cube.scx-source
			<?php echo xml_highlight(file_get_contents("cube.scx", true)); ?>
			<hr />
			<a href="#" id="cube-hide" class="hideLink" onclick="showHide('cube');return true;">hide source</a>
      </div>
		</td></tr>
	<tr><td></td><td></td></tr>
	<tr><td></td><td></td></tr>
</table>

  <div id="wrap">
      <h1>Show/Hide Content</h1>
      <p>Go back to the main article. This example shows you how to create a show/hide container using a couple of links, a div, a few lines of CSS, 
      	and some JavaScript to manipulate our CSS. 
      		Just click on the "see more" link at the end of this paragraph to see the technique in action, and be sure to view the source to see how it all works together. 
      		<a href="#" id="example-show" class="showLink" onclick="showHide('example');return false;" >meer</a></p>
      <div id="example" class="more">
         <p>Congratulations! You've found the magic hidden text! Clicking the link below will hide this content again.</p>
         <p><a href="#" id="example-hide" class="hideLink" onclick="showHide('example');return false;">minder</a></p>
      </div>
   </div>



</body>
</html>