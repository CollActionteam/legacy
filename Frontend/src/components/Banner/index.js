import React from "react";
import styles from "./style.module.scss";
import { Grid } from "@material-ui/core";

export const Banner = ({ children, photo }) => {
  return (
    <Grid container className={styles.banner} style={{ backgroundImage: `url(${photo})` }}>
      {children}
    </Grid>
  )
};
