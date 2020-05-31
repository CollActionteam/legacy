import React from "react";
import { Helmet } from "react-helmet";
import { Section } from "../../../components/Section/Section";

import styles from './RegistrationComplete.module.scss';
import { Container } from "@material-ui/core";
import { Link } from "react-router-dom";

const RegistrationCompletePage = () => {
  return (
    <>
      <Helmet>
        <title>Create a crowdaction</title>
        <meta name="description" content="Create a crowdaction" />
      </Helmet>

      <Section title="Thank you for registering!" className={styles.title}>
        <Container className={styles.login}>
          <p>You can now <Link to="/account/login">login</Link> to start a new Crowdaction, or join others.</p>
        </Container>
      </Section>
    </>
  )
}

export default RegistrationCompletePage;