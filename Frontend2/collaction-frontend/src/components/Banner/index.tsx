import React from "react";
import styles from "./style.module.scss";
import { Grid } from "@material-ui/core";

export const Banner = ({ children, photo, dots = false }: any) => (
  <React.Fragment>
    {dots ? <div className={styles.dots}></div> : null}
    <Grid container>{children}</Grid>
  </React.Fragment>
);
