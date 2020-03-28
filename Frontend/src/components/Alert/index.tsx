import React from "react";
import styles from "./style.module.scss";

interface IAlertProps {
  type: string;
  text: string | null | undefined;
}

export const Alert = ({ type, text } : IAlertProps) => (
  text ? <div className={styles[type]}>{text}</div> : null
);
