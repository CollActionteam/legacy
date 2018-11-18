import "./home";
import "./project";
import "./global";
import "./account";
import "./admin";
import "./manage";

import "whatwg-fetch";
import * as Promise from "bluebird";
import * as jQuery from "jquery";

Promise.config({
  longStackTraces: true,
  warnings: true
});

import "@fortawesome/fontawesome-free/css/all.css";
import "@fortawesome/fontawesome-free/js/all.js";
import { faTwitter } from "@fortawesome/free-brands-svg-icons";
import { faFacebook } from "@fortawesome/free-brands-svg-icons";
import { faLinkedin } from "@fortawesome/free-brands-svg-icons";
import { dom, library } from "@fortawesome/fontawesome-svg-core";

library.add(faTwitter, faFacebook, faLinkedin);
dom.i2svg();

window["jQuery"] = jQuery;

function displayBodyOnLoad() {
  document.getElementById("body").style.display = "block";
}

window.addEventListener("load", displayBodyOnLoad);
