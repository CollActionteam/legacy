import React from "react";
import { Alert } from "../Alert";
import styles from "./style.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export default class NewsletterSignup extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      email: "",
      error: "",
    };
    this.handleChange = this.handleChange.bind(this);
    this.handleKeyUp = this.handleKeyUp.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidMount() {}

  handleChange(event) {
    this.setState({
      email: event.target.value,
      error: null,
    });
  }

  handleKeyUp(event) {
    if (event.keyCode === 13) return this.submitForm();
  }

  handleSubmit(event) {
    event.preventDefault();
    this.submitForm();
  }

  submitForm() {
    console.log("Submitted");
    this.setState({ error: "Signup could not be completed." });
  }

  render() {
    return (
      <form onSubmit={this.handleSubmit} onKeyUp={this.handleKeyUp}>
        {this.state.error ? (
          <Alert type="error" text={this.state.error} />
        ) : null}
        <div className={styles.wrapper}>
          <input
            type="email"
            placeholder="Fill in your e-mail"
            value={this.state.email}
            onFocus={this.handleChange}
            onChange={this.handleChange}
          />
          <button
            className={styles.submit}
            disabled={this.state.error || !this.state.email}
          >
            <FontAwesomeIcon
              className={styles.submitIcon}
              icon={["far", "envelope"]}
              />
            {this.state.email ? (
              <span className={styles.submitLabel}>Sign up</span>
            ) : null}
          </button>
        </div>
      </form>
    );
  }
}