import React from "react";
import { Grid } from "@material-ui/core";

import { Banner } from "../../../components/Banner/Banner";
import { Section } from "../../../components/Section/Section";
import { StartProjectSteps } from "../../../components/StartProjectSteps/StartProjectSteps";
import { Button } from "../../../components/Button/Button";
import IntroCard from "./IntroCard";
import ProjectStartFaqs from "./ProjectStartFaqs";
import Kickstart from "./Kickstart";

import styles from "./Start.module.scss";

const StartProjectPage = () => {
  return (
    <React.Fragment>
      <Banner>
        <Grid container className={styles.banner}></Grid>
      </Banner>

      <Grid container className={styles.cardContainer}>
        <IntroCard></IntroCard>
      </Grid>

      <Grid container>
        <Section color="grey" title="How it works">
          <StartProjectSteps />
          <Grid item className={styles.callToAction} xs={12}>
            <Button to="/projects/create">Start a project</Button>
          </Grid>
        </Section>
      </Grid>

      <Grid container>
        <Grid item md={2}></Grid>
        <Grid item md={8} xs={12}>
          <Section title="Frequently Asked Questions">
            <ProjectStartFaqs></ProjectStartFaqs>
          </Section>
        </Grid>
      </Grid>

      <Grid container>
        <Grid item md={2}></Grid>
        <Grid item md={8} xs={12}>
          <div className={styles.kickstart}>
            <Kickstart></Kickstart>
          </div>
        </Grid>
      </Grid>
    </React.Fragment>
  );
};

export default StartProjectPage;
