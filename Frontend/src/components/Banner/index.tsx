import React from "react";
import styles from "./style.module.scss";
import { Grid } from "@material-ui/core";
import BackgroundImage from "gatsby-background-image";

export const Banner = ({ children, photo, dots = false }) => {
  return (
    <BackgroundImage fluid={photo}>
      {dots ? <div className={styles.dots}></div> : null}
      <Grid container>{children}</Grid>
    </BackgroundImage>
  );
};
