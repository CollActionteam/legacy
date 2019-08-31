import React from "react";
import { Grid, Container } from '@material-ui/core';

import styles from './style.module.scss';

export const Intro = ({ title, content }) => {
  return (
    <Container className={ styles.intro }>
      <Grid container>
        <Grid item xs={12}>
          <h2>{title}</h2>
        </Grid>
        <Grid item xs={12}>
          <p dangerouslySetInnerHTML={{ __html: content }}></p>
        </Grid>
      </Grid>
    </Container>
  );
};
