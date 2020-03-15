import React from "react";

import styles from "./style.module.scss";

export const Faq = ({ children, title }: any) => (
  <div className={styles.faq}>
    <h3>{title}</h3>
    {children}
  </div>
);
