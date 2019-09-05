import * as React from "react";
import ProjectList from "../project/ProjectList";
import { IProjectsProps, IProjectsState, Projects} from "../global/Projects";
import renderComponentIf from "../global/renderComponentIf";

export default class MyProjects extends Projects<IProjectsProps, IProjectsState> {
  constructor (props) {
    super(props);
    const projectList = [];

    this.state = {
      projectList,
      projectFetching: false,
      projectFetchError: null,
      allProjectsFetched: false
    };
  }

  componentDidMount() {
    this.fetchProjects();
  }

  projectsUrl(): string {
    return "/api/manage/myprojects";
  }

  render() {
    if (this.state.projectFetching) {
      return (
        <h2>
          <span className="busy-indicator">
            Loading your projects<span>.</span><span>.</span><span>.</span>
          </span>
        </h2>
      );
    }

    if (this.state.projectList.length > 0) {
      return (
        <div>
          <h2>Projects you've created</h2>
          <ProjectList projectList={this.state.projectList} tileClassName="col-xs-12 col-md-6" isEmbedded={false} />
        </div>
      );
    }
    else {
      return (
        <div>
          <h2>Projects you've created</h2>
          <div className="kickstart-card">
              <div className="row">
                  <div className="col-xs-10">
                      <h2>You haven't created any projects yet.</h2>
                  </div>
                  <div className="col-xs-2">
                      <h2><object className="logo pull-right" type="image/svg+xml" data="/images/step3.svg"></object></h2>
                  </div>
                  <div className="col-xs-12">
                      <p>
                        Would you like to do something about social or environmental problems? Would you like to lead the crowdacting movement? Start a project on CollAction now!
                      </p>
                      <a className="btn" href="/start">Start a project</a>
                  </div>
              </div>
            </div>
        </div>
      );
    }
  }
}

renderComponentIf(
    <MyProjects></MyProjects>,
    document.getElementById("my-projects")
);