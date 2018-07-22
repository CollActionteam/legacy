import * as React from "react";
import FindProject from "./FindProject";

import "./styles/FindProject.scss";
import "./styles/ProjectDetails.scss";
import "./styles/ProjectCreate.scss";
import "./styles/ProjectCommit.scss";
import "./styles/ProjectThankYouCommit.scss";
import "./styles/ProjectSendEmail.scss";

import "./PieChart";
import "./Carousel";

import renderComponentIf from "../global/renderComponentIf";

renderComponentIf(
  <FindProject controller={true} />,
  document.getElementById("project-controller")
);

renderComponentIf(
  <FindProject controller={false} />,
  document.getElementById("projects-container")
);

renderComponentIf(
  <FindProject controller={false} projectId={getQueryString("projectId") } />,
  document.getElementById("embedded-projects-container")
);

function getQueryString(key: string): number {
  let value = decodeURIComponent(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
  return parseInt(value);
}
