import React from "react";
import { Grid } from "@material-ui/core";

import { Banner } from "../../../components/Banner/Banner";
import { Section } from "../../../components/Section/Section";
import { Button } from "../../../components/Button/Button";
import IntroCard from "./IntroCard";
import CrowdactionStartFaqs from "./CrowdactionStartFaqs";
import Kickstart from "./Kickstart";

import styles from "./Start.module.scss";
import { StartCrowdactionSteps } from "./StartCrowdactionSteps";
import {Helmet} from "react-helmet";

const StartCrowdactionPage = () => {
  return (
    <>
        <Helmet>
            <title>Start Crowdaction</title>
            <meta name="description" content="Start Crowdaction"/>
        </Helmet>
      <Banner>
        <Grid container className={styles.banner}></Grid>
      </Banner>

      <Grid container className={styles.cardContainer}>
        <IntroCard></IntroCard>
      </Grid>

      <Grid container>
        <Section color="grey" title="How it works">
          <StartCrowdactionSteps />
          <Grid item className={styles.callToAction} xs={12}>
            <Button to="/crowdactions/create">Start a crowdaction</Button>
          </Grid>
        </Section>
      </Grid>

      <Grid container>
        <Grid item md={2}></Grid>
        <Grid item md={8} xs={12}>
          <Section title="Frequently Asked Questions">
            <CrowdactionStartFaqs></CrowdactionStartFaqs>
          </Section>
        </Grid>
      </Grid>

      <Grid container>
        <Grid item md={2}></Grid>
        <Grid item md={8} xs={12}>
          <Section>
            <div className={styles.kickstart}>
              <Kickstart></Kickstart>
            </div>
          </Section>
        </Grid>
      </Grid>
    </>
  );
};

export default StartCrowdactionPage;
