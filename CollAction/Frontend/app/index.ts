import "./home";
import "./project";
import "./global";
import "./account";
import "./admin";
import "./manage";
import "./donation";

import "whatwg-fetch";

import * as Promise from "bluebird";

// Configure
Promise.config({
    longStackTraces: true,
    warnings: true // note, run node with --trace-warnings to see full stack traces for warnings
});

function displayBodyOnLoad() {
  document.getElementById("body").style.display = "block";
}

window.addEventListener("load", displayBodyOnLoad);
