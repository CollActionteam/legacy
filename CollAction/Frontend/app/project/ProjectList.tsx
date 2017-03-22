import * as React from "react";

export interface IProject {
  projectId: string,
  projectName: string;
  projectProposal: string;
  categoryName: string;
  categoryColorHex: string;
  locationName: string;
  remainingTime: number;
  bannerImagePath: string;
  target: number;
  participants: number;
  progressPercent: number;
  statusText: string;
  statusStubText: string;
}

class ProjectThumb extends React.Component<IProject, null> {
  render () {
    return (
      <div className="col-xs-12 col-md-3">
        Project
      </div>
    );
  }
}

export default ({ projectList }) => {
  return (
    <div id="project-list">
      <div className="container">
        { projectList.map(project => <ProjectThumb {...project} />) }
      </div>
    </div>
  );
}
