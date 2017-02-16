import ProjectFilter from "./ProjectFilter";
import ProjectList from "./ProjectList";
import * as React from "react";

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