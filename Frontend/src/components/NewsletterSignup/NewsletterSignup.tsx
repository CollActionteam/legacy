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
  mailchimpUserId: string;
  mailchimpServer: string;
  mailchimpAccount: string;
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
        action={`//${this.props.mailchimpAccount}.${this.props.mailchimpServer}.list-manage.com/subscribe/post?u=${this.props.mailchimpUserId}&id=${this.props.mailchimpListId}`}
        id="mc-embedded-subscribe-form"
        name="mc-embedded-subscribe-form"
        onSubmit={this.handleSubmit}
        onKeyUp={this.handleKeyUp}
      >
        <Alert type="error" text={this.state.error} />
        <div className={styles.wrapper}>
          <input
            type="email"
            placeholder="Fill in your e-mail"
            aria-label="E-Mail Address"
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
              name={`b_${this.props.mailchimpUserId}_${this.props.mailchimpListId}`}
              value=""
              tabIndex={-1}
              readOnly={true}
            ></input>
          </div>
          <button
            name="subscribe"
            id="mc-embedded-subscribe"
            aria-label="Subscribe Newsletter"
            className={styles.submit}
            disabled={!!this.state.error || !this.state.email}
          >
            <div className={styles.submitIcon}>
              <FontAwesomeIcon icon={["far", "envelope"]} size="sm" />
            </div>
            {this.state.email ? (
              <span className={styles.submitLabel}>Sign up</span>
            ) : null}
          </button>
        </div>
      </form>
    );
  }
}
