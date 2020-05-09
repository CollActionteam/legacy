import React from "react";
import Loader from "../Loader/Loader";
import styles from "./PageLoader.module.scss";

export default () => {
    return <div className={styles.pageLoader}>
        <Loader />
    </div>;
};