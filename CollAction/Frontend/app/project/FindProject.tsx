import * as React from "react";
import ProjectList from "./ProjectList";
import { IProjectsProps, IProjectsState, Projects} from "../global/Projects";
import { ProjectFilter, IProjectFilter } from "./ProjectFilter";
import renderComponentIf from "../global/renderComponentIf";

interface IFindProjectProps extends IProjectsProps {
  controller: boolean;
}

interface IFindProjectState extends IProjectsState {
  projectFilterState: IProjectFilter;
}

export default class FindProject extends Projects<IFindProjectProps, IFindProjectState> {

  constructor (props) {
    super(props);
    const projectList = [];

    this.state = {
      projectList,
      projectFilterState: null,
      projectFetching: false,
      projectFetchError: null,
    };
  }

  componentDidMount() {
    if (this.props.controller === false) {
      this.fetchProjects();
    }
  }

  projectsUrl() {
    if (this.state.projectFilterState) {
      return `/api/projects/find?categoryId=${this.state.projectFilterState.categoryId}&statusId=${this.state.projectFilterState.statusId}`;
    }
    return "/api/projects/find";
  }

  onChange (newState: IProjectFilter) {
    this.setState(
      { projectFilterState: newState },
      () => this.fetchProjects());
  }

  render() {
    const controller = <ProjectFilter onChange={(searchState: IProjectFilter) => this.onChange(searchState) }/>;
    return (
      <div id="find-project">
        { this.props.controller ?  controller : null }
        <div className="container">
          <ProjectList projectList={this.state.projectList} tileClassName="col-xs-12 col-md-4" />
        </div>
      </div>
    );
  }
}

renderComponentIf(
  <FindProject controller={true} />,
  document.getElementById("project-controller")
);

renderComponentIf(
  <FindProject controller={false} />,
  document.getElementById("projects-container")
);