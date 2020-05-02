import React from "react";
import { Helmet } from "react-helmet";
import { Section } from "../../../components/Section/Section";

import styles from './RegistrationComplete.module.scss';
import { Container } from "@material-ui/core";
import { Link } from "react-router-dom";

const RegistrationCompletePage = () => {
  return (
    <React.Fragment>
      <Helmet>
        <title>Create a project</title>
        <meta name="description" content="Create a project" />
      </Helmet>

      <Section title="Thank you for registering!" className={styles.title}>
        <Container className={styles.login}>
          <p>You can now <Link to="/account/login">login</Link> to start a new Crowdaction, or join others.</p>
        </Container>
      </Section>
    </React.Fragment>
  )
}

export default RegistrationCompletePage;