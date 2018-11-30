import * as React from "react";
import ProjectList, { IProject } from "../project/ProjectList";
import renderComponentIf from "../global/renderComponentIf";

interface IParticipatingInProjectsProps {
}

interface IParticipatingInProjectsState {
  projectList: IProject[];
  projectFetching: boolean;
  projectFetchError: any;
}

export default class ParticipatingInProjects extends React.Component<IParticipatingInProjectsProps, IParticipatingInProjectsState> {
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

  async fetchProjects() {
    this.setState({ projectFetching: true });

    const getUrl: () => string = () => {
      return "/api/manage/participating";
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

  render () {
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