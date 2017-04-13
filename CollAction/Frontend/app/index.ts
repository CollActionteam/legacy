import "./global/bootstrap.css";
import "./global/dotbackground.scss";
import "./project/styles/index.scss";
import "./admin/styles/index.scss";
import "./forms/style.scss";
import "./global/body.scss";
import "./header/style.scss";
import "./footer/style.scss";
import "./home/index.ts";
import "./project/index.tsx";
import "./global/index";

import * as jQuery from "jquery";
import * as injectTapEventPlugin from "react-tap-event-plugin";

// Let's Load jQuery in the window for the asp validations
window["jQuery"] = jQuery;

// Needed for onTouchTap.
// http://stackoverflow.com/a/34015469/988941 
injectTapEventPlugin();
