import React from "react";

import Grid from "@material-ui/core/Grid";

import { Hidden } from "@material-ui/core";

import { CircleButton, CircleButtonContainer, Button } from "../Button/Button";

import styles from "./style.module.scss";

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
              <CircleButton to="/projects/find">Find Project</CircleButton>
              <CircleButton to="/projects/start">Start Project</CircleButton>
            </CircleButtonContainer>
          </Grid>
        </Hidden>
        <Hidden mdUp>
          <Grid item xs={2}></Grid>
          <Grid item xs={8}>
            <div className={styles.buttonContainer}>
              <Button to="projects/find">Find Project</Button>
              <Button to="projects/start">Start Project</Button>
            </div>
          </Grid>
        </Hidden>
      </Grid>
    </div>
  );
};
