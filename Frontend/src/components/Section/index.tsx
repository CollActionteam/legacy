import React from "react";
import styles from "./style.module.scss";
import { Container } from "@material-ui/core";

export default ({
  children,
  color,
  title,
}: {
  children: any;
  color?: string;
  title?: string;
}) => (
  <section className={color ? styles[color] : null}>
    <Container>
      {title ? <h3 className={styles.title}>{title}</h3> : null}
      {children}
    </Container>
  </section>
);
