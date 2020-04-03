import React from "react";
import { Container } from "@material-ui/core";

import styles from "./Section.module.scss";

interface ISectionProps {
  children: any;
  center?: boolean;
  color?: string;
  title?: string;
  className?: string;
  anchor?: string;
}

export const Section = ({
  children,
  center,
  color,
  title,
  anchor,
  className,
}: ISectionProps) => {
  // tslint:disable-next-line: prettier
  const classes = `${color ? styles[color] : ""} ${center ? styles.center : ""} ${className ? className : ""}`.trim();
  return (
    <section className={classes}>
      <Container>
        { title ? <h2 className={styles.title} id={anchor}>{ title }</h2> : null }
        { children }
      </Container>
    </section>
  );
};
