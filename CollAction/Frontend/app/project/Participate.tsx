import * as React from "react";

import renderComponentIf from "../global/renderComponentIf";

interface IParticipationProps {
    formId: string;
    email: string;
    projectActive: boolean;
    projectStatus: string;
    commitUrl: string;
}

interface IParticipationState {
    error: string;
    email: string;
}

export default class ParticipateInProject extends React.Component<IParticipationProps, IParticipationState> {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            email: null
        };
    }

    displayContactEmail = () => (
        <div>
            <p>By clicking ‘Take Part’ you’re signing up with {this.props.email}.</p>
        </div>
    )

    displayError = () => {
        if (this.state.error !== null) {
            return <div className="error">
                <p>{this.state.error}</p>
            </div>;
        }
    }

    setEmail = (event: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({ email: event.currentTarget.value });
    }

    askContactEmail = () => (
        <div>
            <p>Want to participate? Enter your e-mail address and click 'Take Part'!</p>
            {this.displayError()}
            <input className="form-control" name="email" type="email" placeholder="Your e-mail address" onChange={(event) => this.setEmail(event)}></input>
        </div>
    )

    submitForm = () => {
        if (!this.props.email && !this.validEmail()) {
            this.setState({ error: "Please provide a valid e-mail address" });
            return;
        }

        const form = document.getElementById(this.props.formId) as HTMLFormElement;
        if (form) {
            form.submit();
        }
    }

    private validEmail = () => this.state.email && this.state.email.match(/^\S+@\S+\.\S+$/);

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
                <span className="take-part-button disable-commit">{caption}</span>
            );
        }

        return (
            <div>
                <div className="participant-information">
                    {this.props.email
                        ? this.displayContactEmail()
                        : this.askContactEmail()}
                </div>
                <p>
                    <a href="javascript:void(0)" onClick={() => this.submitForm()} className="take-part-button">Take part</a>
                </p>
            </div>
        );
    }
}

const infoCard = document.getElementById("participate-in-projectinfocard");
renderComponentIf(
    <ParticipateInProject
        formId={infoCard && infoCard.dataset.formId}
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
        formId={details && details.dataset.formId}
        email={details && details.dataset.email}
        commitUrl={details && details.dataset.commitUrl}
        projectActive={details && details.dataset.projectActive !== "False"}
        projectStatus={details && details.dataset.projectStatus}
    ></ParticipateInProject>,
    details
);
