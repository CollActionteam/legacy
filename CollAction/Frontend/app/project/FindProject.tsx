import { ProjectFilter, IProjectFilter } from "./ProjectFilter";
import ProjectList from "./ProjectList";
import * as React from "react";
import { IProject } from "./ProjectList";


interface IFindProjectState {
  projectList: IProject[];
  projectFilterState: IProjectFilter;
  projectFetching: boolean;
  projectFetchError: any;

}

export default class FindProject extends React.Component<null, IFindProjectState> {
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

  async fetchProjects(projectFilter: IProjectFilter) {
    this.setState({ projectFetching: true });
    try {
      const url: string = `/api/projects/find?categoryId=${projectFilter.categoryId}&statusId=${this.state.projectFilterState.statusId}`;
      const searchProjectRequest: Request = new Request(url);
      const fetchResult: Response = await fetch(searchProjectRequest);
      const jsonResponse = await fetchResult.json();
      this.setState({ projectFetching: false, projectList: jsonResponse });
    } catch (e) {
      this.setState({ projectFetching: false, projectFetchError: e });
    }
  }

  onChange (newState: IProjectFilter) {
    this.setState({ projectFilterState: newState });
    this.fetchProjects(newState);
  }

  render () {
    return (
      <div id="find-project">
        <ProjectFilter onChange={(searchState: IProjectFilter) => this.onChange(searchState) }/>
        <ProjectList projectList={this.state.projectList} />
      </div>
    );
  }
}
