import * as React from "react";
import renderComponentIf from "./renderComponentIf";
import { library } from "@fortawesome/fontawesome-svg-core";
import { faEnvelope } from "@fortawesome/free-solid-svg-icons";
import * as classnames from "classnames";

library.add(faEnvelope);

enum States { VOID, ERROR, READY, LOADING }

interface NewsLetterSignupProps {
  mailchimpUser: string;
  mailChimpListId: string;
}

interface NewsLetterSignupState {
  subscriptionState: States;
  email?: string;
  signupStatusMessage?: string;
}

class NewsletterSignup extends React.Component<NewsLetterSignupProps, NewsLetterSignupState> {
  constructor(props) {
    super(props);

    this.state = {
      email: "",
      subscriptionState: States.VOID
    };

    this.handleEmailChange = this.handleEmailChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.getFormContainerClassNames = this.getFormContainerClassNames.bind(this);
  }

  componentDidMount() {}

  getFormContainerClassNames() {
    return classnames(
      "newsletter-signup__container",
      {
        "loading": this.state.subscriptionState === States.LOADING,
        "hidden": this.state.subscriptionState === States.READY,
      }
    );
  }

  getStatusMessageClassNames() {
    return classnames(
      "newsletter-signup__message",
      {
        "show-error" : this.state.subscriptionState === States.ERROR,
        "show-success" : this.state.subscriptionState === States.READY
      }
    );
  }

  handleEmailChange(event) {
    this.setState({
      email: event.target.value,
      subscriptionState: States.VOID
    });
  }

  isValidEmail() {
    return /@/.test(this.state.email);
  }

  handleSubmit(event) {
    event.preventDefault();

    // disable submit if email is invalid
    if (!this.isValidEmail()) {
      this.setState({
        subscriptionState: States.ERROR,
        signupStatusMessage: "Please enter a valid email address"
      });

      return;
    }

    this.setState({ subscriptionState: States.LOADING });

    fetch("http://collaction.us14.list-manage.com/subscribe/post-json", {
      method: "GET",
      headers: new Headers({
        "dataType" : "json",
        "contentType" : "application/json; charset=utf-8"
      }),
      body: JSON.stringify({
        MERGE0 : this.state.email,
        u : this.props.mailchimpUser,
        id: this.props.mailChimpListId
      }),
    })
    .then(response => {
      if (response.ok) {
        this.showSignupSucces();
      } else {
        this.showSignupError(response.statusText);
      }
    })
    .catch(() => this.showSignupError("E-mail signup could not be completed."));
  }

  showSignupSucces() {
    this.setState({
      subscriptionState: States.READY,
      signupStatusMessage: "Thank you for subscribing to our newsletter!"
    });
  }

  showSignupError(errorMessage: string) {
    this.setState({
      subscriptionState: States.ERROR,
      signupStatusMessage: errorMessage
    });

    throw new Error(errorMessage);
  }

  render() {
    return (
      <form onSubmit={this.handleSubmit}>
        <div className={this.getFormContainerClassNames()}>
          <input type="email" className="newsletter-signup__input" placeholder="Enter your email" value={this.state.email} onChange={this.handleEmailChange} />
          <input type="submit" className="newsletter-signup__submit" value="Sign Up" />
        </div>

        <div className={this.getStatusMessageClassNames()}>
          {this.state.signupStatusMessage}
        </div>
      </form>
    );
  }
}

renderComponentIf(
  <NewsletterSignup mailchimpUser={document.querySelector(".newsletter-signup") && document.querySelector(".newsletter-signup").getAttribute("data-u")}
                    mailChimpListId={document.querySelector(".newsletter-signup") && document.querySelector(".newsletter-signup").getAttribute("data-list-id")}
  />,
  document.querySelector(".newsletter-signup")
);
