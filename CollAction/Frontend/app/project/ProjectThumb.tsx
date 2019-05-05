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
  subscribedToEmails: boolean | null;
  canSendProjectEmail: boolean | null;

  tileClassName: string;
}

interface IThumbState {
  busy: boolean;
}

export default class ProjectThumb extends React.Component<IProject, IThumbState> {
  constructor(props: IProject) {
    super(props);

    this.state = {
      busy: false
    };

    this.toggleSubscription = this.toggleSubscription.bind(this);
  }

  async toggleSubscription() {
    this.setState({busy: true});

    let formData = new FormData();
    formData.append("projectId", this.props.projectId);

    const request = new Request("/Manage/ToggleEmailSubscription", { method: "POST", body: formData});
    const verificationToken = document.getElementById("RequestVerificationToken") as HTMLInputElement;
    if (verificationToken) {
      request.headers.set("RequestVerificationToken", verificationToken.value);
    }

    try {
      await fetch(request);
      location.reload();
    }
    catch (e) {
      this.setState({busy: false});
    }
  }

  renderSubscriptionButton() {
    if (this.props.subscribedToEmails === null) {
      return;
    }

    if (!this.state.busy) {
      return (
        <div className="email-subscription">
          <a  href="javascript:void(0)"
              onClick={this.toggleSubscription}
              className={`btn-${this.props.subscribedToEmails ? "unsubscribe" : "subscribe"}`}>
            {this.props.subscribedToEmails ? "Unsubscribe from news" : "Subscribe to news"}
          </a>
        </div>
      );
    }
    else {
      return (
        <div className="email-subscription">
          <div className="busy-indicator">
            {this.props.subscribedToEmails ? "Unsubscribing..." : "Subscribing..."}
          </div>
        </div>
      );
    }
  }

  renderMailToParticipantsButton() {
    if (this.props.canSendProjectEmail === null) {
      return;
    }
    else if (this.props.canSendProjectEmail === true) {
      return (
        <div className="mail-to-participants">
            <a href={`/Projects/SendProjectEmail/${this.props.projectId}`} className="btn-send-email">
              Send project e-mail
            </a>
        </div>
      );
    }
    else {
      return (
        <div className="mail-to-participants not-available">
          <span>Project e-mails no longer available</span>
        </div>
      );
    }
  }

  openProject = () => {
    window.location.href = `/projects/${this.props.projectNameUriPart}/${this.props.projectId}/details`;
  }
  render () {
    const projectImageStyle = {
      backgroundImage: `url(${this.props.bannerImagePath})`,
    };

    const link = `/projects/${this.props.projectNameUriPart}/${this.props.projectId}/details`;

    return (
      <div>
        <div className={`${this.props.tileClassName} project-thumb-container`}>
          <div className="project-thumb">
            <div className="project-thumb-image" style={projectImageStyle} onClick={this.openProject}>
              <div className="category-name" style={{backgroundColor: `#${this.props.categoryColorHex}`}}>
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
              <a href={link} style={{backgroundColor: `#${this.props.categoryColorHex}`}}>Read More</a>
            </div>
          </div>

          {this.renderSubscriptionButton()}
          {this.renderMailToParticipantsButton()}
        </div>
      </div>
    );
  }
}
