import React from "react";
import { Grid } from "@material-ui/core";

import styles from "./Banner.module.scss";

export const Banner = ({ children, photo, dots = false }: any) => (
  <div className={styles.banner}>
    {dots ? <div className={styles.dots}></div> : null}
    <Grid container>{children}</Grid>
  </div>
);
