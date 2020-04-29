import React from "react";
import { Alert } from "../Alert/Alert";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

import styles from "./NewsletterSignup.module.scss";

interface INewsletterSignupState {
  email: string;
  error: string;
}

interface INewsletterSignupProps {
  mailchimpListId: string;
}

export default class NewsletterSignup extends React.Component<
  INewsletterSignupProps,
  INewsletterSignupState
> {
  constructor(props: INewsletterSignupProps) {
    super(props);
    this.state = {
      email: "",
      error: "",
    };
    this.handleChange = this.handleChange.bind(this);
    this.handleKeyUp = this.handleKeyUp.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  handleChange(event: any) {
    this.setState({
      email: event.target.value,
      error: "",
    });
  }

  handleKeyUp(event: any) {
    if (event.keyCode === 13) {
      return this.handleSubmit(event);
    }
  }

  handleSubmit(event: any) {
    if (this.state.error) {
      event.preventDefault();
    }
  }

  render() {
    return (
      <form
        action={`//collaction.us14.list-manage.com/subscribe/post?u=48e9b2f8f522cf59b9d5ffa8d&amp;id=${this.props.mailchimpListId}`}
        id="mc-embedded-subscribe-form"
        name="mc-embedded-subscribe-form"
        onSubmit={this.handleSubmit}
        onKeyUp={this.handleKeyUp}
      >
        {this.state.error ? (
          <Alert type="error" text={this.state.error} />
        ) : null}
        <div className={styles.wrapper}>
          <input
            type="email"
            placeholder="Fill in your e-mail"
            name="EMAIL"
            id="mce-EMAIL"
            value={this.state.email}
            onFocus={this.handleChange}
            onChange={this.handleChange}
          />
          <div className={styles.hidden} aria-hidden="true">
            <input
              type="text"
              place-holder="e-Mail"
              name="b_48e9b2f8f522cf59b9d5ffa8d_@NewsletterSubscriptionServiceOptions.Value.MailChimpNewsletterListId"
              value=""
              readOnly={true}
            ></input>
          </div>
          <button
            name="subscribe"
            id="mc-embedded-subscribe"
            aria-label="Subscribe Newsletter E-Mail"
            className={styles.submit}
            disabled={!!this.state.error || !this.state.email}
          >
            <FontAwesomeIcon icon={["far", "envelope"]} size="sm" />
            {this.state.email ? (
              <span className={styles.submitLabel}>Sign up</span>
            ) : null}
          </button>
        </div>
      </form>
    );
  }
}
