import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import styles from "./Loader.module.scss";

export default () => {
  return (
    <div className={styles.loader}>
      <FontAwesomeIcon icon="spinner" spin />
    </div>
  );
};
