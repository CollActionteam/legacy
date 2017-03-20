import { ProjectFilter, IProjectFilterState } from "./ProjectFilter";
import ProjectList from "./ProjectList";
import * as React from "react";

export default class FindProject extends React.Component<null, {}> {

  onChange (currentState: IProjectFilterState) {
    console.log(currentState);
  }

  render () {
    return (
      <div id="find-project">
        <ProjectFilter onChange={(searchState: IProjectFilterState) => this.onChange(searchState) }/>
        <ProjectList />
      </div>
    );
  }
}