import * as React from "react";

export interface IProject {
  projectId: string;
  projectName: string;
  projectNameUriPart: string;
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
  subscribedToEmails?: boolean;

  tileClassName: string;
}

class ProjectThumb extends React.Component<IProject, null> {
  render () {
    const projectImageStyle = {
      backgroundImage: `url(${this.props.bannerImagePath})`,
    };

    const link = `/projects/${this.props.projectNameUriPart}/${this.props.projectId}/details`;

    return (
      <div>
        <div className={this.props.tileClassName + " project-thumb-container"}>
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
          { this.props.subscribedToEmails !== undefined &&
            <div className="email-subscription">
              <form action="/Manage/ToggleEmailSubscription" method="post">
                <input type="hidden" name="projectId" value={this.props.projectId} />
                <input type="submit" className={"btn " + (this.props.subscribedToEmails ? "unsubscribe" : "subscribe")} value={this.props.subscribedToEmails ? "Unsubscribe from news" : "Subscribe to news"} />
              </form>
            </div>
          }
        </div>
      </div>
    );
  }
}

export default ({ projectList, tileClassName }) => {
  return (
    <div id="project-list">
        { projectList.map(project => <ProjectThumb key={project.projectId} tileClassName={tileClassName} {...project} />) }
    </div>
  );
};