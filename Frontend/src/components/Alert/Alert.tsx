import React from "react";
import styles from "./Alert.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { IconProp } from "@fortawesome/fontawesome-svg-core";

interface IAlertProps {
  type: string;
  text: string | null | undefined;
  icon?: IconProp | undefined;
}

export const Alert = ({ type, text, icon } : IAlertProps) => 
  text ? <div className={styles[type]}>{ icon ? <FontAwesomeIcon icon={icon} /> : null } { text }</div> : null;
