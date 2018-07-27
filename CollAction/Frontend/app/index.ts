import "./home/index";
import "./project/index";
import "./global/index";
import "./account/index";
import "./admin/index";
import "./manage/index";

import "whatwg-fetch";

import * as jQuery from "jquery";
import * as Promise from "bluebird";

// Let's Load jQuery in the window for the asp validations
window["jQuery"] = jQuery;

// Configure
Promise.config({
    longStackTraces: true,
    warnings: true // note, run node with --trace-warnings to see full stack traces for warnings
});

function displayBodyOnLoad() {
  document.getElementById("body").style.display = "block";
}

window.addEventListener("load", displayBodyOnLoad);
