import "./components/home";
import "./components/project";
import "./components/global";
import "./components/account";
import "./components/admin";
import "./components/manage";

// New ITCSS architecture
import "./style";

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
