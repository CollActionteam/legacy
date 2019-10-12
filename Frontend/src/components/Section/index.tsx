import React from "react";
import styles from "./style.module.scss";
import { Container } from "@material-ui/core";

interface ISectionProps {
  children: any;
  indent?: boolean;
  color?: string;
  title?: string;
}

export const Section = ({ children, indent, color, title }: ISectionProps) => {
  // tslint:disable-next-line: prettier
  const classes = `${color ? styles[color] : ""} ${indent ? styles.indent : ""}`;
  return (
    <section className={classes}>
      <Container>
        {title ? <h3 className={styles.title}>{title}</h3> : null}
        {children}
      </Container>
    </section>
  );
};
