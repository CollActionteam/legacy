import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import styles from "./Loader.module.scss";

const Loader = () => {
  return (
    <div className={styles.loader}>
      <FontAwesomeIcon icon="spinner" spin />
    </div>
  );
};

export default Loader;