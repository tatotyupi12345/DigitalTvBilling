// We need a reference to the element we want to insert the loader into
// This can be any div we get a reference to
var $loader = $("#loader");

var $startBtn = $("button.start");
var $scaleBtn = $("button.scale");
var $stopBtn = $("button.stop");

var handleStartClick = function() {
  // We start the spinner with <element>.gSpinner()
  $loader.gSpinner();
};

var handleScaleClick = function() {
  // We can scale down the default size of the spinner by passing an options object
  // where we specify the scale of the spinner relative to it's default size, 200x200px
  $loader.gSpinner({ scale: .5 });
};

var handleStopClick = function() {
  // We hide the spinner with <element>.gSpinner("hide")
  $loader.gSpinner("hide");
};

$startBtn.click(handleStartClick);
$scaleBtn.click(handleScaleClick);
$stopBtn.click(handleStopClick);

