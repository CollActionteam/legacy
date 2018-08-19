import * as React from "react";

export interface IProject {
  projectId: string,
  projectName: string;
  projectUriPart: string;
  projectProposal: string;
  categoryName: string;
  categoryColorHex: string;
  locationName: string;
  remainingTime: number;
  bannerImagePath: string;
  target: number;
  participants: number;
  progressPercent: number;
  status: string;
}

class ProjectThumb extends React.Component<IProject, null> {
  render () {
    const projectImageStyle = {
      backgroundImage: `url(${this.props.bannerImagePath})`,
    };

    const link = `/projects/${encodeURIComponent(this.props.projectUriPart)}/${this.props.projectId}/details`;

    return (
      <div className="col-xs-12 col-md-4 project-thumb-container">
        <div className="project-thumb">
          <div className="project-thumb-image" style={projectImageStyle} >
            <div className="category-name" style={{backgroundColor: "#" + this.props.categoryColorHex}}>
              {this.props.categoryName}
            </div>
          </div>
          <div className="project-thumb-body">
            <strong>{this.props.projectName}</strong>
            <p>{this.props.projectProposal}</p>
          </div>
          <div className="project-thumb-about">
            <div>{this.props.locationName}</div>
            <div>{this.props.progressPercent}%</div>
          </div>
          <div className="project-thumb-stats">
            <div>
              <div className="value">{this.props.remainingTime}</div>
              <div className="label">to go</div>
            </div>
            <div>
              <div className="value">{this.props.participants}</div>
              <div className="label">Participants</div>
            </div>
            <div>
              <div className="value">{this.props.target}</div>
              <div className="label" >Target</div>
            </div>
          </div>
          <div className="project-thumb-button">
            <a href={link} style={{backgroundColor: "#" + this.props.categoryColorHex}}>Read More</a>
          </div>
        </div>
      </div>
    );
  }
}

export default ({ projectList }) => {
  return (
    <div id="project-list">
      <div className="container">
        { projectList.map(project => <ProjectThumb key={project.projectId} {...project} />) }
      </div>
    </div>
  );
}
