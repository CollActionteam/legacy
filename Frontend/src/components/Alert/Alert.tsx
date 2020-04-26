import React, {FunctionComponent} from "react";
import styles from "./Alert.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { IconProp } from "@fortawesome/fontawesome-svg-core";

interface IAlertProps {
  type: string;
  text?: string | null;
  icon?: IconProp;
}

export const Alert : FunctionComponent<IAlertProps> = ({ type, text, icon, children }) =>
  text || children ? <div className={styles[type]}>{
    icon ? <FontAwesomeIcon icon={icon} /> : null
  } { children ? children : text }</div> : null;
