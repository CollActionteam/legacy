import { ProjectFilter, IProjectFilterState } from "./ProjectFilter";
import ProjectList from "./ProjectList";
import * as React from "react";
import { IProject } from "./ProjectList";

interface IFindProjectState {
  projectList: IProject[];
  projectFilterState: IProjectFilterState;
  projectFetching: boolean;
  projectFetchError: any;
}

export default class FindProject extends React.Component<null, IFindProjectState> {
  constructor (props) {
    super(props);
    const projectList = [];
    const projectFilterState:IProjectFilterState = {
      filter: '1',
      status: '2',
      location: '3',
    };

    this.state = {
      projectList,
      projectFilterState,
      projectFetching: false,
      projectFetchError: null,
    };
  }

  async fetchProjects() {
    this.setState({ projectFetching: true });
    try {
      const searchProjectRequest: Request = new Request("/Projects/Find");
      const fetchResult: Response = await fetch(searchProjectRequest);
      const jsonResponse = await fetchResult.json();
      this.setState({ projectFetching: false, projectList: jsonResponse });
    } catch (e) {
      this.setState({ projectFetching: false, projectFetchError: e });
    }
  }

  onChange (newState: IProjectFilterState) {
    this.setState({ projectFilterState: newState });
    this.fetchProjects();
  }

  render () {
    return (
      <div id="find-project">
        <ProjectFilter onChange={(searchState: IProjectFilterState) => this.onChange(searchState) }/>
        <ProjectList projectList={this.state.projectList} />
      </div>
    );
  }
}
