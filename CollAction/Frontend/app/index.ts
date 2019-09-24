import "./home";
import "./project";
import "./global";
import "./account";
import "./admin";
import "./manage";
import "./donation";

function displayBodyOnLoad() {
  document.getElementById("body").style.display = "block";
}

window.addEventListener("load", displayBodyOnLoad);
