import React from "react";
import { Container, Grid, Hidden } from "@material-ui/core";

import styles from "./Section.module.scss";

interface ISectionProps {
  children: any;
  center?: boolean;
  color?: string;
  title?: string;
  className?: string;
  anchor?: string;
  withOffset?: boolean;
}

export const Section = ({
  children,
  center,
  color,
  title,
  anchor,
  className,
  withOffset
}: ISectionProps) => {
  // tslint:disable-next-line: prettier
  const classes = `${color ? styles[color] : ""} ${center ? styles.center : ""} ${className ? className : ""}`.trim();
  
  if(withOffset) {
    return (
      <section className={classes}>
        <Container>
          <Grid container>
              <Hidden smDown>
                  <Grid item md={3}></Grid>
              </Hidden>
              <Grid item md={9}>
                { title ? <h2 id={anchor}>{ title }</h2> : null }
                { children }
              </Grid>
          </Grid>
        </Container>
      </section>
    );
  }

  return (
    <section className={classes}>
      <Container>
        { title ? <h2 className={styles.title} id={anchor}>{ title }</h2> : null }
        { children }
      </Container>
    </section>
  );
};
