window.collAction = window.collAction || {}

window.collAction.drawPieChart = function (percentComplete: number, canvas: HTMLCanvasElement): void {
  var ctx = canvas.getContext("2d");
  var lastend = 4.71239;
  var data = [percentComplete, 100 - percentComplete ]; // If you add more data values make sure you add more colors
  var myTotal = 0; // Automatically calculated so don't touch
  var myColor = ['#9bd8bc', '#23d844' ]; // Colors of each slice

  ctx.canvas.width = 100;
  ctx.canvas.height = 100;

  for (var e = 0; e < data.length; e++) {
    myTotal += data[e];
  }

  for (var i = 0; i < data.length; i++) {
    ctx.fillStyle = myColor[i];
    ctx.beginPath();
    ctx.moveTo(canvas.width / 2, canvas.height / 2);
    // Arc Parameters: x, y, radius, startingAngle (radians), endingAngle (radians), antiClockwise (boolean)
    ctx.arc(canvas.width / 2, canvas.height / 2, canvas.height / 2, lastend, lastend + (Math.PI * 2 * (data[i] / myTotal)), false);
    ctx.lineTo(canvas.width / 2, canvas.height / 2);
    ctx.fill();
    lastend += Math.PI * 2 * (data[i] / myTotal);
  }
}
