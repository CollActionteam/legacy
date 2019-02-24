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
    if (this.state.projectFetching) {
      return (
        <h2>
          <span className="busy-indicator">
            Loading projects you're participating in<span>.</span><span>.</span><span>.</span>
          </span>
        </h2>
      );
    }

    if (this.state.projectList.length > 0) {
      return (
        <div>
          <h2>Projects you're participating in</h2>
          <ProjectList projectList={this.state.projectList} tileClassName="col-xs-12 col-md-6" />
        </div>
      );
    }
    else {
      return (
        <div>
          <h2>Projects you're participating in</h2>
          <div className="kickstart-card">
              <div className="row">
                  <div className="col-xs-10">
                      <h2>You are not yet participating.</h2>
                  </div>
                  <div className="col-xs-2">
                      <h2><object className="logo pull-right" type="image/svg+xml" data="/images/step3.svg"></object></h2>
                  </div>
                  <div className="col-xs-12">
                      <p>
                        Do you want to make the world a better place, but do you sometimes feel that your actions are only a drop in the ocean?
                      </p>
                      <p>
                        Well, not anymore, because we are introducing crowdacting: taking action knowing that you are one of many.
                        We revamp the neighbourhood with a hundred people or switch to renewable energy with thousands at the
                        same time. Would you like to do something about social or environmental problems?
                      </p>
                      <a className="btn" href="/find">Find a project</a>
                  </div>
              </div>
            </div>
        </div>
      );
    }
  }
}

renderComponentIf(
    <ParticipatingInProjects></ParticipatingInProjects>,
    document.getElementById("participating-in-projects")
);