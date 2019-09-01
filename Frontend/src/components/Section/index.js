import React from "react";
import styles from "./style.module.scss";

export default ({ children, color, title }) => (
  <section className={styles[color]}>
    <h3 className={styles.title}>{title}</h3>
    {children}
  </section>
);
