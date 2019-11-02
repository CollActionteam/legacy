import React from "react";
import styles from "./style.module.scss";
import { Grid } from "@material-ui/core";

export const Banner = ({ children, photo, dots = false }) => {
  return (
    <Grid
      container
      className={styles.banner}
      style={{ backgroundImage: `url(${photo})` }}
    >
      <div className={dots ? styles.dots : null}>{children}</div>
    </Grid>
  );
};
