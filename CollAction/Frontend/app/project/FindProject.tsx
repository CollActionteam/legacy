import { ProjectFilter, IProjectFilter } from "./ProjectFilter";
import ProjectList from "./ProjectList";
import * as React from "react";
import { IProject } from "./ProjectList";

interface IFindProjectProps {
  controller: boolean;
}

interface IFindProjectState {
  projectList: IProject[];
  projectFilterState: IProjectFilter;
  projectFetching: boolean;
  projectFetchError: any;
}

export default class FindProject extends React.Component<IFindProjectProps, IFindProjectState> {
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

  async fetchProjects(projectFilter: IProjectFilter = null) {
    this.setState({ projectFetching: true });

    // Fetch projects with out filters set
    const getUrl: () => string = () => {
      if (projectFilter) {
        return `/api/projects/find?categoryId=${projectFilter.categoryId}&statusId=${projectFilter.statusId}`;
      }
      return "/api/projects/find";
    };

    try {
      const searchProjectRequest: Request = new Request(getUrl());
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