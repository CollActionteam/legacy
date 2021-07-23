import React from "react";
import styles from "./PageLoader.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const PageLoader = () => {
    return <div className={styles.pageLoader}>
        <FontAwesomeIcon icon="spinner" spin />
    </div>;
};

export default PageLoader;