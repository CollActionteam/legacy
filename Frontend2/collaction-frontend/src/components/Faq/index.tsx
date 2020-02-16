import React from "react";

import styles from "./style.module.scss";

export const Faq = ({ title, content }: any) => (
  <div className={styles.faq}>
    <h3>{title}</h3>
    <span dangerouslySetInnerHTML={{ __html: content }}></span>
  </div>
);
