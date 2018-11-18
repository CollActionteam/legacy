import "./home";
import "./project";
import "./global";
import "./account";
import "./admin";
import "./manage";

import "whatwg-fetch";

import * as jQuery from "jquery";

import "@fortawesome/fontawesome-free/css/all.css";
import "@fortawesome/fontawesome-free/js/all.js";
import { faTwitter } from "@fortawesome/free-brands-svg-icons";
import { faFacebook } from "@fortawesome/free-brands-svg-icons";
import { faLinkedin } from "@fortawesome/free-brands-svg-icons";
import { dom, library } from "@fortawesome/fontawesome-svg-core";

// after library.add
library.add(faTwitter, faFacebook, faLinkedin);
dom.i2svg();

// Let's Load jQuery in the window for the asp validations
window["jQuery"] = jQuery;

function displayBodyOnLoad() {
  document.getElementById("body").style.display = "block";
}

window.addEventListener("load", displayBodyOnLoad);
