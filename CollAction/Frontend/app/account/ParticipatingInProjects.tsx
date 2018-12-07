import * as React from "react";
import ProjectList from "../project/ProjectList";
import { IProjectsProps, IProjectsState, Projects} from "../global/Projects";
import renderComponentIf from "../global/renderComponentIf";

export default class ParticipatingInProjects extends Projects<IProjectsProps, IProjectsState> {
  constructor (props) {
    super(props);
    const projectList = [];

    this.state = {
      projectList,
      projectFetching: false,
      projectFetchError: null,
    };
  }

  componentDidMount() {
    this.fetchProjects();
  }

  projectsUrl(): string {
    return "/api/manage/participating";
  }

  render() {
    return (
      <div>
        <h2 hidden={this.state.projectFetching}>Projects you're participating in</h2>
        <ProjectList projectList={this.state.projectList} tileClassName="col-xs-12 col-md-6" />
      </div>
    );
  }
}

renderComponentIf(
    <ParticipatingInProjects></ParticipatingInProjects>,
    document.getElementById("participating-in-projects")
);