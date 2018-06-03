import "./home/index";
import "./project/index";
import "./global/index";
import "./account/index";
import "./admin/index";
import "./manage/index";

import "whatwg-fetch";
import "quill";

import * as jQuery from "jquery";
import * as injectTapEventPlugin from "react-tap-event-plugin";

// Let's Load jQuery in the window for the asp validations
window["jQuery"] = jQuery;

// Needed for onTouchTap.
// http://stackoverflow.com/a/34015469/988941 
injectTapEventPlugin();

function displayBodyOnLoad() {
  document.getElementById("body").style.display = "block";
}

window.addEventListener("load", displayBodyOnLoad);
