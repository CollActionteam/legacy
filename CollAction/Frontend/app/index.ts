import "./home";
import "./project";
import "./global";
import "./account";
import "./admin";
import "./manage";

import "whatwg-fetch";

import * as jQuery from "jquery";

// Let's Load jQuery in the window for the asp validations
window["jQuery"] = jQuery;

function displayBodyOnLoad() {
  document.getElementById("body").style.display = "block";
}

window.addEventListener("load", displayBodyOnLoad);
