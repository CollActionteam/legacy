import React from "react";
import styles from "./style.module.scss";

export const Alert = ({ type, text } : any) => (
  <div className={styles[type]}>{text}</div>
);
