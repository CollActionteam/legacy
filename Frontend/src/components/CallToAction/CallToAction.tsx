import React from "react";

import Grid from "@material-ui/core/Grid";

import { Hidden } from "@material-ui/core";

import { CircleButton, CircleButtonContainer, Button } from "../Button/Button";

import styles from "./CallToAction.module.scss";

export const CallToAction = ({ title }: any) => {
  return (
    <div className={styles.callToAction}>
      <Grid container>
        {title !== undefined && (
          <Grid item xs={12}>
            <h1>{title}</h1>
          </Grid>
        )}
        <Hidden smDown>
          <Grid item xs={12}>
            <CircleButtonContainer>
              <CircleButton to="/crowdactions/find">Find Crowdaction</CircleButton>
              <CircleButton to="/crowdactions/start">Start Crowdaction</CircleButton>
            </CircleButtonContainer>
          </Grid>
        </Hidden>
        <Hidden mdUp>
          <Grid item xs={2}></Grid>
          <Grid item xs={8}>
            <div className={styles.buttonContainer}>
              <Button to="crowdactions/find">Find Crowdaction</Button>
              <Button to="crowdactions/start">Start Crowdaction</Button>
            </div>
          </Grid>
        </Hidden>
      </Grid>
    </div>
  );
};
