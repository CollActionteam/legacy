import React from "react";
import styles from "./style.module.scss";
import { Container } from "@material-ui/core";

interface ISectionProps {
  children: any;
  center?: boolean;
  color?: string;
  title?: string;
  className?: string;
}

export const Section = ({
  children,
  center,
  color,
  title,
  className,
}: ISectionProps) => {
  // tslint:disable-next-line: prettier
  const classes = `${color ? styles[color] : ""} ${center ? styles.center : ""} ${className ? className : ""}`.trim();
  return (
    <section className={classes}>
      <Container>
        {title ? <h2 className={styles.title}>{title}</h2> : null}
        {children}
      </Container>
    </section>
  );
};
