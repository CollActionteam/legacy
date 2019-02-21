import * as React from "react";
import renderComponentIf from "./renderComponentIf";
import { library } from "@fortawesome/fontawesome-svg-core";
import { faEnvelope } from "@fortawesome/free-solid-svg-icons";

library.add(faEnvelope);

enum States { ERROR, READY, LOADING }

interface NewsLetterSignupProps {
  signupUrl: string;
}

interface NewsLetterSignupState {
  subscriptionState: States;
  email?: string;
  signupError?: Error;
}

class NewsletterSignup extends React.Component<NewsLetterSignupProps, NewsLetterSignupState> {

  constructor(props) {
    super(props);
    this.state = { subscriptionState: States.LOADING };
  }

  componentDidMount() {
  }

  getSignupUrl() {
    if (this.props.signupUrl) {
      return encodeURIComponent(this.props.signupUrl);
    } else {
      return null;
    }
  }

  async post() {

    let formData = new FormData();
    formData.append("EMAIL", this.state.email);

    try {
      const apiLink: string = this.getSignupUrl();
      const request = new Request(apiLink, { method: "POST", body: formData});
      const response: Response = await fetch(apiLink);
      const parsed = await response.json();
      console.log(parsed);
      this.setState({ subscriptionState: States.READY });
    } catch (e) {
      this.setState({ subscriptionState: States.ERROR, signupError: e });
    }
  }

  render() {
    return (
        <div>
          <input type="email" className="newsletter-signup-form__input" placeholder="Enter your email" value="" />
          < input type="submit" className="button" value="Subscribe" />
        </div>
      );
    }
}

renderComponentIf(
  <NewsletterSignup signupUrl={document.querySelector(".newsletter-signup-form") && document.querySelector(".newsletter-signup-form").getAttribute("action")}
  />,
  document.getElementById("newsletter-signup-form")
);
