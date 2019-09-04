import React from "react";
import { Button } from "../Button";
import styles from "./style.module.scss";

export default class NewsletterSignup extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      error: "",
    };
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidMount() {}

  handleSubmit(event) {
    event.preventDefault();
    this.setState({ error: "Signup could not be completed." })
  }

  render() {
    return (
      <form onSubmit={this.handleSubmit}>
        {this.state.error ? <Error message={this.state.error} /> : null}
        <div className={styles.wrapper}>
          <input type="email" placeholder="Fill in your e-mail"></input>
          <Button type="submit">Sign up</Button>
        </div>
      </form>
    );
  }
}

const Error = ({ message }) => <div className={styles.error}>{message}</div>;
