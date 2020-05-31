import React from "react";
import styles from "./PageLoader.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export default () => {
    return <div className={styles.pageLoader}>
        <FontAwesomeIcon icon="spinner" spin />
    </div>;
};