import React from "react";

import Grid from "@material-ui/core/Grid";

import styles from './style.module.scss';

export const ThreeSteps = () => {
  return (
    <Grid container className={styles.main} spacing={5}>
      <Grid item xs={12}>
        <h1>Crowdacting in 3 steps</h1>
      </Grid>
      <Grid item xs={12} md={4}>
        <h2>Proposal</h2>
        <p>Someone proposes a collective action and sets a target number of participants and a deadline</p>
      </Grid>
      <Grid item xs={12} md={4}>
        <h2>Crowd</h2>
        <p>Supporters pledge to join if the target is met before the deadline</p>
      </Grid>
      <Grid item xs={12} md={4}>
        <h2>Act</h2>
        <p>When the target number of supporters is met, we all act!</p>
      </Grid>
    </Grid>
  )
};
