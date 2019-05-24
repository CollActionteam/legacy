import * as React from "react";
import Collapsible from "react-collapsible";

import renderComponentIf from "../global/renderComponentIf";

interface IParticipationProps {
  formId: string;
  email: string;
  projectActive: boolean;
  projectStatus: string;
  commitUrl: string;
}

export default class ParticipateInProject extends React.Component<IParticipationProps> {
  constructor(props) {
    super(props);
  }

  displayContactEmail = () => (
    <div>
      <p>We'll contact you on this e-mail address:</p>
      <b>{ this.props.email }</b>
    </div>
  )

  askContactEmail = () => (
    <div>
      <p>Please enter your e-mail address so we can contact you with more information on the project:</p>
      <input className="form-control" name="email" type="email" placeholder="Your e-mail address"></input>
    </div>
  )

  submitForm = () => {
    const form = document.getElementById(this.props.formId) as HTMLFormElement;
    if (form) {
      form.submit();
    }
  }

  render() {
    let caption = "Participate!";
    if (this.props.projectStatus === "ComingSoon") {
      caption = "Coming soon";
    }
    else if (this.props.projectStatus === "Closed") {
      caption = "Project is finished";
    }

    if (!this.props.projectActive) {
      return (
        <span className="take-part-button disable-commit">{ caption }</span>
      );
    }

    return (
      <Collapsible
        transitionTime={300}
        trigger={<a href="javascript:void(0)" className="take-part-button">{ caption }</a>}
      >
        <div className="participant-information">
          <h2>That's awesome!</h2>
          { this.props.email
            ? this.displayContactEmail()
            : this.askContactEmail() }
        </div>
        <p>
          <a href="javascript:void(0)" onClick={ () => this.submitForm() } className="take-part-button">Take part</a>
        </p>
      </Collapsible>
    );
  }
}

const infoCard = document.getElementById("participate-in-projectinfocard");
renderComponentIf(
    <ParticipateInProject
      formId={infoCard && infoCard.dataset.formId }
      email={infoCard && infoCard.dataset.email}
      commitUrl={infoCard && infoCard.dataset.commitUrl}
      projectActive={infoCard && infoCard.dataset.projectActive !== "False"}
      projectStatus={infoCard && infoCard.dataset.projectStatus}
    ></ParticipateInProject>,
    infoCard
);

const details = document.getElementById("participate-in-projectdetails");
renderComponentIf(
  <ParticipateInProject
    formId={details && details.dataset.formId }
    email={details && details.dataset.email}
    commitUrl={details && details.dataset.commitUrl}
    projectActive={details && details.dataset.projectActive !== "False"}
    projectStatus={details && details.dataset.projectStatus}
  ></ParticipateInProject>,
  details
);
