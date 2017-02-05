import * as React from "react";
import * as ReactDOM from "react-dom";
import "./style.scss";
import ProjectFilter from "./ProjectFilter";
import ProjectList from "./ProjectList";

export default class FindProject extends React.Component<null, {}> {
  render () {
    return (
      <div id="find-project">
        <ProjectFilter />
        <ProjectList />
      </div>
    );
  }
}

function renderComponentIf(component, element) {
  if (element) {
    ReactDOM.render(component, element);
  }
}

renderComponentIf(<FindProject />, document.getElementById("project-controller"));